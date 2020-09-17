using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppApi.Models;
using DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using static AppApi.Extensions.IFormFileExtension;

namespace AppApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        // GET: api/<CustomerController>
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> Get()
        {
            return _service.GetAllCustumers().ToList();
        }

        // GET api/<CustomerController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> Get(int id)
        {
            if (id == 0)
                return NotFound();

            var Customer = await _service.GetCustomerByIdAsync(id);

            if (Customer == null)
                return NotFound();

            return Customer;
        }

        // POST api/<CustomerController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CustomerModel customerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (customerModel.Photo == null) return BadRequest();

            if (!customerModel.Photo.IsImage()) return BadRequest();

            if (!customerModel.Photo.LessThan(5)) return BadRequest();

            var custumer = new Customer
            {
                Name = customerModel.Name,
                Description = customerModel.Description
            };

            try
            {
                custumer.Image = await customerModel.Photo.SaveAsync(_env.WebRootPath, "images", "customer");
                await _service.CreateCustomerAsync(custumer);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = custumer.ID}, custumer);
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CustomerModel customerModel)
        {
            var customerFromDb = await _service.GetCustomerByIdAsync(id);

            if (customerFromDb == null)
                return NotFound();

            if (id != customerModel.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            customerFromDb.Name = customerModel.Name;
            customerFromDb.Description = customerModel.Description;

            try
            {
                if (customerModel.Photo != null)
                {
                    if (!customerModel.Photo.IsImage()) return BadRequest();

                    if (!customerModel.Photo.LessThan(6)) return BadRequest();

                    RemoveFile(_env.WebRootPath, "images", "customer", customerFromDb.Image);

                    customerFromDb.Image = await customerModel.Photo.SaveAsync(_env.WebRootPath, "images", "customer");
                }

                await _service.UpdateCustomerAsync(id, customerFromDb);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = customerFromDb.ID}, customerFromDb);
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var customer = await _service.GetCustomerByIdAsync(id);

            if (customer == null)
                return BadRequest();

            try
            {
                RemoveFile(_env.WebRootPath, "images", "customer", customer.Image);
                await _service.DeleteCustomerAsync(customer);
            }

            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}