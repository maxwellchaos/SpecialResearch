using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialResearch.Models
{
    public class Equipment
    {
        [Required]
        public int Id { get; set; }//Guid - глобальный идентификатор

        [Required] //Обязательное поле
        [Display(Name = "Наименование оборудования")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Заявка")]        //[Display(Request.Number,Name = "Заявка")]
        public int RequestId { get; set; }
        public Request Request { get; set; }

        [Display(Name = "Производитель")]
        public string Manufacturer { get; set; }

        [Display(Name = "Модель")]
        public string Model { get; set; }

        [Display(Name = "Серийный номер")]
        public string SerialNumber { get; set; }

        [Display(Name = "Режим работы")]
        public string OperatingMode { get; set; }

        //[Display(Name = "Фотокопия серийного номера")]
        //public Bitmap PhotoCopy { get; set; }
    }
}
