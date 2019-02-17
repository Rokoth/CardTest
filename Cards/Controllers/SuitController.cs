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
    [Route("api/suits")]
    [ApiController]
    public class SuitController : ControllerBase
    {
        private SuitRangePoolService suitRangePoolService;

        public SuitController(IServiceProvider serviceProvider)
        {
            suitRangePoolService = serviceProvider.GetRequiredService<SuitRangePoolService>();
        }

        /// <summary>
        /// Получить масти карт
        /// </summary>       
        /// <returns></returns>
        // GET api/suits
        [HttpGet]
        public ActionResult<IEnumerable<Suit>> Get()
        {
            return Ok(suitRangePoolService.GetSuits());
        }

        /// <summary>
        /// Получить масть карт по id
        /// </summary>       
        /// <returns></returns>
        // GET api/suits/{id}
        [HttpGet("{id}")]
        public ActionResult<Suit> Get(Guid id)
        {
            var ret = suitRangePoolService.GetSuit(id);
            if (ret != null)
            {
                return Ok(ret);
            }
            else return NotFound($"Suit with id = {id} not found");
        }

        /// <summary>
        /// Добавить масть карт
        /// </summary>       
        /// <returns></returns>
        // POST api/suits
        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromBody] string name)
        {
            return Ok(await suitRangePoolService.CreateSuitAsync(name));
        }
        
        /// <summary>
        /// Переименовать масть карт
        /// </summary>       
        /// <returns></returns>
        // PUT api/suits/{id}
        [HttpPut("{id}")]
        public ActionResult<Suit> Put(Guid id, [FromBody] string name)
        {
            var suits = suitRangePoolService.GetSuits();
            if (suits.Count(s => s.Name.Equals(name, StringComparison.CurrentCulture) && s.Id != id) > 0)
            {
                return BadRequest("Suit with such name yet exists");
            }
            var suit = suitRangePoolService.GetSuit(id);
            suit.Name = name;
            return Ok(suit);
        }
    }
}
