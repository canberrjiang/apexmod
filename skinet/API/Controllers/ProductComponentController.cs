using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Core.Specifications.ProductComponentWithPhotoSpecification;

namespace API.Controllers
{
  public class ProductComponentController : BaseApiController
  {
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPhotoService _photoService;
    private readonly IProductComponent _productComponent;

    public ProductComponentController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService, IProductComponent productComponent)
    {
      _productComponent = productComponent;
      _photoService = photoService;
      _unitOfWork = unitOfWork;
      _mapper = mapper;
    }

    [HttpPut("{id}/componentphoto")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductComponentToReturnDto>> AddComponentPhoto(int id, [FromForm] ProductPhotoDto componentPhotoDto)
    {
      var productComponent = await _unitOfWork.Repository<ProductComponent>().GetByIdAsync(id);

      if (componentPhotoDto.Photo.Length > 0)
      {
        var photo = await _photoService.SaveComponentPhotoToDiskAsync(componentPhotoDto.Photo);
        if (photo != null)
        {
          productComponent.AddComponentPhoto(photo.PictureUrl, photo.FileName);
          _unitOfWork.Repository<ProductComponent>().Update(productComponent);
          var result = await _unitOfWork.Complete();

          if (result <= 0) return BadRequest(new ApiResponse(400, "Problem adding photo product"));
        }
        else
        {
          return BadRequest(new ApiResponse(400, "problem saving photo to disk"));
        }
      }
      return _mapper.Map<ProductComponent, ProductComponentToReturnDto>(productComponent);
    }

    [HttpDelete("{id}/componentphoto/{photoId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteComponentPhoto(int id, int photoId)
    {
      var spec = new ProductComponentsWithPhotoSpecification(id);

      var productComponent = await _unitOfWork.Repository<ProductComponent>().GetEntityWithSpec(spec);
      var photo = productComponent.Photo.Id == photoId ? productComponent.Photo : null;
      if (photo != null)
      {
        _photoService.DeleteComponentPhotoFromDisk(photo);
      }
      else
      {
        return BadRequest(new ApiResponse(400, "Photo does not exist"));
      }

      productComponent.RemoveComponentPhoto(photoId);
      _unitOfWork.Repository<ProductComponent>().Update(productComponent);
      var result = await _unitOfWork.Complete();
      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem deleting photos"));
      return Ok();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductComponentToReturnDto>> UpdateProductcomponents(int id, ProductComponentCreateDto newComponent)
    {
      var component = await _unitOfWork.Repository<ProductComponent>().GetByIdAsync(id);
      _mapper.Map(newComponent, component);
      _unitOfWork.Repository<ProductComponent>().Update(component);
      var result = await _unitOfWork.Complete();
      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem updating products component"));
      return _mapper.Map<ProductComponent, ProductComponentToReturnDto>(component);
    }
  }
}