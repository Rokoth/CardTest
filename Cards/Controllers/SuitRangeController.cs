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
    [Route("api/suit-ranges")]
    [ApiController]
    public class SuitRangeController : ControllerBase
    {
        private SuitRangePoolService suitRangePoolService;

        public SuitRangeController(IServiceProvider serviceProvider)
        {
            suitRangePoolService = serviceProvider.GetRequiredService<SuitRangePoolService>();
        }

        /// <summary>
        /// Получить наборы Мастей-Рангов (типы колод)
        /// </summary>       
        /// <returns></returns>
        // GET api/suit-ranges
        [HttpGet]
        public ActionResult<IEnumerable<SuitRangeView>> Get()
        {
            var ret = suitRangePoolService.GetSuitRanges();
            var result = ret.Select(s => suitRangePoolService.ToDto(s));
            return Ok(result);
        }

        /// <summary>
        /// Получить набор Мастей-Рангов (типа колод) по id
        /// </summary>       
        /// <returns></returns>
        // GET api/suit-ranges/{id}
        [HttpGet("{id}")]
        public ActionResult<SuitRangeView> Get(Guid id)
        {
            var ret = suitRangePoolService.GetSuitRange(id);
            if (ret != null)
            {
                return Ok(suitRangePoolService.ToDto(ret));
            }
            else return NotFound($"SuitRange with id = {id} not found");
        }

        /// <summary>
        /// Добавить набор Мастей-Рангов (типа колод) по id
        /// Параметры: имя типа колоды, id набора мастей, id набора рангов
        /// При запуске создаются два типа колоды по умолчанию: 36 карт и 52 карты 
        /// (старшинство рангов по возрастанию, старшинство мастей не установлено)
        /// </summary>       
        /// <returns></returns>
        // POST api/suit-ranges
        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromBody] string name, Guid suitSetId, Guid rangeSetId)
        {
            var suitRanges = suitRangePoolService.GetSuitRanges();
            if (suitRanges.Count(s => s.Name.Equals(name, StringComparison.CurrentCulture)) > 0)
            {
                return BadRequest("Suit with such name yet exists");
            }
            return Ok(await suitRangePoolService.AddToSuitRangePoolAsync(name, suitSetId, rangeSetId));
        }

        /// <summary>
        /// Переименовать набор Мастей-Рангов (типа колод)
        /// </summary>       
        /// <returns></returns>
        // PUT api/suit-ranges/{id}
        [HttpPut("{id}")]
        public ActionResult<SuitRangeView> Put(Guid id, [FromBody] string name)
        {
            var suitRanges = suitRangePoolService.GetSuitRanges();
            if (suitRanges.Count(s => s.Name.Equals(name, StringComparison.CurrentCulture) && s.Id != id) > 0)
            {
                return BadRequest("Suit with such name yet exists");
            }
            var suitRange = suitRangePoolService.GetSuitRange(id);
            suitRange.Name = name;
            return Ok(suitRangePoolService.ToDto(suitRange));
        }
    }
}
