using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace AppApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PortfolioCategoryController : ControllerBase
    {
        private readonly IPortfolioCategoryService _service;

        public PortfolioCategoryController(IPortfolioCategoryService service)
        {
            _service = service;
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public ActionResult<IEnumerable<PortfolioCategory>> Get()
        {
            return _service.GetAllPortfolios().ToList();
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PortfolioCategory>> Get(int id)
        {
            if (id == 0)
                return NotFound();

            var category = await _service.GetPortfolioByIdAsync(id);

            if (category == null)
                return NotFound();

            return category;
        }

        // POST api/<CategoryController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PortfolioCategory category)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                await _service.CreatePortfolioAsync(category);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = category.ID}, category);
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] PortfolioCategory category)
        {
            var categoryFromDb = await _service.GetPortfolioByIdAsync(id);

            if (categoryFromDb == null)
                return NotFound();

            if (id != category.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                await _service.UpdatePortfolioAsync(id, category);
            }

            catch
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Get), new {id = category.ID}, category);
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var category = await _service.GetPortfolioByIdAsync(id);

            if (category == null)
                return BadRequest();

            try
            {
                await _service.DeletePortfolioAsync(category);
            }

            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}