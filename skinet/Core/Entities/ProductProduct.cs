namespace Core.Entities
{
  public class ProductProduct : BaseEntity
  {
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public ChildProduct ChildProduct { get; set; }
    public int ChildProductId { get; set; }
    public bool IsDefault { get; set; }
  }
}