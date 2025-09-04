using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleLight.Application.DTOs.Person;
using PeopleLight.Application.Interfaces;
using PeopleLight.Domain.Entities;
using PeopleLight.Domain.ValueObjects;

namespace PeopleLight.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly IPersonRepository _repository;
        public PeopleController(IPersonRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var people = await _repository.GetAllAsync();
                return Ok(people);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var person = await _repository.GetByIdAsync(id);
                if (person == null)
                    return NotFound(new { message = $"Pessoa com ID {id} não encontrada." });
                return Ok(person);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(PersonCreateDto dto)
        {
            try
            {
                var person = new Person(dto.Name, new Documents(dto.Document), dto.Email, dto.Phone);
                await _repository.AddAsync(person);
                return CreatedAtAction(nameof(Get), new { id = person.Id }, person);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PersonUpdateDto dto)
        {
            try
            {
                var person = await _repository.GetByIdAsync(id);
                if (person == null)
                    return NotFound(new { message = $"Pessoa com ID {id} não encontrada." });

                person.Update(dto.Name, new Documents(dto.Document), dto.Email, dto.Phone);
                await _repository.UpdateAsync(person);
                return NoContent();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
