using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Core.Specifications.ProductWithTypesAndBrandsSpecification;

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
    public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
      [FromQuery] ProductsSpecParams productParams)
    {
      var spec = new ProductsWithTypesAndBrandsSpecification(productParams);
      var countSpec = new ProductWithFiltersForCountSpecification(productParams);
      var totalItems = await _unitOfWork.Repository<Product>().CountAsync(countSpec);
      var products = await _unitOfWork.Repository<Product>().ListAsync(spec);
      var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
      return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
    }

    [Cached(600)]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {
      var spec = new ProductsWithTypesAndBrandsSpecification(id);
      var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);
      if (product == null) return NotFound(new ApiResponse(404));
      return _mapper.Map<Product, ProductToReturnDto>(product);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
    {
      var result = await _unitOfWork.Repository<Product>().ListAllAsync();
      return Ok(result);
    }

    [Cached(1000)]
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
    {
      var result = await _unitOfWork.Repository<Product>().ListAllAsync();
      return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductToReturnDto>> CreateProduct(ProductCreateDto productToCreate)
    {
      var product = _mapper.Map<ProductCreateDto, Product>(productToCreate);
      _unitOfWork.Repository<Product>().Add(product);
      var result = await _unitOfWork.Complete();
      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem creating products"));
      return _mapper.Map<Product, ProductToReturnDto>(product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductToReturnDto>> UpdateProduct(int id, ProductCreateDto productToUpdate)
    {
      var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
      _mapper.Map(productToUpdate, product);
      _unitOfWork.Repository<Product>().Update(product);
      var result = await _unitOfWork.Complete();
      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem updating products"));
      return _mapper.Map<Product, ProductToReturnDto>(product);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
      var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
      foreach (var photo in product.Photos)
      {
        _photoService.DeleteFromDisk(photo);
      }
      _unitOfWork.Repository<Product>().Delete(product);
      var result = await _unitOfWork.Complete();
      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem deleting products"));
      return Ok();
    }

    [HttpPut("{id}/photo")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductToReturnDto>> AddProductPhoto(int id, [FromForm] ProductPhotoDto photoDto)
    {
      var spec = new ProductsWithTypesAndBrandsSpecification(id);
      var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);
      // Todo - check nullable product.
      if (photoDto.Photo.Length > 0)
      {
        var photo = await _photoService.SaveToDiskAsync(photoDto.Photo);

        if (photo != null)
        {
          product.AddPhoto(photo.PictureUrl, photo.FileName);

          _unitOfWork.Repository<Product>().Update(product);

          var result = await _unitOfWork.Complete();

          if (result <= 0) return BadRequest(new ApiResponse(400, "Problem adding photo product"));
        }
        else
        {
          return BadRequest(new ApiResponse(400, "problem saving photo to disk"));
        }
      }

      return _mapper.Map<Product, ProductToReturnDto>(product);
    }

    [HttpDelete("{id}/photo/{photoId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProductPhoto(int id, int photoId)
    {
      var spec = new ProductsWithTypesAndBrandsSpecification(id);
      var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);

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
      _unitOfWork.Repository<Product>().Update(product);
      var result = await _unitOfWork.Complete();
      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem adding photos"));
      return Ok();
    }
  }
}