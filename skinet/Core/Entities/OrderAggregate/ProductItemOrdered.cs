namespace Core.Entities.OrderAggregate
{
  public class ProductItemOrdered
  {
    public ProductItemOrdered()
    {
    }

    public ProductItemOrdered(int productItemId, string productName, string productDescription, string pictureUrl)
    {
      ProductItemId = productItemId;
      ProductName = productName;
      ProductDescription = productDescription;
      PictureUrl = pictureUrl;
    }

    public int ProductItemId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public string PictureUrl { get; set; }
  }
}