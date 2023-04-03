using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;


namespace SpecialResearch.Models
{
    public class Request
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage ="Заполните номер заявки")] //Обязательное поле
        [Display(Name = "Номер заявки")]
        public string Number { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreateDate { get; set; }

        
        //Подключаем таблицу этапов
        [Display(Name = "Этап заявки")]
        public int StageID { get; set; }
        public Stage Stage { get; set; }


        [Display(Name = "Создатель заявки")]
        public int? CreatorId { get; set; }
        [Display(Name = "Создатель заявки")]
        public User Creator { get; set; }

        [Display(Name = "Выдавший предписание")]
        public int ControlerId { get; set; }
        [Display(Name = "Выдавший предписание")]
        public User Controler { get; set; }

        [Display(Name = "Предписание на эксплуатацию")]
        public string UseOrder { get; set; }


        [Display(Name = "Дата завершения")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Фотокопия")]
        public string PhotoCopy { get; set; }
    }
}
