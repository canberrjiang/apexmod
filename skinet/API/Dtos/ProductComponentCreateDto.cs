namespace API.Dtos
{
  public class ProductComponentCreateDto
  {
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal PPrice { get; set; }
    public decimal TPrice { get; set; }
    public int ProductId { get; set; }
  }
}