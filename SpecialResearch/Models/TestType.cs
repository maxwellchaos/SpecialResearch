using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialResearch.Models
{
    public class TestType
    {
        [Required]
        public int Id { get; set; }

        [Required] //Обязательное поле
        [Display(Name = "Название испытания")]
        public string TestName { get; set; }


        [Display(Name = "Описание испытания")]
        public string TestDescription { get; set; }
    }
}
