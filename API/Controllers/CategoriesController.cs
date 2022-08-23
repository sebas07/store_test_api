using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CategoriesController : BaseApiController
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var result = await this._unitOfWork.Categories.GetAllAsync();

            var dtoResult = this._mapper.Map<IEnumerable<CategoryDto>>(result);

            return Ok(dtoResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var result = await this._unitOfWork.Categories.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            var dtoResult = this._mapper.Map<CategoryDto>(result);

            return Ok(dtoResult);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> AddCategory(CategoryDto categoryDto)
        {
            var newCategory = this._mapper.Map<Category>(categoryDto);
            this._unitOfWork.Categories.Add(newCategory);
            await this._unitOfWork.SaveAsync();

            return Ok(categoryDto);
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            var tmpCategory = await this._unitOfWork.Categories.GetByIdAsync(id);
            if (tmpCategory == null)
                return NotFound();

            tmpCategory.Name = categoryDto.Name;

            this._unitOfWork.Categories.Update(tmpCategory);
            await this._unitOfWork.SaveAsync();

            return Ok(categoryDto);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await this._unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            this._unitOfWork.Categories.Remove(category);
            await this._unitOfWork.SaveAsync();

            return NoContent();
        }

        [HttpGet("find/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> SearchCategory(string searchTerm)
        {
            var result = this._unitOfWork.Categories.Find(c => c.Name.ToLower().Contains(searchTerm.ToLower()));
            return Ok(result);
        }

    }
}
