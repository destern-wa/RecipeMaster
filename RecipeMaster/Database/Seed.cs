using RecipeMaster.Interface;
using RecipeMaster.Model;
using System.Collections.Generic;
using System.Linq;

namespace RecipeMaster.Database
{
    /// <summary>
    /// Static utility class for seeding an recipe database with data
    /// </summary>
    public static class Seed
    {

        /// <summary>
        /// Seeds a recipe database with measures, categories, ingredients, and recipes
        /// </summary>
        /// <param name="recipeDatabase">Database to seed</param>
        public static void SeedDatabase(IRecipeDatabase recipeDatabase)
        {
            SeedMeasures(recipeDatabase);
            SeedCategories(recipeDatabase);
            SeedIngredients(recipeDatabase);
            SeedRecipes(recipeDatabase);
        }

        /// <summary>
        /// Seeds a recipe database with measures
        /// </summary>
        /// <param name="recipeDatabase">Database to seed</param>
        private static void SeedMeasures(IRecipeDatabase recipeDatabase)
        {

            List<Measure> measures = new List<Measure>()
            {
                // Dry units
                new Measure("g"),
                new Measure("kg"),
                new Measure("oz"), // ounces
                new Measure("lb"), // pounds

                // Wet units
                new Measure("mL"),
                new Measure("L"),
                new Measure("fl oz"), // fluid ounces
                new Measure("pint"),
                new Measure("gal"), // gallons

                // Wet-or-dry units
                new Measure("teaspoon"),
                new Measure("tablespoon"),
                new Measure("cups"),

                // Fractional units
                new Measure("eighths"),
                new Measure("quater"),
                new Measure("half"),
                new Measure("whole"),

                // Misc.
                new Measure("slice"),
                new Measure("dash"),
            };
            recipeDatabase.AddAll(typeof(Measure), measures);
        }

        /// <summary>
        /// Seeds a recipe database with categories
        /// </summary>
        /// <param name="recipeDatabase">Database to seed</param>
        private static void SeedCategories(IRecipeDatabase recipeDatabase)
        {

            List<Category> categories = new List<Category>()
            {
                new Category("Breakfast"),
                new Category("Pasta"),
                new Category("Soup"),
                new Category("Drinks"),
                new Category("Dessert"),
            };
            recipeDatabase.AddAll(typeof(Category), categories);
        }

        /// <summary>
        /// Seeds a recipe database with ingredients
        /// </summary>
        /// <param name="recipeDatabase">Database to seed</param>
        private static void SeedIngredients(IRecipeDatabase recipeDatabase)
        {
            List<Ingredient> ingredients = new List<Ingredient>() {
                new Ingredient("flour"),
                new Ingredient("sugar"),
                new Ingredient("baking powder"),
                new Ingredient("milk"),
                new Ingredient("egg"),
                new Ingredient("banana"),
                new Ingredient("sausages"),
                new Ingredient("bacon rashers"),
                new Ingredient("wraps"),
                new Ingredient("ketchup"),
                new Ingredient("pasta"),
                new Ingredient("butter"),
                new Ingredient("cheddar cheese"),
                new Ingredient("chicken breasts"),
                new Ingredient("onion"),
                new Ingredient("carrots"),
                new Ingredient("celery stalks"),
                new Ingredient("chicken stock or broth"),
                new Ingredient("Irish cream liqueur"),
                new Ingredient("Irish whiskey"),
                new Ingredient("hot brewed coffee"),
                new Ingredient("whipped cream"),
                new Ingredient("ground nutmeg"),
                new Ingredient("puff pastry"),
                new Ingredient("apricot jam"),
                new Ingredient("apples"),
                new Ingredient("ground cinnamon"),
                new Ingredient("breadcrumbs"),
                new Ingredient("powdered sugar"),
            };
            recipeDatabase.AddAll(typeof(Ingredient), ingredients);
        }

