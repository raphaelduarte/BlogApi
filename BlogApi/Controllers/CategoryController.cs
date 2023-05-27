using Blog.Data;
using Blog.Models;
using BlogApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using BlogApi.Extensions;

namespace BlogApi.Controllers
{
    [Route("/")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
        {
            var categories = await context.Categories.ToListAsync();
            return Ok(new ResultViewModel<List<Category>>(categories));
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new ResultViewModel<Category>("Content not found"));
            }

            return Ok(new ResultViewModel<Category>(category));
        }

        [HttpPost("v1/categories/{id:int}")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));
            }

            try
            {
                var category = new Category
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug.ToLower(),
                };

                context.Categories.Add(category);
                context.SaveChanges();

                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch(DbException ex)
            {
                return BadRequest(new ResultViewModel<List<Category>>("It was impossible return category"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("Internal fail on server"));
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id); 

            if (category == null)
            {
                return NotFound(new ResultViewModel<Category>("Content is not found"));
            }

            category.Name = model.Name;
            category.Slug = model.Slug;

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Category>(category));
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new ResultViewModel<Category>("Content is not found"));
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Category>(category));
        }
    }
}
