using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialResearch.Models
{
    public class Stage
    {
        [Required] //Обязательное поле
        //[Display(Name = "Номер этапа заявки")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Заполните название этапа заявки")]
        [Display(Name = "Название этапа заявки")]
        public string StageName { get; set; }
    }
}
