using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthFoodAPI.Models
{
    public class RecipesImageItem
    {
        public string Title { get; set; }
        public IFormFile Image { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string CookTime { get; set; }
        public string TimeUnit { get; set; }
        public string Size { get; set; }
        public string Dietary { get; set; }
        public string Link { get; set; }
        public string PostName { get; set; }
    }
}
