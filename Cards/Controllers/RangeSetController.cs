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
    [Route("api/rangesets")]
    [ApiController]
    public class RangeSetController : ControllerBase
    {
        private SuitRangePoolService suitRangePoolService;

        public RangeSetController(IServiceProvider serviceProvider)
        {
            suitRangePoolService = serviceProvider.GetRequiredService<SuitRangePoolService>();
        }

        /// <summary>
        /// Получить наборы рангов карт
        /// </summary>       
        /// <returns></returns>
        // GET api/rangesets
        [HttpGet]
        public ActionResult<IEnumerable<RangeSetView>> Get()
        {
            return Ok(suitRangePoolService.GetRangeSets().Select(s=> suitRangePoolService.ToDto(s)));
        }

        /// <summary>
        /// Получить набор рангов карт по id
        /// </summary>       
        /// <returns></returns>
        // GET api/rangesets/{id}
        [HttpGet("{id}")]
        public ActionResult<RangeSetView> Get(Guid id)
        {
            var ret = suitRangePoolService.GetRangeSet(id);
            if (ret != null)
            {
                return Ok(suitRangePoolService.ToDto(ret));
            }
            else return NotFound($"RangeSet with id = {id} not found");
        }

        /// <summary>
        /// Добавить набор рангов карт.
        /// Параметры: имя набора, старшинство рангов в наборе (набор "id ранга": "значение")
        /// При запуске сервиса создаются 2 набора рангов: 9 (с 6 до туза) и 13 (с 2 до туза),
        /// отсортированных по возрастанию
        /// </summary>       
        /// <returns></returns>
        // POST api/rangesets
        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromBody] string name, Dictionary<Guid, int> set)
        {
            var rangesets = suitRangePoolService.GetRangeSets();
            if (rangesets.Count(s => s.Name.Equals(name, StringComparison.CurrentCulture)) > 0)
            {
                return BadRequest("RangeSet with such name yet exists");
            }
            return Ok(await suitRangePoolService.CreateRangeSetAsync(name, set));
        }

        /// <summary>
        /// Переименовать набор рангов карт.        
        /// </summary>       
        /// <returns></returns>
        // PUT api/rangesets/{id}
        [HttpPut("{id}")]
        public ActionResult<RangeSetView> Put(Guid id, [FromBody] string name)
        {
            var rangesets = suitRangePoolService.GetRangeSets();
            if (rangesets.Count(s => s.Name.Equals(name, StringComparison.CurrentCulture) && s.Id != id) > 0)
            {
                return BadRequest("RangeSet with such name yet exists");
            }
            var rangeset = suitRangePoolService.GetRangeSet(id);
            rangeset.Name = name;
            return Ok(suitRangePoolService.ToDto(rangeset));
        }
    }
}
