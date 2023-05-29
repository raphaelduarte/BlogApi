using Blog.Data;
using Blog.Models;
using BlogApi.Extensions;
using BlogApi.Services;
using BlogApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace BlogApi.Controllers
{

    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AccountController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet("v1/accounts/")]
        public IActionResult Get([FromServices] BlogDataContext context)
        {
            return Ok(context.Users.ToList());
        }

        [HttpPut("v1/accounts/{id:int}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] RegisterViewModel model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
            }

            var user = context.Users.FirstOrDefault(x => x.Id == id);

            user.Name = model.Name;
            user.Email = model.Email;

            context.SaveChanges();

            return Ok(new ResultViewModel<User>(user));
        }

        [HttpPost("v1/accounts")]
        public async Task<IActionResult> PostAsync(
            [FromBody] RegisterViewModel model,
            [FromServices] EmailService emailService,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Bio = model.Name ,
                Slug = model.Email.Replace("@","-").Replace(".","-")
            };

            var passaword = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(passaword);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                emailService.Send(
                    user.Name,
                    user.Email,
                    "Bem-vindo",
                    $"sua senha é {passaword}");

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email, passaword
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>("This email is already registred"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("Internal server fail"));
            }
        }
        
        [HttpDelete("v1/accounts/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            var user = await context.Users.FirstOrDefaultAsync(c => c.Id == id);

            if (user == null)
            {
                return NotFound(new ResultViewModel<User>("User is not found"));
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<User>(user));
        }

        [HttpPost("v1/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, [FromServices] BlogDataContext context, [FromServices] TokenService tokenService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
            }

            var user = await context
                .Users
                .AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
            {
                return StatusCode(401, new ResultViewModel<string>("User or password is invalid"));
            }

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            {
                return StatusCode(401, new ResultViewModel<string>("User or password is invalid"));
            }

            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("internal fail on server"));
            }
        }
    }
}