        /// <summary>
        /// Seeds a recipe database with recipes. Requires database to first be seeded
        /// with <see cref="SeedMeasures"/> and <see cref="SeedCategories"/> and
        /// <see cref="SeedIngredients"/>
        /// </summary>
        /// <param name="recipeDatabase">Database to seed</param>
        private static void SeedRecipes(IRecipeDatabase recipeDatabase)
        {
            List<Measure> measures = recipeDatabase.GetAll(typeof(Measure)).Cast<Measure>().ToList();
            List<Category> categories = recipeDatabase.GetAll(typeof(Category)).Cast<Category>().ToList();
            List<Ingredient> ingredients = recipeDatabase.GetAll(typeof(Ingredient)).Cast<Ingredient>().ToList();
            // CC0 recipes from Based Cooking https://based.cooking/
            List<Recipe> recipes = new List<Recipe>() {
                new Recipe(
                    "Banana Pancakes",
                    20,
                    "1. Combine flour, sugar, baking powder and a little bit of salt in a large mixing bowl.\n" +
                    "2. Whisk milk and egg into the flour mixture until no clumps remain in the resulting batter.\n" +
                    "3. Add mashed bananas and mix well.\n" +
                    "4. Add some butter to a medium warm frying pan.\n" +
                    "5. Make smaller or bigger pancakes, up to you. Wait until tiny air bubbles form on top (2 to 5 minutes), turn and continue frying until the bottom is browned. Repeat.\n\n" +
                    "Either eat the pancakes as they get ready or put a plate in a preheated oven(low degree) to keep the ready pancakes warm.",
                    4,
                    null,
                    categories.Find(m => m.Name=="Breakfast"),
                    new List<RecipeIngredient>()
                    {
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="flour"),
                            Measure = measures.Find(m => m.Name == "cups"),
                            Quantity = 1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="sugar"),
                            Measure = measures.Find(m => m.Name == "tablespoon"),
                            Quantity = 1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="baking powder"),
                            Measure = measures.Find(m => m.Name == "teaspoon"),
                            Quantity = 2
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="milk"),
                            Measure = measures.Find(m => m.Name == "cups"),
                            Quantity = 1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="egg"),
                            Measure = measures.Find(m => m.Name == "whole"),
                            Quantity = 1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="banana"),
                            Measure = measures.Find(m => m.Name == "whole"),
                            Quantity = 2
                        },
                    }
                ),

                new Recipe(
                    "Breakfast Wrap",
                    15,
                    "1. Begin by cutting the sausages in half and frying them ensuring that they’re cooked the whole way through.\n"+
                    "2. Next, fry the bacon.\n"+
                    "3. Next, possibly while the bacon is frying, crack an egg into a bowl, add a dash of milk and whisk it\n"+
                    "4. Then, put the egg mixture into the microwave for a minute, take it out and whisk it again, do this twice again or until the scrambled egg is ready.\n"+
                    "5. Add the sausage, bacon and scrambled egg to a wrap (I recommend two sausage halves, a bacon rasher and half the bowl of scrambled egg per wrap). Add ketchup if desired.",
                    1,
                    null,
                    categories.Find(m => m.Name=="Breakfast"),
                    new List<RecipeIngredient>()
                    {
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="sausages"),
                            Measure = measures.Find(m => m.Name == "whole"),
                            Quantity = 2
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="bacon rashers"),
                            Measure = measures.Find(m => m.Name == "whole"),
                            Quantity = 2
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="egg"),
                            Measure = measures.Find(m => m.Name == "whole"),
                            Quantity = 1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="wraps"),
                            Measure = measures.Find(m => m.Name == "whole"),
                            Quantity = 2
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="milk"),
                            Measure = measures.Find(m => m.Name == "tablespoon"),
                            Quantity = 1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="ketchup"),
                            Measure = measures.Find(m => m.Name == "tablespoon"),
                            Quantity = 1
                        },
                    }
                ),

                new Recipe(
                    "Cheesy Pasta Bake",
                    75,
                    "1. Grate the cheese.\n"+
                    "2. Chop up the bacon into roughly 2cm x 2cm squares.\n"+
                    "3. Start cooking the pasta.\n"+
                    "4. Start melting the butter in another saucepan.\n"+
                    "5. Once melted, add flour and mix until the mixture forms a paste that does not stick to the sides. The quantity of flour required may vary.\n"+
                    "6. Now start adding the milk slowly, while mixing, as to avoid lumps.\n"+
                    "7. Heat on low heat while stirring until the sauce becomes thick. If unsure, taste the sauce, and if you cannot taste the flour, it is ready for the next step.\n"+
                    "8. Add 80% of the cheese and stir until melted in.\n"+
                    "9. Drain the pasta and add to baking dish.\n"+
                    "10. Add the sauce and mix in.\n"+
                    "11. If using bacon, fry it up and mix into the dish, making sure to include any fat that comes out of the bacon for extra flavour.\n"+
                    "12. Use the rest of the cheese to cover the top.\n"+
                    "13. Bake at gas mark 6 for about 30 minutes.",
                    4,
                    null,
                    categories.Find(c=>c.Name=="Pasta"),
                   new List<RecipeIngredient>()
                    {
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="pasta"),
                            Measure = measures.Find(m => m.Name == "g"),
                            Quantity = 350
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="butter"),
                            Measure = measures.Find(m => m.Name == "g"),
                            Quantity = 50
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="flour"),
                            Measure = measures.Find(m => m.Name == "g"),
                            Quantity = 100
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="milk"),
                            Measure = measures.Find(m => m.Name == "mL"),
                            Quantity = 20
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="cheddar cheese"),
                            Measure = measures.Find(m => m.Name == "g"),
                            Quantity = 400
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="bacon rashers"),
                            Measure = measures.Find(m => m.Name == "whole"),
                            Quantity = 6
                        }
                    }
                ),

                new Recipe(
                    "Chicken soup",
                    150,
                    "1. Cook chicken breasts, shred, and set aside in a bowl.\n"+
                    "2. Cut up carrots and celery, place in pot and saute.\n"+
                    "3. Add in chicken and stock or broth and mix together well. Season with salt, pepper, hot sauce, whatever you desire\n"+
                    "4. Allow it to simmer on low heat for 2 hours mixing every so often.\n",
                    null,
                    null,
                    categories.Find(c=>c.Name=="Soup"),
                    new List<RecipeIngredient>()
                    {
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="chicken breasts"),
                            Measure = measures.Find(m=>m.Name=="whole"),
                            Quantity = 2
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="onion"),
                            Measure = measures.Find(m=>m.Name=="whole"),
                            Quantity = 1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="carrots"),
                            Measure = measures.Find(m=>m.Name=="whole"),
                            Quantity = 2
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="celery stalks"),
                            Measure = measures.Find(m=>m.Name=="whole"),
                            Quantity = 2
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="chicken stock or broth"),
                            Measure = measures.Find(m=>m.Name=="mL"),
                            Quantity = 950
                        },
                    }
                ),

                new Recipe(
                    "Irish Coffee",
                    10,
                    "In a coffee mug, combine Irish cream and Irish whiskey. Fill mug with coffee. Top with a dab of whipped cream and a dash of nutmeg.",
                    1,
                    null,
                    categories.Find(c=>c.Name=="Drinks"),
                    new List<RecipeIngredient>()
                    {
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="Irish cream liqueur"),
                            Measure = measures.Find(m=>m.Name == "fl oz"),
                            Quantity = 1.5
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="Irish whiskey"),
                            Measure = measures.Find(m=>m.Name == "fl oz"),
                            Quantity = 1.5
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="hot brewed coffee"),
                            Measure = measures.Find(m=>m.Name == "cups"),
                            Quantity = 1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="whipped cream"),
                            Measure = measures.Find(m=>m.Name == "tablespoon"),
                            Quantity = 1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient = ingredients.Find(i =>i.Name=="ground nutmeg"),
                            Measure = measures.Find(m=>m.Name == "dash"),
                            Quantity = 1
                        },
                    }
                ),

                new Recipe(
                    "Apple strudel",
                    50,
                    "1. Peel and cut the apples in thin slices.\n"+
                    "2. Roll out the puff pastry with a rolling pin.\n"+
                    "3. Spread the jam over the puff pastry with a spoon.\n"+
                    "4. Arrange the apple slices over the jam.\n"+
                    "5. Sprinkle with cinnamon and breadcrumbs.\n"+
                    "6. Cut tiny pieces of butter and arrange them over the apple slices.\n"+
                    "7. Roll the puff pastry edges over, overlapping them.\n"+
                    "8. Bake for around 40 minutes at 180°C (360° F).\n"+
                    "9. Cover with powdered sugar.",
                    6,
                    null,
                    categories.Find(c=>c.Name=="Dessert"),
                    new List<RecipeIngredient>()
                    {
                        new RecipeIngredient()
                        {
                            Ingredient=ingredients.Find(i =>i.Name=="puff pastry"),
                            Measure = measures.Find(m=>m.Name=="slice"),
                            Quantity=1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient=ingredients.Find(i =>i.Name=="apricot jam"),
                            Measure = measures.Find(m=>m.Name=="oz"),
                            Quantity=10
                        },
                        new RecipeIngredient()
                        {
                            Ingredient=ingredients.Find(i =>i.Name=="apples"),
                            Measure = measures.Find(m=>m.Name=="whole"),
                            Quantity=3
                        },
                        new RecipeIngredient()
                        {
                            Ingredient=ingredients.Find(i =>i.Name=="ground cinnamon"),
                            Measure = measures.Find(m=>m.Name=="tablespoon"),
                            Quantity=1
                        },
                        new RecipeIngredient()
                        {
                            Ingredient=ingredients.Find(i =>i.Name=="breadcrumbs"),
                            Measure = measures.Find(m=>m.Name=="cups"),
                            Quantity=0.5
                        },
                        new RecipeIngredient()
                        {
                            Ingredient=ingredients.Find(i =>i.Name=="butter"),
                            Measure = measures.Find(m=>m.Name=="cups"),
                            Quantity=0.25
                        },
                        new RecipeIngredient()
                        {
                            Ingredient=ingredients.Find(i =>i.Name=="powdered sugar"),
                            Measure = measures.Find(m=>m.Name=="cups"),
                            Quantity=0.5
                        },
                    }
                )
            };
            recipeDatabase.AddAll(typeof(Recipe), recipes);
        }
    }
}
