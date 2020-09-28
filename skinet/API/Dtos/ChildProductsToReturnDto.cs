namespace API.Dtos
{
  public class ChildProductsToReturnDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Information { get; set; }
    public string Description { get; set; }
    public string ProductCategory { get; set; }
    public decimal Price { get; set; }
    public string PictureUrl { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDefault { get; set; }
  }
}