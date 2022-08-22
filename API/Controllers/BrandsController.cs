using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BrandsController : BaseApiController
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BrandsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAllBrands()
        {
            var result = await this._unitOfWork.Brands.GetAllAsync();
            var brandDtos = this._mapper.Map<IEnumerable<BrandDto>>(result);
            return Ok(brandDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDto>> GetBrand(int id)
        {
            var result = await this._unitOfWork.Brands.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            var brandDto = this._mapper.Map<BrandDto>(result);
            return Ok(brandDto);
        }

        [HttpPost]
        public async Task<ActionResult> AddBrand(BrandDto brandDto)
        {
            var newBrand = this._mapper.Map<Brand>(brandDto);
            this._unitOfWork.Brands.Add(newBrand);
            await this._unitOfWork.SaveAsync();

            return Created(nameof(AddBrand), brandDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBrand(int id, [FromBody] BrandDto brandDto)
        {
            var brand = await this._unitOfWork.Brands.GetByIdAsync(id);
            if (brand == null)
                return NotFound();

            brand.Name = brandDto.Name;
            this._unitOfWork.Brands.Update(brand);
            await this._unitOfWork.SaveAsync();

            return Ok(brandDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBrand(int id)
        {
            var result = await this._unitOfWork.Brands.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            this._unitOfWork.Brands.Remove(result);
            await this._unitOfWork.SaveAsync();

            return NoContent();
        }

        [HttpGet("search/{searchText}")]
        public async Task<ActionResult<IEnumerable<BrandDto>>> SearchBrandsAsync(string searchText)
        {
            var result = this._unitOfWork.Brands.Find(b => b.Name.ToLower().Contains(searchText.ToLower()));
            return Ok(this._mapper.Map<IEnumerable<BrandDto>>(result));
        }

    }
}
