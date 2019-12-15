namespace Day14
{
    using System.Collections.Generic;

    public class Product
    {
        private static readonly Dictionary<string, Product> ExistingProducts;

        static Product()
        {
            ExistingProducts = new Dictionary<string, Product>();
        }

        private Product(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public static bool Exists(string name)
        {
            return ExistingProducts.ContainsKey(name);
        }

        public static Product Get(string name)
        {
            if (!ExistingProducts.TryGetValue(name, out var product))
            {
                product = new Product(name);
                ExistingProducts[name] = product;
            }

            return product;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
