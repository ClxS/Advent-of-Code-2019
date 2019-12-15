namespace Day14
{
    using System.Collections.Generic;
    using System.Linq;

    public class Recipe
    {
        public Recipe(ProductQuantity[] inputProducts, ProductQuantity outputProducts)
        {
            this.InputProducts = inputProducts;
            this.Output = outputProducts;
        }

        public ProductQuantity[] InputProducts { get; }

        public ProductQuantity Output { get; }

        public static Recipe Create(string recipe)
        {
            IEnumerable<ProductQuantity> CollectComponents(string value)
            {
                var entries = value.Split(',');
                return entries.Select(e =>
                {
                    e = e.Trim();
                    var element = e.Split(' ');
                    return new ProductQuantity(Product.Get(element[1]), int.Parse((string)element[0]));
                });
            }

            var components = recipe.Split("=>");
            var inputs = CollectComponents(components[0]);
            var outputs = CollectComponents(components[1]);

            // Only support one output for now. Can change if part 2 needs it.
            return new Recipe(inputs.ToArray(), outputs.First());
        }
    }
}
