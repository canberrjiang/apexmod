using System.Linq;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Entities.OrderAggregate;

namespace API.Helpers
{
  public class MappingProfiles : Profile
  {
    public MappingProfiles()
    {
      CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductCategory, s => s.MapFrom(s => s.ProductCategory.Name))
                .ForMember(d => d.Tags, o => o.MapFrom(s => s.ProductTag.Select(t => t.Tag).ToList()))
                .ForMember(d => d.ChildProducts, o => o.MapFrom(s => s.ChildProducts))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductUrlResolver>());
      CreateMap<BaseProduct, BaseProductToReturnDto>()
                .ForMember(d => d.ProductCategory, s => s.MapFrom(s => s.ProductCategory.Name))
                .ForMember(d => d.Tags, o => o.MapFrom(s => s.ProductTag.Select(t => t.Tag).ToList()))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<BaseProductUrlResolver>());
      CreateMap<BaseProduct, ProductToReturnDto>()
                .ForMember(d => d.ProductCategory, s => s.MapFrom(s => s.ProductCategory.Name))
                .ForMember(d => d.Tags, o => o.MapFrom(s => s.ProductTag.Select(t => t.Tag).ToList()))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<BaseProducToProductUrlResolver>());
      CreateMap<ChildProduct, ProductToReturnDto>()
                .ForMember(d => d.ProductCategory, s => s.MapFrom(s => s.ProductCategory.Name))
                .ForMember(d => d.Tags, o => o.MapFrom(s => s.ProductTag.Select(t => t.Tag).ToList()))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ChildProducToProductUrlResolver>());
      CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();
      CreateMap<CustomerBasketDto, CustomerBasket>();
      CreateMap<BasketItemDto, BasketItem>();
      CreateMap<AddressDto, Core.Entities.OrderAggregate.Address>();
      CreateMap<Order, OrderToReturnDto>()
          .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
          .ForMember(d => d.ShippingPrice, o => o.MapFrom(s => s.DeliveryMethod.Price));
      CreateMap<OrderItem, OrderItemDto>()
          .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ItemOrdered.ProductItemId))
          .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ItemOrdered.ProductName))
          .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.ItemOrdered.PictureUrl))
          .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemUrlResolver>());
      CreateMap<ProductCreateDto, Product>()
          .ForMember(d => d.ChildProducts, o => o.MapFrom(s => s.ChildProducts))
          .ForMember(d => d.ProductTag, o => o.MapFrom(s => s.ProductTags));
      CreateMap<ProductCreateDto, ChildProduct>();
      CreateMap<Photo, PhotoToReturnDto>()
                .ForMember(d => d.PictureUrl, o => o.MapFrom<PhotoUrlResolver>());
      CreateMap<Tag, TagToReturnDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
      CreateMap<ChildProduct, ChildProductToReturnDto>()
        .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
        .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
        .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
        .ForMember(d => d.ProductCategory, o => o.MapFrom(s => s.ProductCategory.Name))
        .ForMember(d => d.PictureUrl, o => o.MapFrom<ChildProductPhotoUrlResolver>())
        .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
        .ReverseMap();
      CreateMap<ProductTagToCreate, ProductTag>();
      CreateMap<ProductCategory, ProductCategoryToReturnDto>();
      CreateMap<ChildProduct, ChildProductToCreate>();
      CreateMap<ChildProductToCreate, ProductProduct>();
      CreateMap<ProductTagToCreate, ProductTag>();
      CreateMap<ChildProductsToReturnDto, ProductProduct>().ReverseMap();
    }
  }
}