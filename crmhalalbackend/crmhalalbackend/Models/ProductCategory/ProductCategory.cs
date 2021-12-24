namespace CRMHalalBackEnd.Models.ProductCategory
{
    public class ProductCategory
    {
        public int ProductCategoryId { get; set; }
        public int ParentProductCategoryId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{nameof(ProductCategoryId)}: {ProductCategoryId}, {nameof(ParentProductCategoryId)}: {ParentProductCategoryId}, {nameof(Name)}: {Name}";
        }
    }
}