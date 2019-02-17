using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cards.DTO;
using Cards.Models;
using Cards.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Cards.Controllers
{
    [Route("api/suitsets")]
    [ApiController]
    public class SuitSetController : ControllerBase
    {
        private SuitRangePoolService suitRangePoolService;

        public SuitSetController(IServiceProvider serviceProvider)
        {
            suitRangePoolService = serviceProvider.GetRequiredService<SuitRangePoolService>();
        }

        /// <summary>
        /// Получить наборы мастей
        /// </summary>       
        /// <returns></returns>
        // GET api/suitsets
        [HttpGet]
        public ActionResult<IEnumerable<SuitSetView>> Get()
        {
            return Ok(suitRangePoolService.GetSuitSets());
        }

        /// <summary>
        /// Получить набор мастей по id
        /// </summary>       
        /// <returns></returns>
        // GET api/suitsets/{id}
        [HttpGet("{id}")]
        public ActionResult<SuitSet> Get(Guid id)
        {
            var ret = suitRangePoolService.GetSuitSet(id);
            if (ret != null)
            {
                return Ok(ret);
            }
            else return NotFound($"SuitSet with id = {id} not found");
        }

        /// <summary>
        /// Добавить набор мастей (при запуске сервиса создаётся набор из 4 мастей без старшинства)
        /// </summary>       
        /// <returns></returns>
        // POST api/suitsets
        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromBody] string name, Dictionary<Guid, int> set)
        {
            var suits = suitRangePoolService.GetSuitSets();
            if (suits.Count(s => s.Name.Equals(name, StringComparison.CurrentCulture)) > 0)
            {
                return BadRequest("SuitSet with such name yet exists");
            }
            return Ok(await suitRangePoolService.CreateSuitSetAsync(name, set));
        }

        /// <summary>
        /// Переименовать набор мастей
        /// </summary>       
        /// <returns></returns>
        // PUT api/suitsets/{id}
        [HttpPut("{id}")]
        public ActionResult<SuitSet> Put(Guid id, [FromBody] string name)
        {
            var suits = suitRangePoolService.GetSuitSets();
            if (suits.Count(s => s.Name.Equals(name, StringComparison.CurrentCulture) && s.Id != id) > 0)
            {
                return BadRequest("SuitSet with such name yet exists");
            }
            var suit = suitRangePoolService.GetSuitSet(id);
            suit.Name = name;
            return Ok(suit);
        }
    }
}
