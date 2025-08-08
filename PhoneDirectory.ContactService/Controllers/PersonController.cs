using Microsoft.AspNetCore.Mvc;
using PhoneDirectory.ContactService.Dtos;
using PhoneDirectory.ContactService.Mappings;
using PhoneDirectory.ContactService.Services.ContactInfoServices;
using PhoneDirectory.ContactService.Services.PersonServices;

namespace PhoneDirectory.ContactService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly IContactInfoService _contactInfoService;
        public PersonController(IPersonService personService, IContactInfoService contactInfoService)
        {
            _personService = personService;
            _contactInfoService = contactInfoService;
        }

        #region PERSON OPERATIONS


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

        [HttpGet("stats")]
        public async Task<IActionResult> Stats([FromQuery] string location)
        {
            var (personCount, phoneCount) = await _personService.GetStatsByLocationAsync(location);
            return Ok(new { location, personCount, phoneCount });
        }


        #endregion

        #region CONTACTINFO OPERATIONS


        [HttpPost("{id:guid}/contact-info")]
        public async Task<IActionResult> AddContactInfo(Guid id, [FromBody] ContactInfoRequestDto dto)
        {
            var entity = dto.ToEntity();
            var added = await _contactInfoService.AddContactInfoAsync(id, entity);
            if (added is null) return NotFound();
            return CreatedAtAction(nameof(GetById), new { id = id }, null);
        }

        [HttpDelete("{id:guid}/contact-info/{contactId:guid}")]
        public async Task<IActionResult> RemoveContactInfo(Guid id, Guid contactId)
        {
            var ok = await _contactInfoService.RemoveContactInfoAsync(id, contactId);
            if (!ok) return NotFound();
            return NoContent();
        }


        #endregion
    }
}
