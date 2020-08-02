namespace Core.Entities
{
  public class ComponentPhoto : BasePhoto
  {
    public ProductComponent ProductComponent { get; set; }
    public int ProductComponentId { get; set; }
  }
}