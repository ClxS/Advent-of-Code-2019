namespace Day14
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Refinery
    {
        public Refinery(Recipe[] recipes)
        {
            this.Recipes = recipes;
        }

        public Recipe[] Recipes { get; }

        public static Refinery Create(string recipe)
        {
            var entries = recipe.Split('\n');
            return new Refinery(entries.Select(Recipe.Create).ToArray());
        }

        public ProductQuantity GetOreRequirement(Product product, long amountToProduce = 1)
        {
            var depthFirstOrder = new List<Product>();
            var marked = new HashSet<Product>();

            void DepthFirstPopulate(Product start)
            {
                if (!marked.Add(start))
                {
                    return;
                }

                foreach (var item in this.GetRecipesWhichRequireProduct(start).Select(r => r.Output.Product))
                {
                    if (!marked.Contains(item))
                    {
                        DepthFirstPopulate(item);
                    }
                }

                depthFirstOrder.Add(start);
            }

            foreach (var products in this.Recipes.Select(r => r.Output.Product).Distinct())
            {
                DepthFirstPopulate(products);
            }

            var quantity = new Dictionary<Product, long>
            {
                [product] = amountToProduce
            };

            foreach (var item in depthFirstOrder)
            {
                var recipe = this.GetRecipesForProduct(item);
                if (recipe == null)
                {
                    continue;
                }

                var requiredCount = (long)Math.Ceiling((decimal)quantity[item] / recipe.Output.Count);
                foreach (var dependency in recipe.InputProducts)
                {
                    if (quantity.ContainsKey(dependency.Product))
                    {
                        quantity[dependency.Product] += dependency.Count * requiredCount;
                    }
                    else
                    {
                        quantity.Add(dependency.Product, dependency.Count * requiredCount);
                    }
                }
            }

            var ore = Product.Get("ORE");
            return new ProductQuantity(ore, quantity[ore]);
        }

        public Recipe GetRecipesForProduct(Product product)
        {
            return this.Recipes.FirstOrDefault(r => r.Output.Product == product);
        }

        private IEnumerable<Recipe> GetRecipesWhichRequireProduct(Product product)
        {
            return this.Recipes.Where(r => r.InputProducts.Any(p => p.Product == product));
        }
    }
}
