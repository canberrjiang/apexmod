namespace Core.Entities
{
  public class ProductTag : BaseEntity
  {
    public int ProductId { get; set; }
    public int TagId { get; set; }

    public BaseProduct BaseProduct { get; set; }
    public Tag Tag { get; set; }
  }
}