using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthFoodAPI.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new HealthFoodAPIContext(
                serviceProvider.GetRequiredService<DbContextOptions<HealthFoodAPIContext>>()))
            {
                // Look for any movies.
                if (context.RecipesItem.Count() > 0)
                {
                    return;   // DB has been seeded
                }

                context.RecipesItem.AddRange(
                    new RecipesItem
                    {
                        Title = "Is Mayo an Instrument?",
                        Description = "Here is an easy but super tasty variation to the classic potato salad. Using Mediterranean Chunky Roasted Capsicum Dip adds a lovely creaminess while the addition of chorizo and olives provide a deliciously salty and spicy flavour.",
                        Category = "Vegetarian",
                        Subcategory = "Potato",
                        CookTime = "30",
                        TimeUnit = "mins",
                        Size = "4",
                        Dietary = "",
                        Image = "https://i.kym-cdn.com/photos/images/original/001/371/723/be6.jpg",
                        Link = "https://www.foodinaminute.co.nz/Recipes/Greek-Style-Potato-Salad2",
                        Uploaded = "07-10-18 4:20T18:25:43.511Z",
                        PostName = "Agnes Chiang",
                        Width = "768",
                        Height = "432"
                    }


                );
                context.SaveChanges();
            }
        }



    }
}
