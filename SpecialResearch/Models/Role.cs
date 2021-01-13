using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialResearch.Models
{
    public class Role
    {
        [Required]
        public int Id { get; set; }

        [Required] //Обязательное поле
        [Display(Name = "Права доступа")]
        public string Name { get; set; }

        [Display(Name = "Описание прав доступа")]
        public string Description { get; set; }
    }
}
