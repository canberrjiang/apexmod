using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Core.Specifications.AllBaseProductWithTagsAndCategoriesSpecification;
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

    [Cached(600)]
    [HttpGet]
    public async Task<ActionResult<Pagination<BaseProductToReturnDto>>> GetProducts(
      [FromQuery] BaseProductsSpecParams productParams)
    {
      var email = HttpContext.User.RetrieveEmailFromPrincipal();
      var spec = new BaseProductsWithTagsAndCategoriesSpecification(productParams);
      var countSpec = new BaseProductWithFiltersForCountSpecification(productParams);
      var totalItems = await _unitOfWork.Repository<BaseProduct>().CountAsync(countSpec);
      var products = await _unitOfWork.Repository<BaseProduct>().ListAsync(spec);
      var data = _mapper.Map<IReadOnlyList<BaseProduct>, IReadOnlyList<BaseProductToReturnDto>>(products);
      return Ok(new Pagination<BaseProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
    }


    [HttpGet("productcategory/{productcategoryid}")]
    public async Task<ActionResult<IReadOnlyList<BaseProductToReturnDto>>> GetProductsByCategory(int productcategoryid)
    {
      var spec = new AllBaseProductWithDiscriminatorAndCategory(productcategoryid);
      var products = await _unitOfWork.Repository<BaseProduct>().ListAsync(spec);
      var data = _mapper.Map<IReadOnlyList<BaseProduct>, IReadOnlyList<BaseProductToReturnDto>>(products);
      return Ok(data);
    }

    // [HttpGet("discriminator/{discriminator}")]
    // public async Task<ActionResult<IReadOnlyList<BaseProductToReturnDto>>> GetProductsByDiscriminator(string discriminator)
    // {
    //   var spec = new BaseProductWithDiscriminatorAndCategory(discriminator);
    //   var products = await _unitOfWork.Repository<BaseProduct>().ListAsync(spec);
    //   var data = _mapper.Map<IReadOnlyList<BaseProduct>, IReadOnlyList<BaseProductToReturnDto>>(products);
    //   return Ok(data);
    // }

    [HttpGet("publishstatus/{publishstatus}")]
    public async Task<ActionResult<IReadOnlyList<BaseProductToReturnDto>>> GetUnpublishedProducts(bool publishstatus)
    {
      var spec = new BaseProductByPublishStatusSpecification(publishstatus);
      var products = await _unitOfWork.Repository<BaseProduct>().ListAsync(spec);
      var data = _mapper.Map<IReadOnlyList<BaseProduct>, IReadOnlyList<BaseProductToReturnDto>>(products);
      return Ok(data);
    }

    [Cached(600)]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {

      var spec = new AllBaseProductsWithTagsAndCategoriesSpecification(id);
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
    public async Task<ActionResult<IReadOnlyList<TagToReturnDto>>> GetProductTag()
    {
      var result = await _unitOfWork.Repository<Tag>().ListAllAsync();
      var tagToReturn = _mapper.Map<IReadOnlyList<Tag>, IReadOnlyList<TagToReturnDto>>(result);
      return Ok(tagToReturn);
    }

    // Only return tags that attached to products
    // [HttpGet("tags")]
    // public async Task<ActionResult<IReadOnlyList<TagToReturnDto>>> GetAdminProductTags()
    // {
    //   // var result = await _unitOfWork.Repository<Tag>().ListAllAsync();
    //   // var tagToReturn = _mapper.Map<IReadOnlyList<Tag>, IReadOnlyList<TagToReturnDto>>(result);
    //   var spec = new ProductTagWithProductAndTagSpecification();
    //   var productTags = await _unitOfWork.Repository<ProductTag>().GetEntitiesWithSpec(spec);
    //   var tags = productTags.Select(x => x.Tag).Distinct().ToList();
    //   var tagToReturn = _mapper.Map<IReadOnlyList<Tag>, IReadOnlyList<TagToReturnDto>>(tags);
    //   return Ok(tagToReturn);
    // }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<ProductCategoryToReturnDto>>> GetProductCategories()
    {
      var result = await _unitOfWork.Repository<ProductCategory>().ListAllAsync();
      var categoryToReturn = _mapper.Map<IReadOnlyList<ProductCategory>, IReadOnlyList<ProductCategoryToReturnDto>>(result);
      return Ok(categoryToReturn);
    }

    // For Admin to view all products
    [Authorize(Roles = "Admin")]
    [HttpGet("admin/products")]
    public async Task<ActionResult<BaseProductToReturnDto>> GetAdminProducts()
    {
      // var email = HttpContext.User.RetrieveEmailFromPrincipal();
      var spec = new AllBaseProductsWithTagsAndCategoriesSpecification();
      // var countSpec = new AllBaseProductWithFiltersForCountSpecification(productParams);
      // var totalItems = await _unitOfWork.Repository<BaseProduct>().CountAsync(countSpec);
      var products = await _unitOfWork.Repository<BaseProduct>().GetEntitiesWithSpec(spec);
      var data = _mapper.Map<IReadOnlyList<BaseProduct>, IReadOnlyList<BaseProductToReturnDto>>(products);
      return Ok(data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> CreateProduct(ProductCreateDto productToCreate)
    {
      var result = 0;
      var tagResult = 0;
      var productCreatedId = 0;

      if (productToCreate.Discriminator == "Product")
      {
        var product = _mapper.Map<ProductCreateDto, Product>(productToCreate);
        _unitOfWork.Repository<Product>().Add(product);
        result = await _unitOfWork.Complete();
        productCreatedId = product.Id;
      }
      else if (productToCreate.Discriminator == "ChildProduct")
      {
        var childProduct = _mapper.Map<ProductCreateDto, ChildProduct>(productToCreate);
        _unitOfWork.Repository<ChildProduct>().Add(childProduct);
        result = await _unitOfWork.Complete();
        productCreatedId = childProduct.Id;
      }

      if (productToCreate.ProductTagIds.Count > 0)
      {
        for (int i = 0; i < productToCreate.ProductTagIds.Count; i++)
        {
          ProductTag productTag = new ProductTag()
          {
            ProductId = productCreatedId,
            TagId = productToCreate.ProductTagIds[i]
          };

          _unitOfWork.Repository<ProductTag>().Add(productTag);
          tagResult = await _unitOfWork.Complete();
        }
      }

      if (result <= 0 || tagResult <= 0) return BadRequest(new ApiResponse(400, "Problem creating products"));
      return Ok(productCreatedId);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductToReturnDto>> UpdateProduct(int id, ProductCreateDto productToUpdate)
    {
      ProductToReturnDto productToReturn;
      var updatedProductId = 0;
      if (productToUpdate.Discriminator == "Product")
      {
        // Update ProductTag Table - Delete all producattag relationship by productid and then add new producttag.
        var spec = new ProductTagByProductIdSpecification(id);
        var productTags = await _unitOfWork.Repository<ProductTag>().GetEntitiesWithSpec(spec);
        for (int j = 0; j < productTags.Count; j++)
        {
          _unitOfWork.Repository<ProductTag>().Delete(productTags[j]);
        }

        // Update ProductProduct Table
        var productProductSpec = new ProductProductByProductIdSpecification(id);
        var productProducts = await _unitOfWork.Repository<ProductProduct>().GetEntitiesWithSpec(productProductSpec);

        for (int j = 0; j < productProducts.Count; j++)
        {
          _unitOfWork.Repository<ProductProduct>().Delete(productProducts[j]);
        }

        var productSpec = new ProductsWithTagAndCategorySpecification(id);
        var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(productSpec);
        if (product == null) return BadRequest(new ApiResponse(400, "Product does not exist"));
        _mapper.Map(productToUpdate, product);
        _unitOfWork.Repository<Product>().Update(product);

        productToReturn = _mapper.Map<Product, ProductToReturnDto>(product);
        updatedProductId = product.Id;
      }
      else if (productToUpdate.Discriminator == "ChildProduct")
      {
        // Update ProductTag Table - Delete all producattag relationship by productid and then add new producttag.
        var spec = new ProductTagByProductIdSpecification(id);
        var productTags = await _unitOfWork.Repository<ProductTag>().GetEntitiesWithSpec(spec);
        for (int j = 0; j < productTags.Count; j++)
        {
          _unitOfWork.Repository<ProductTag>().Delete(productTags[j]);
        }

        var childProductSpec = new ChildProductsWithTagAndCategoriesSpecification(id);
        var childProduct = await _unitOfWork.Repository<ChildProduct>().GetEntityWithSpec(childProductSpec);
        if (childProduct == null) return BadRequest(new ApiResponse(400, "Product does not exist"));
        _mapper.Map(productToUpdate, childProduct);
        _unitOfWork.Repository<ChildProduct>().Update(childProduct);

        productToReturn = _mapper.Map<ChildProduct, ProductToReturnDto>(childProduct);
        updatedProductId = childProduct.Id;
      }
      else
      {
        return BadRequest(new ApiResponse(400, "No such a product"));
      }

      // Manually update ProductTag relationship table
      if (productToUpdate.ProductTagIds.Count > 0 && updatedProductId != 0)
      {
        var spec = new ProductTagByProductIdSpecification(updatedProductId);
        var productTagEntities = await _unitOfWork.Repository<ProductTag>().GetEntitiesWithSpec(spec);

        for (int i = 0; i < productToUpdate.ProductTagIds.Count; i++)
        {
          ProductTag productTag = new ProductTag()
          {
            ProductId = updatedProductId,
            TagId = productToUpdate.ProductTagIds[i]
          };

          _unitOfWork.Repository<ProductTag>().Add(productTag);
          // tagResult = await _unitOfWork.Complete();
          // if (tagResult <= 0) return BadRequest(new ApiResponse(400, "error updating products"));
        }
      }
      var result = await _unitOfWork.Complete();
      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem updating products"));
      return Ok(productToReturn);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
      var spec = new ProductTagByProductIdSpecification(id);
      var productTags = await _unitOfWork.Repository<ProductTag>().GetEntitiesWithSpec(spec);
      if (productTags.Count > 0)
      {
        for (int j = 0; j < productTags.Count; j++)
        {
          _unitOfWork.Repository<ProductTag>().Delete(productTags[j]);
        }
      }


      var productProductSpec = new ProductProductByProductIdSpecification(id);
      var productProducts = await _unitOfWork.Repository<ProductProduct>().GetEntitiesWithSpec(productProductSpec);

      if (productProducts.Count > 0)
      {
        for (int j = 0; j < productProducts.Count; j++)
        {
          _unitOfWork.Repository<ProductProduct>().Delete(productProducts[j]);
        }
      }

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
      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem deleting products"));
      return Ok();
    }

    // Process images uploaded from client via richtext editor and return a photo url if successful.
    [HttpPost("richtextphoto")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> AddRichtextPhoto([FromForm] ProductPhotoDto photoDto)
    {
      if (photoDto.Photo.Length > 0)
      {
        var photo = await _photoService.SaveToDiskAsync(photoDto.Photo);
        if (photo == null) return BadRequest(new ApiResponse(400, "problem saving photo to disk"));
        return Ok(photo.PictureUrl);
      }

      return BadRequest(new ApiResponse(400, "This is not a valid photo."));
    }


    [HttpPut("{id}/photo")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BaseProductToReturnDto>> AddProductPhoto(int id, [FromForm] ProductPhotoDto photoDto)
    {
      var spec = new AllBaseProductsWithTagsAndCategoriesSpecification(id);
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
      var spec = new AllBaseProductsWithTagsAndCategoriesSpecification(id);
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
      var spec = new AllBaseProductsWithTagsAndCategoriesSpecification(id);
      var product = await _unitOfWork.Repository<BaseProduct>().GetEntityWithSpec(spec);

      if (product.Photos.All(x => x.Id != photoId)) return NotFound();

      product.SetMainPhoto(photoId);


      _unitOfWork.Repository<BaseProduct>().Update(product);

      var result = await _unitOfWork.Complete();

      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem setting main photo"));

      return _mapper.Map<BaseProduct, BaseProductToReturnDto>(product);
    }
  }
}