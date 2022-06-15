using System.ComponentModel.DataAnnotations;

namespace asp.blazor.Models
{
    [Serializable]
    public class FormsModel
    {
        [Required]
        [Range(typeof(bool), "true", "true",
             ErrorMessage = "Need to check.")]
        public bool Check { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Decimal Decimal { get; set; }

        [Required]
        public int Integer { get; set; }

        [Required]
        public Salad SomeSalad { get; set; }

        [Required, MinLength(1), MaxLength(4)]
        public Salad[] SaladSelection { get; set; } = Array.Empty<Salad>();

        [Required]
        public string Line { get; set; } = default!;

        [Required]
        public string Paragraph { get; set; } = default!;
    }

    public enum Salad
    {
        Beans,
        Corn,
        Eggs,
        Lentils,
    }
}