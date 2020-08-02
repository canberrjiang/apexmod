namespace API.Dtos
{
  public class ProductComponentToReturnDto
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal PPrice { get; set; }
    public decimal TPrice { get; set; }
  }
}