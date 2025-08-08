using Microsoft.AspNetCore.Mvc;
using PhoneDirectory.ContactService.Dtos;
using PhoneDirectory.ContactService.Mappings;
using PhoneDirectory.ContactService.Services.PersonServices;

namespace PhoneDirectory.ContactService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonCreateRequestDto dto)
        {
            var person = dto.ToEntity();
            await _personService.AddAsync(person);

            var response = person.ToResponse();
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonResponseDto>>> GetAll()
        {
            var people = await _personService.GetAllAsync();
            var result = people.Select(p => p.ToResponse()).ToList();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PersonResponseDto>> GetById(Guid id)
        {
            var person = await _personService.GetByIdAsync(id);
            if (person is null) return NotFound();

            return Ok(person.ToResponse());
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _personService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
