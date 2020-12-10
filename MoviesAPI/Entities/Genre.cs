using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MoviesAPI.Validations;

namespace MoviesAPI.Entities
{
    public class Genre : IValidatableObject //Model validation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The field with name {0} is required")]
        [StringLength(10)]
        //[FirstLetterUppercase] // Custom attribute validation
        public string Name { get; set; }

        //[Range(18, 120)]
        //public int Age { get; set; }
        //[CreditCard]
        //public string CreditCard { get; set; }
        //[Url]
        //public string Url { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var firstLetter = Name[0].ToString();

                if (firstLetter != firstLetter.ToUpper())
                {
                    yield return new ValidationResult("First letter should be uppercase", 
                        new string[] { nameof(Name) });
                }
            }
        }
    }
}
