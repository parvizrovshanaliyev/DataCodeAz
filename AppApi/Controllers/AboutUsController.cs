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
    public class AboutUsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        private readonly IAboutUsService _service;

        public AboutUsController(IAboutUsService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }


        // GET: api/<AboutUsController>
        [HttpGet]
        public ActionResult<IEnumerable<AboutUs>> Get()
        {
            return _service.GetAllAboutUs().ToList();
        }

        // GET api/<AboutUsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AboutUs>> Get(int id)
        {
            if (id == 0)
                return NotFound();

            var aboutUs = await _service.GetAboutUsByIdAsync(id);

            if (aboutUs == null)
                return NotFound();

            return aboutUs;
        }

        // POST api/<AboutUsController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AboutModel aboutModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (aboutModel.Photo == null) return BadRequest();

            if (!aboutModel.Photo.IsImage()) return BadRequest();

            if (!aboutModel.Photo.LessThan(5)) return BadRequest();

            var aboutUs = new AboutUs
            {
                Title = aboutModel.Title,
                Description = aboutModel.Description
            };

            try
            {
                aboutUs.Image = await aboutModel.Photo.SaveAsync(_env.WebRootPath, "images", "about");
                await _service.CreateAboutAsync(aboutUs);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = aboutUs.ID}, aboutUs);
        }

        // PUT api/<AboutUsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] AboutModel aboutModel)
        {
            var aboutUsFromDb = await _service.GetAboutUsByIdAsync(id);

            if (aboutUsFromDb == null)
                return NotFound();

            if (id != aboutModel.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            aboutUsFromDb.Title = aboutModel.Title;
            aboutUsFromDb.Description = aboutModel.Description;

            try
            {
                if (aboutModel.Photo != null)
                {
                    if (!aboutModel.Photo.IsImage()) return BadRequest();

                    if (!aboutModel.Photo.LessThan(6)) return BadRequest();

                    RemoveFile(_env.WebRootPath, "images", "about", aboutUsFromDb.Image);

                    aboutUsFromDb.Image = await aboutModel.Photo.SaveAsync(_env.WebRootPath, "images", "about");
                }

                await _service.UpdateAboutUsAsync(id, aboutUsFromDb);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = aboutUsFromDb.ID}, aboutUsFromDb);
        }

        // DELETE api/<AboutUsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var aboutUs = await _service.GetAboutUsByIdAsync(id);

            if (aboutUs == null)
                return BadRequest();

            try
            {
                RemoveFile(_env.WebRootPath, "images", "about", aboutUs.Image);
                await _service.DeleteAboutUsAsync(aboutUs);
            }

            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}