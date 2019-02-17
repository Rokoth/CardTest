using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cards.DTO;
using Cards.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using static Cards.Services.PackPoolService;

namespace Cards.Controllers
{
    [Route("api/packs")]
    [ApiController]
    public class PackController : ControllerBase
    {
        private PackPoolService packPoolService;
        private SuitRangePoolService suitRangePoolService;

        public PackController(IServiceProvider serviceProvider)
        {
            packPoolService = serviceProvider.GetRequiredService<PackPoolService>();
            suitRangePoolService = serviceProvider.GetRequiredService<SuitRangePoolService>();
        }

        // GET api/packs
        /// <summary>
        /// Получить все колоды
        /// </summary>       
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<PackView>> Get()
        {
            return Ok(packPoolService.GetPacks().Select(s=> suitRangePoolService.ToDto(s)));
        }

        // GET api/packs/sorts
        /// <summary>
        /// Типы сортировки колод
        /// </summary>       
        /// <returns></returns>
        [HttpGet("sorts")]
        public ActionResult<Dictionary<Sort, string>> GetSorts()
        {
            return Ok(packPoolService.GetSorts());
        }

        /// <summary>
        /// Получить колоду по id
        /// </summary>       
        /// <returns></returns>
        // GET api/packs/{id}
        [HttpGet("{id}")]
        public ActionResult<PackView> Get(Guid id)
        {
            var ret = packPoolService.GetPack(id);
            if (ret != null)
            {
                return Ok(suitRangePoolService.ToDto(ret));
            }
            else return NotFound($"Pack with id = {id} not found");
        }

        /// <summary>
        /// Получить колоду по имени
        /// </summary>       
        /// <returns></returns>
        // GET api/packs/{name}
        [HttpGet("{name}")]
        public ActionResult<PackView> Get(string name)
        {
            var ret = packPoolService.GetPack(name);
            if (ret != null)
            {
                return Ok(suitRangePoolService.ToDto(ret));
            }
            else return NotFound($"Pack with name = {name} not found");
        }

        /// <summary>
        /// Добавить новые колоды. Параметры: набор пар имя колоды и id набора Масти-Значения (типа колоды)
        /// </summary>       
        /// <returns></returns>
        // POST api/packs
        [HttpPost]
        public async Task<ActionResult<List<PackView>>> Post([FromBody] Dictionary<string, Guid> args)
        {
            List<PackView> result = new List<PackView>();
            foreach (var arg in args)
            {
                if (arg.Key == null || arg.Value == Guid.Empty)
                {
                    return BadRequest("Required parameters empty");
                    
                }
                else result.Add(suitRangePoolService.ToDto(await packPoolService.AddToPool(arg.Value, arg.Key)));
            }
            return Ok(result); 
        }

        /// <summary>
        /// Перемешать колоду. 
        /// Параметры: id колоды, тип сортировки, 
        /// количество перемешиваний (только для симуляции ручного)
        /// </summary>       
        /// <returns></returns>
        // POST api/packs/sort-by-guid
        [HttpPost("sort-by-guid")]
        public async Task<ActionResult<PackView>> Post([FromBody] Guid id, Sort sort, int count = 1)
        {
            var result = await packPoolService.SortPack(id, sort, count);
            if (result != null)
            {
                return Ok(suitRangePoolService.ToDto(result));
            }
            return BadRequest("Колода не найдена");
        }

        /// <summary>
        /// Перемешать колоду. 
        /// Параметры: имя колоды, тип сортировки, 
        /// количество перемешиваний (только для симуляции ручного)
        /// </summary>       
        /// <returns></returns>
        // POST api/packs/sort-by-name
        [HttpPost("sort-by-name")]
        public async Task<ActionResult<PackView>> Post([FromBody] string name, Sort sort, int count = 1)
        {
            var result = await packPoolService.SortPack(name, sort, count);
            if (result != null)
            {
                return Ok(suitRangePoolService.ToDto(result));
            }
            return BadRequest("Колода не найдена");
        }

        /// <summary>
        /// Переименовать колоду. 
        /// Параметры: имя колоды, id колоды
        /// </summary>       
        /// <returns></returns>
        // PUT api/packs/{id}
        [HttpPut("{id}")]
        public ActionResult<PackView> Put(Guid id, [FromBody] string name)
        {
            var packs = packPoolService.GetPacks();
            if (packs.Count(s => s.Name.Equals(name, StringComparison.CurrentCulture) && s.Id != id) > 0)
            {
                return BadRequest("Pack with such name yet exists");
            }
            var pack = packPoolService.GetPack(id);
            pack.Name = name;
            return Ok(suitRangePoolService.ToDto(pack));
        }
    }
}
