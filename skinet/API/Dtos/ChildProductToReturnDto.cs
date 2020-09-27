using System.Collections.Generic;
using Core.Entities;

namespace API.Dtos
{
  public class ChildProductToReturnDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Information { get; set; }
    public string Description { get; set; }
    public string ProductCategory { get; set; }
    public decimal Price { get; set; }
    public string PictureUrl { get; set; }
    public bool IsPublished { get; set; }
  }
}