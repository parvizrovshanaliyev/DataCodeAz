using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace AppApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IContactService _service;

        public ContactController(IContactService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        // GET: api/<ContactController>
        [HttpGet]
        public ActionResult<IEnumerable<Contact>> Get()
        {
            return _service.GetAllContacts().ToList();
        }

        // GET api/<ContactController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> Get(int id)
        {
            if (id == 0)
                return NotFound();

            var contact = await _service.GetContactByIdAsync(id);

            if (contact == null)
                return NotFound();

            return contact;
        }

        // POST api/<ContactController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                if (_service.SendMail(contact.Email, contact.Message))
                    await _service.CreateContactAsync(contact);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = contact.ID}, contact);
        }

        // PUT api/<ContactController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Contact contact)
        {
            var contactFromDb = await _service.GetContactByIdAsync(id);

            if (contactFromDb == null)
                return NotFound();

            if (id != contact.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                await _service.UpdateContactAsync(id, contact);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = contactFromDb.ID}, contactFromDb);
        }

        // DELETE api/<ContactController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var contact = await _service.GetContactByIdAsync(id);

            if (contact == null)
                return BadRequest();

            try
            {
                await _service.DeleteContactAsync(contact);
            }

            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}