namespace Day14
{
    public struct ProductQuantity
    {
        public ProductQuantity(Product product, long count)
        {
            this.Product = product;
            this.Count = count;
        }

        public long Count { get; }

        public Product Product { get; }

        public override string ToString()
        {
            return $"{this.Count}x {this.Product.Name}";
        }
    }
}
