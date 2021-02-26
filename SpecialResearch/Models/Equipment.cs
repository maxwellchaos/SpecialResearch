using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Display(Name = "Заявка")]       
        public int RequestId { get; set; }
        [Display(Name = "Заявка")]        
        public Request Request { get; set; }

        [Display(Name = "Производитель")]
        public string Manufacturer { get; set; }

        [Display(Name = "Модель")]
        public string Model { get; set; }

        [Display(Name = "Серийный номер")]
        public string SerialNumber { get; set; }

        [Display(Name = "Режим работы")]
        public string OperatingMode { get; set; }

        [Display(Name = "Фотокопия серийного номера")]
        public string PhotoCopy { get; set; }

        [NotMapped]
        [Display(Name = "Количество испытаний")]
        public int TestResultCount { get; set; }

        [NotMapped]
        [Display(Name = "Количество проваленных испытаний")]
        public int TestResultFailCount { get; set; }

    }
}
