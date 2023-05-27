using System.ComponentModel.DataAnnotations;

namespace BlogApi.ViewModels
{
    public class EditorCategoryViewModel
    {
        [Required(ErrorMessage ="The name is mandatory")]
        public string Name { get; set; }

        [Required(ErrorMessage ="The slug is mandatory")]
        public string Slug { get; set; }
    }
}
