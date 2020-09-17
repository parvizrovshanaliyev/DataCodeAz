using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppApi.Models;
using DataAccess.Entites;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using static AppApi.Extensions.IFormFileExtension;


namespace AppApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IBannerService _service;

        public BannerController(IBannerService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        // GET: api/<BannerController>
        [HttpGet]
        public ActionResult<IEnumerable<Banner>> Get()
        {
            return _service.GetAllBanners().ToList();
        }

        // GET api/<BannerController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Banner>> Get(int id)
        {
            if (id == 0)
                return NotFound();

            var banner = await _service.GetBannerByIdAsync(id);

            if (banner == null)
                return NotFound();

            return banner;
        }

        // POST api/<BannerController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] BannerModel bannerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (bannerModel.Photo == null) return BadRequest();

            if (!bannerModel.Photo.IsImage()) return BadRequest();

            if (!bannerModel.Photo.LessThan(5)) return BadRequest();

            var banner = new Banner
            {
                Title = bannerModel.Title,
                Description = bannerModel.Description
            };

            try
            {
                banner.Image = await bannerModel.Photo.SaveAsync(_env.WebRootPath, "images", "banner");
                await _service.CreateBannerAsync(banner);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = banner.ID}, banner);
        }

        // PUT api/<BannerController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] BannerModel bannerModel)
        {
            var bannerFromDb = await _service.GetBannerByIdAsync(id);

            if (bannerFromDb == null)
                return NotFound();

            if (id != bannerModel.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            bannerFromDb.Title = bannerModel.Title;
            bannerFromDb.Description = bannerModel.Description;

            try
            {
                if (bannerModel.Photo != null)
                {
                    if (!bannerModel.Photo.IsImage()) return BadRequest();

                    if (!bannerModel.Photo.LessThan(6)) return BadRequest();

                    RemoveFile(_env.WebRootPath, "images", "banner", bannerFromDb.Image);

                    bannerFromDb.Image = await bannerModel.Photo.SaveAsync(_env.WebRootPath, "images", "banner");
                }

                await _service.UpdateBannerAsync(id, bannerFromDb);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = bannerFromDb.ID}, bannerFromDb);
        }

        // DELETE api/<BannerController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var banner = await _service.GetBannerByIdAsync(id);

            if (banner == null)
                return BadRequest();

            try
            {
                RemoveFile(_env.WebRootPath, "images", "banner", banner.Image);
                await _service.DeleteBannerAsync(banner);
            }

            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}