namespace Infrastructure.Data
{
  public class ProductSeedModel
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PictureUrl { get; set; }
    public int ProductCategoryId { get; set; }
  }
}