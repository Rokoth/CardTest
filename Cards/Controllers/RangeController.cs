using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cards.Models;
using Cards.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Cards.Controllers
{
    [Route("api/ranges")]
    [ApiController]
    public class RangeController : ControllerBase
    {
        private SuitRangePoolService suitRangePoolService;

        public RangeController(IServiceProvider serviceProvider)
        {
            suitRangePoolService = serviceProvider.GetRequiredService<SuitRangePoolService>();
        }

        // GET api/ranges
        /// <summary>
        /// Получить ранги карт
        /// </summary>       
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Range>> Get()
        {
            return Ok(suitRangePoolService.GetRanges());
        }

        /// <summary>
        /// Получить ранг карт по id
        /// </summary>       
        /// <returns></returns>
        // GET api/ranges/{id}
        [HttpGet("{id}")]
        public ActionResult<Range> Get(Guid id)
        {
            var ret = suitRangePoolService.GetRange(id);
            if (ret != null)
            {
                return Ok(ret);
            }
            else return NotFound($"Range with id = {id} not found");
        }

        /// <summary>
        /// Добавить ранг карт
        /// </summary>       
        /// <returns></returns>
        // POST api/ranges
        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromBody] string name)
        {
            return Ok(await suitRangePoolService.CreateRangeAsync(name));
        }

        /// <summary>
        /// Переименовать ранг карт
        /// </summary>       
        /// <returns></returns>
        // PUT api/ranges/{id}
        [HttpPut("{id}")]
        public ActionResult<Range> Put(Guid id, [FromBody] string name)
        {
            var ranges = suitRangePoolService.GetSuits();
            if (ranges.Count(s => s.Name.Equals(name, StringComparison.CurrentCulture) && s.Id != id) > 0)
            {
                return BadRequest("Suit with such name yet exists");
            }
            var range = suitRangePoolService.GetRange(id);
            range.Name = name;
            return Ok(range);
        }
    }
}
