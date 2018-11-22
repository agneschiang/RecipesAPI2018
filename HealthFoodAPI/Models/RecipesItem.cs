using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthFoodAPI.Models
{
    public class RecipesItem
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string CookTime { get; set; }
        public string TimeUnit { get; set; }
        public string Size { get; set; }
        public string Dietary { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        public string Uploaded { get; set; }
        public string PostName { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

    }


}
