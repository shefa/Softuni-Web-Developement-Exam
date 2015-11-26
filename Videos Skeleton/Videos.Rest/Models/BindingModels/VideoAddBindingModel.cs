using System.ComponentModel.DataAnnotations;

namespace Videos.Rest.Models.BindingModels
{
    public class VideoAddBindingModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int? locationId { get; set; }
    }
}