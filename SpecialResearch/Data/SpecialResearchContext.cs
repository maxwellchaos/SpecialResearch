using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        }
    }
}