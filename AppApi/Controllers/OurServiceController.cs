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
    public class OurServiceController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IOurServiceService _service;

        public OurServiceController(IOurServiceService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        // GET: api/<OurServiceController>
        [HttpGet]
        public ActionResult<IEnumerable<OurService>> Get()
        {
            return _service.GetAllOurServices().ToList();
        }

        // GET api/<OurServiceController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OurService>> Get(int id)
        {
            if (id == 0)
                return NotFound();

            var ourService = await _service.GetOurServiceByIdAsync(id);

            if (ourService == null)
                return NotFound();

            return ourService;
        }

        // POST api/<BannerController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ServiceModel serviceModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (serviceModel.Photo == null) return BadRequest();

            if (!serviceModel.Photo.IsImage()) return BadRequest();

            if (!serviceModel.Photo.LessThan(5)) return BadRequest();

            var ourService = new OurService
            {
                Title = serviceModel.Title,
                Description = serviceModel.Description
            };

            try
            {
                ourService.Image = await serviceModel.Photo.SaveAsync(_env.WebRootPath, "images", "service");
                await _service.CreateOurServiceAsync(ourService);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = ourService.ID}, ourService);
        }

        // PUT api/<OurServiceController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ServiceModel serviceModel)
        {
            var ourServiceFromDb = await _service.GetOurServiceByIdAsync(id);

            if (ourServiceFromDb == null)
                return NotFound();

            if (id != serviceModel.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            ourServiceFromDb.Title = serviceModel.Title;
            ourServiceFromDb.Description = serviceModel.Description;

            try
            {
                if (serviceModel.Photo != null)
                {
                    if (!serviceModel.Photo.IsImage()) return BadRequest();

                    if (!serviceModel.Photo.LessThan(6)) return BadRequest();

                    RemoveFile(_env.WebRootPath, "images", "service", ourServiceFromDb.Image);

                    ourServiceFromDb.Image = await serviceModel.Photo.SaveAsync(_env.WebRootPath, "images", "service");
                }

                await _service.UpdateOurServiceAsync(id, ourServiceFromDb);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = ourServiceFromDb.ID}, ourServiceFromDb);
        }

        // DELETE api/<OurServiceController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var ourService = await _service.GetOurServiceByIdAsync(id);

            if (ourService == null)
                return BadRequest();
            try
            {
                RemoveFile(_env.WebRootPath, "images", "service", ourService.Image);
                await _service.DeleteOurServiceAsync(ourService);
            }

            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}