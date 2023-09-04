using Cinema.DTO;
using Cinema.DTO.ManagerService;
using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using TencentCloud.Kms.V20190118.Models;

namespace Cinema.Controllers
{
    /// <summary>
    /// 影厅实体类控制器类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HallController
    {
        private readonly CinemaDb _db;
        private readonly ILogger _logger;
        private static int _cinemaId;
        /// <summary>
        /// 控制器构造函数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="logger"></param>
        public HallController(CinemaDb db, ILogger<HallController> logger)
        {
            _db = db;
            _logger = logger;
/*            if (_cinemaId == 0)
            {
                _cinemaId = int.Parse(_db.Cinemas.Max(m => m.CinemaId) ?? "0");
            }*/

        }
        /// <summary>
        /// 管理端接口，获取某个影院编号下的所有影厅
        /// </summary> 
        /// <param name=“cinemaId”>影院编号</param>
        /// <returns></returns>
        [HttpGet("{cinemaId}")]
        //[Authorize(Policy = “CinemaAdmin”)]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<HallDTO>>))]
        public async Task<IAPIResponse> GetHallsByCinemaId([FromRoute] string cinemaId)
        {
            var cinema = await _db.Cinemas.FindAsync(cinemaId);
            if (cinema == null)
            {
                return APIResponse.Failaure("4004", "影院不存在");
            }
            var halls = await _db.Halls.Where(h => h.CinemaId == cinemaId) // 根据影院编号筛选影厅
                       .OrderBy(h => h.Id) .ToArrayAsync();
            if (halls.Length == 0)
            {
                return APIResponse.Failaure("4001", "影院下没有影厅");
            }
            var hallDTOs = halls.Select(h => new HallDTO(h)).ToList(); 

            return APIDataResponse<List<HallDTO>>.Success(hallDTOs);
        }

    }
}
