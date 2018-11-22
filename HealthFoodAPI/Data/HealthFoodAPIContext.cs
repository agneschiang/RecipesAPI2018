using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HealthFoodAPI.Models
{
    public class HealthFoodAPIContext : DbContext
    {
        public HealthFoodAPIContext (DbContextOptions<HealthFoodAPIContext> options)
            : base(options)
        {
        }

        public DbSet<HealthFoodAPI.Models.RecipesItem> RecipesItem { get; set; }
    }
}
