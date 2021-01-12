using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialResearch.Models
{
    public class Interface
    {
        [Required]
        public int Id { get; set; }

        [Required] //Обязательное поле
        [Display(Name = "Название интерфейса")]
        public string Name { get; set; }


        [Display(Name = "Нормальные показатели")]
        public string NormalState { get; set; }
    }
}
