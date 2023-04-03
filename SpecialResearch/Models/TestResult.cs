using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialResearch.Models
{
    public class TestResult
    {

        [Required]
        public int Id { get; set; }//Guid - глобальный идентификатор

        [Required]
        [Display(Name = "Оборудование")]
        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        [Display(Name = "Результат испытания")]
        public string Result { get; set; }

        [Display(Name = "Интерфейс")]
        public int InterfaceId { get; set; }
        public Interface Interface { get; set; }

        [Display(Name = "Сигнал обнаружен")]
        public bool SignalFound { get; set; }

        [Display(Name = "Тест пройден")]
        public bool TestIsOk { get; set; }

        [Display(Name = "Дата проведения испытания")]
        public DateTime Date { get; set; }

        [Display(Name = "Тактовая частота")]
        public int frequency { get; set; }


        [Display(Name = "Тип испытания")]
        public int TestTypeId { get; set; }
        public TestType TestType { get; set; }

        [Display(Name = "Испытатель")]
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
