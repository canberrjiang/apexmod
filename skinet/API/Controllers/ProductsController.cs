using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Core.Specifications.BaseProductWithTagsAndCategoriesSpecification;
using static Core.Specifications.ProductWithTagsAndCategoriesSpecification;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProductsController : BaseApiController
  {
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPhotoService _photoService;

    public ProductsController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
    {
      _photoService = photoService;
      _unitOfWork = unitOfWork;
      _mapper = mapper;
    }

    // [Cached(600)]
    [HttpGet]
    public async Task<ActionResult<Pagination<BaseProductToReturnDto>>> GetProducts(
      [FromQuery] BaseProductsSpecParams productParams)
    {
      var spec = new BaseProductsWithTagsAndCategoriesSpecification(productParams);
      var countSpec = new BaseProductWithFiltersForCountSpecification(productParams);
      var totalItems = await _unitOfWork.Repository<BaseProduct>().CountAsync(countSpec);
      var products = await _unitOfWork.Repository<BaseProduct>().ListAsync(spec);
      var data = _mapper.Map<IReadOnlyList<BaseProduct>, IReadOnlyList<BaseProductToReturnDto>>(products);
      return Ok(new Pagination<BaseProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
    }

    // [Cached(600)]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {

      var spec = new BaseProductsWithTagsAndCategoriesSpecification(id);
      var baseProduct = await _unitOfWork.Repository<BaseProduct>().GetEntityWithSpec(spec);
      var result = _mapper.Map<BaseProduct, ProductToReturnDto>(baseProduct);

      if (baseProduct.Discriminator == "Product")
      {
        var productSpec = new ProductsWithTagAndCategorySpecification(id);
        var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(productSpec);
        if (product == null) return NotFound(new ApiResponse(404));
        result = _mapper.Map<Product, ProductToReturnDto>(product);
      }


      if (baseProduct == null) return NotFound(new ApiResponse(404));
      return result;
    }

    [HttpGet("tags")]
    public async Task<ActionResult<IReadOnlyList<TagToReturnDto>>> GetProductTags()
    {
      var result = await _unitOfWork.Repository<Tag>().ListAllAsync();
      var tagToReturn = _mapper.Map<IReadOnlyList<Tag>, IReadOnlyList<TagToReturnDto>>(result);
      return Ok(tagToReturn);
    }


    // [HttpPost]
    // [Authorize(Roles = "Admin")]
    // public async Task<ActionResult<ProductToReturnDto>> CreateProduct(ProductCreateDto productToCreate)
    // {
    //   var product = _mapper.Map<ProductCreateDto, Product>(productToCreate);
    //   _unitOfWork.Repository<Product>().Add(product);
    //   var result = await _unitOfWork.Complete();
    //   if (result <= 0) return BadRequest(new ApiResponse(400, "Problem creating products"));
    //   return _mapper.Map<Product, ProductToReturnDto>(product);
    // }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductToReturnDto>> CreateProduct(ProductCreateDto productToCreate)
    {
      ProductToReturnDto productCreated = null;
      var result = 0;
      var tagResult = 0;

      if (productToCreate.Discriminator == "Product")
      {
        var product = _mapper.Map<ProductCreateDto, Product>(productToCreate);
        _unitOfWork.Repository<Product>().Add(product);
        productCreated = _mapper.Map<Product, ProductToReturnDto>(product);
        result = await _unitOfWork.Complete();

        for (int i = 0; i < productToCreate.ProductTagIds.Count; i++)
        {
          ProductTag productTag = new ProductTag()
          {
            ProductId = product.Id,
            TagId = productToCreate.ProductTagIds[i]
          };
          _unitOfWork.Repository<ProductTag>().Add(productTag);
        }
        for (int i = 0; i < productToCreate.ChildProductIds.Count; i++)
        {
          ProductProduct productProduct = new ProductProduct()
          {
            Product = product,
            ChildProductId = productToCreate.ChildProductIds[i]
          };
          _unitOfWork.Repository<ProductProduct>().Add(productProduct);
        }
        tagResult = await _unitOfWork.Complete();
      }
      else if (productToCreate.Discriminator == "ChildProduct")
      {
        var childProduct = _mapper.Map<ProductCreateDto, ChildProduct>(productToCreate);
        _unitOfWork.Repository<ChildProduct>().Add(childProduct);
        productCreated = _mapper.Map<ChildProduct, ProductToReturnDto>(childProduct);
        result = await _unitOfWork.Complete();
        for (int i = 0; i < productToCreate.ProductTagIds.Count; i++)
        {
          ProductTag productTag = new ProductTag()
          {
            ProductId = childProduct.Id,
            TagId = productToCreate.ProductTagIds[i]
          };
          _unitOfWork.Repository<ProductTag>().Add(productTag);
        }
        tagResult = await _unitOfWork.Complete();
      }

      if (result <= 0 || tagResult <= 0) return BadRequest(new ApiResponse(400, "Problem creating products"));
      return Ok(productToCreate);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateProduct(int id, ProductCreateDto productToUpdate)
    {
      ProductToReturnDto productToReturn;
      if (productToUpdate.Discriminator == "Product")
      {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        if (product == null) return BadRequest(new ApiResponse(400, "Product does not exist"));
        _mapper.Map(productToUpdate, product);
        _unitOfWork.Repository<Product>().Update(product);

        // Update ProductTag Table - Delete all producattag relationship by productid and then add new producttag.
        var spec = new ProductTagByProductIdSpecification(id);
        var productTags = await _unitOfWork.Repository<ProductTag>().GetEntitiesWithSpec(spec);
        for (int j = 0; j < productTags.Count; j++)
        {
          _unitOfWork.Repository<ProductTag>().Delete(productTags[j]);
        }

        for (int i = 0; i < productToUpdate.ProductTagIds.Count; i++)
        {
          ProductTag newProductTag = new ProductTag()
          {
            ProductId = id,
            TagId = productToUpdate.ProductTagIds[i]
          };

          _unitOfWork.Repository<ProductTag>().Add(newProductTag);
        }

        // Update ProductProduct Table

        var productProductSpec = new ProductProductByProductIdSpecification(id);
        var productProducts = await _unitOfWork.Repository<ProductProduct>().GetEntitiesWithSpec(productProductSpec);

        for (int j = 0; j < productProducts.Count; j++)
        {
          _unitOfWork.Repository<ProductProduct>().Delete(productProducts[j]);
        }

        for (int i = 0; i < productToUpdate.ChildProductIds.Count; i++)
        {
          ProductProduct newProductProduct = new ProductProduct()
          {
            ProductId = id,
            ChildProductId = productToUpdate.ChildProductIds[i]
          };

          _unitOfWork.Repository<ProductProduct>().Add(newProductProduct);
        }

        productToReturn = _mapper.Map<Product, ProductToReturnDto>(product);
      }
      else if (productToUpdate.Discriminator == "ChildProduct")
      {
        var childProduct = await _unitOfWork.Repository<ChildProduct>().GetByIdAsync(id);
        if (childProduct == null) return BadRequest(new ApiResponse(400, "Product does not exist"));
        _mapper.Map(productToUpdate, childProduct);
        _unitOfWork.Repository<ChildProduct>().Update(childProduct);

        // Update ProductTag Table - Delete all producattag relationship by productid and then add new producttag.
        var spec = new ProductTagByProductIdSpecification(id);
        var productTags = await _unitOfWork.Repository<ProductTag>().GetEntitiesWithSpec(spec);
        for (int j = 0; j < productTags.Count; j++)
        {
          _unitOfWork.Repository<ProductTag>().Delete(productTags[j]);
        }

        for (int i = 0; i < productToUpdate.ProductTagIds.Count; i++)
        {
          ProductTag newProductTag = new ProductTag()
          {
            ProductId = id,
            TagId = productToUpdate.ProductTagIds[i]
          };

          _unitOfWork.Repository<ProductTag>().Add(newProductTag);
        }
        productToReturn = _mapper.Map<ChildProduct, ProductToReturnDto>(childProduct);
      }
      else
      {
        productToReturn = null;
      }

      var result = await _unitOfWork.Complete();
      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem updating products"));
      return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
      var spec = new ProductTagByProductIdSpecification(id);
      var productTags = await _unitOfWork.Repository<ProductTag>().GetEntitiesWithSpec(spec);
      for (int j = 0; j < productTags.Count; j++)
      {
        _unitOfWork.Repository<ProductTag>().Delete(productTags[j]);
      }

      var productProductSpec = new ProductProductByProductIdSpecification(id);
      var productProducts = await _unitOfWork.Repository<ProductProduct>().GetEntitiesWithSpec(productProductSpec);

      for (int j = 0; j < productProducts.Count; j++)
      {
        _unitOfWork.Repository<ProductProduct>().Delete(productProducts[j]);
      }

      var objectRelationshipResult = await _unitOfWork.Complete();
      var product = await _unitOfWork.Repository<BaseProduct>().GetByIdAsync(id);
      if (product.Photos.Count > 0)
      {
        foreach (var photo in product.Photos)
        {
          _photoService.DeleteFromDisk(photo);
        }
      }

      _unitOfWork.Repository<BaseProduct>().Delete(product);
      var result = await _unitOfWork.Complete();
      if (result <= 0 || objectRelationshipResult <= 0) return BadRequest(new ApiResponse(400, "Problem deleting products"));
      return Ok();
    }

    [HttpPut("{id}/photo")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BaseProductToReturnDto>> AddProductPhoto(int id, [FromForm] ProductPhotoDto photoDto)
    {
      var spec = new BaseProductsWithTagsAndCategoriesSpecification(id);
      var product = await _unitOfWork.Repository<BaseProduct>().GetEntityWithSpec(spec);
      // Todo - check nullable product.
      if (photoDto.Photo.Length > 0)
      {
        var photo = await _photoService.SaveToDiskAsync(photoDto.Photo);

        if (photo != null)
        {
          product.AddPhoto(photo.PictureUrl, photo.FileName);

          _unitOfWork.Repository<BaseProduct>().Update(product);

          var result = await _unitOfWork.Complete();

          if (result <= 0) return BadRequest(new ApiResponse(400, "Problem adding photo product"));
        }
        else
        {
          return BadRequest(new ApiResponse(400, "problem saving photo to disk"));
        }
      }

      return _mapper.Map<BaseProduct, BaseProductToReturnDto>(product);
    }

    [HttpDelete("{id}/photo/{photoId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProductPhoto(int id, int photoId)
    {
      var spec = new BaseProductsWithTagsAndCategoriesSpecification(id);
      var product = await _unitOfWork.Repository<BaseProduct>().GetEntityWithSpec(spec);

      var photo = product.Photos.SingleOrDefault(x => x.Id == photoId);

      if (photo != null)
      {
        if (photo.IsMain) return BadRequest(new ApiResponse(400, "You cannot delete the main photo"));

        _photoService.DeleteFromDisk(photo);
      }
      else
      {
        return BadRequest(new ApiResponse(400, "Photo does not exist"));
      }

      product.RemovePhoto(photoId);
      _unitOfWork.Repository<BaseProduct>().Update(product);
      var result = await _unitOfWork.Complete();
      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem adding photos"));
      return Ok();
    }

    [HttpPost("{id}/photo/{photoId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BaseProductToReturnDto>> SetMainPhoto(int id, int photoId)
    {
      var spec = new BaseProductsWithTagsAndCategoriesSpecification(id);
      var product = await _unitOfWork.Repository<BaseProduct>().GetEntityWithSpec(spec);

      if (product.Photos.All(x => x.Id != photoId)) return NotFound();

      product.SetMainPhoto(photoId);

      _unitOfWork.Repository<BaseProduct>().Update(product);

      var result = await _unitOfWork.Complete();

      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem adding photo product"));

      return _mapper.Map<BaseProduct, BaseProductToReturnDto>(product);
    }
  }
}