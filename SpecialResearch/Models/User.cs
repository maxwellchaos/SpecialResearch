using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialResearch.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }

        [Required] //Обязательное поле
        [Display(Name = "Логин")]  //[Display(Request.Number,Name = "Заявка")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Права доступа")]        
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [Display(Name = "ФИО")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Пароль(Хеш)")]
        public string Password { get; set; }

    }
}
