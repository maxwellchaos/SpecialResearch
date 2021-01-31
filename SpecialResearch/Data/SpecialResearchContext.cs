using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpecialResearch.Models;

namespace SpecialResearch.Data
{
    public class SpecialResearchContext : DbContext
    {
        public SpecialResearchContext(DbContextOptions<SpecialResearchContext> options)
            : base(options)
        {
        }

        public DbSet<SpecialResearch.Models.Stage> Stage { get; set; }

        //Заполняю значения по умолчанию
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Этапы заявки по умолчанию
            builder.Entity<Stage>().HasData(new Stage
            {
                Id = 1,
                StageName = "Заявка принята"
            });
            builder.Entity<Stage>().HasData(new Stage
            {
                Id = 2,
                StageName = "Испытания проведены"
            });
            builder.Entity<Stage>().HasData(new Stage
            {
                Id = 3,
                StageName = "Предписание выдано"
            });
            builder.Entity<Stage>().HasData(new Stage
            {
                Id = 4,
                StageName = "Заявка закрыта"
            });



            //Права доступа(роли) по умолчанию
            builder.Entity<Role>().HasData(new Role
            {
                Id = 1,
                Name = Startup.AdminRole,
                Description = "Может всё"
            });
            builder.Entity<Role>().HasData(new Role
            {
                Id = 2,
                Name = Startup.RecieverRole,
                Description = "Приемщик СВТ"
            });
            builder.Entity<Role>().HasData(new Role
            {
                Id = 3,
                Name = Startup.TesterRole,
                Description = "Испытатель."
            });
            builder.Entity<Role>().HasData(new Role
            {
                Id = 4,
                Name = Startup.ControllerRole,
                Description = "Контролер. Может выдавать предписания"
            });
            builder.Entity<Role>().HasData(new Role
            {
                Id = 5,
                Name = Startup.ManagerRole,
                Description = "Управленец. Может все смотреть. Отчеты - его главная страница"
            });

            //Юзер

            builder.Entity<User>().HasData(new User
            {
                Id = 1,
                Login = "Nobody",
                Name = "Никто",
                RoleId = 1,
                Password = GetHashString("tewhbx9438j09v8i3ujpviwerufhw98")
            }); ;
            builder.Entity<User>().HasData(new User
            {
                Id = 2,
                Login = "admin",
                Name = "Иванов И.И.",
                RoleId = 1,
                Password = GetHashString("admin")
            }) ;
        }

        internal static string GetHashString(string text)
        {
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        //Заполняю значения по умолчанию
        public DbSet<SpecialResearch.Models.Role> Role { get; set; }

        //Заполняю значения по умолчанию
        public DbSet<SpecialResearch.Models.User> User { get; set; }

        //Заполняю значения по умолчанию
        public DbSet<SpecialResearch.Models.Request> Request { get; set; }

        //Заполняю значения по умолчанию
        public DbSet<SpecialResearch.Models.Equipment> Equipment { get; set; }

        //Заполняю значения по умолчанию
        public DbSet<SpecialResearch.Models.Interface> Interface { get; set; }

        //Заполняю значения по умолчанию
        public DbSet<SpecialResearch.Models.TestType> TestType { get; set; }

        //Заполняю значения по умолчанию
        public DbSet<SpecialResearch.Models.TestResult> TestResult { get; set; }

  
    }
}