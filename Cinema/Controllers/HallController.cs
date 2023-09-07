using Cinema.DTO;
using Cinema.DTO.CinemaService;
using Cinema.DTO.ManagerService;
using Cinema.DTO.MoviesService;
using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using TencentCloud.Kms.V20190118.Models;
using TencentCloud.Mna.V20210119.Models;

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
        private static int _hallId;
        /// <summary>
        /// 控制器构造函数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="logger"></param>
        public HallController(CinemaDb db, ILogger<HallController> logger)
        {
            _db = db;
            _logger = logger;
            if (_hallId == 0)
            {
                _hallId = int.Parse(_db.Halls.Max(m => m.Id) ?? "0");
            }

        }

        /// <summary>
        /// 管理端接口，获取某个影院编号下的所有影厅
        /// </summary> 
        /// <param name="cinemaId">影院编号</param>
        /// <returns></returns>
        [HttpGet("{cinemaId}")]
        //[Authorize(Policy = "CinemaAdmin")]
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

        /// <summary>
        /// 管理端接口，获取某个经理管理下的所有影厅
        /// </summary> 
        /// <param name="managerId">经理编号</param>
        /// <returns></returns>
        [HttpGet("manager/{managerId}/halls")]
        //[Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<HallDTO>>))]
        public async Task<IAPIResponse> GetHallsByManagerId([FromRoute] string managerId)
        {
            var manager = await _db.Managers.Include(m => m.ManagedCinema)
                .ThenInclude(c => c.Halls)
                .FirstOrDefaultAsync(m => m.Id == managerId);
            if (manager == null)
            {
                return APIResponse.Failaure("4004", "经理不存在");
            }
            // 获取管理员管辖影院的所有影厅
            var halls = manager.ManagedCinema.Halls;
            // 映射实体为DTO
            var hallDTOs = halls.Select(h => new HallDTO(h)).ToList();
            // 封装并返回结果
            return APIDataResponse<List<HallDTO>>.Success(hallDTOs);
        }
        /// <summary>
        /// 管理端接口，查询某个Managers管理的所有Cinema
        /// </summary> 
        /// <param name="managerId">Managers编号</param>
        /// <returns></returns>
        [HttpGet("manager/{managerId}/cinemas")]
        //[Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<CinemaDTO>>))]
        public async Task<IAPIResponse> GetCinemasByManagerId([FromRoute] string managerId)
        {
            var manager = await _db.Managers
                                 .Include(m => m.ManagedCinema)
                                 .FirstOrDefaultAsync(m => m.Id == managerId);
            if (manager == null)
            {
                return APIResponse.Failaure("4004", "没有该用户");
            }
            var cinemaDTOs = new List<CinemaDTO>();

            if (manager.ManagedCinema != null)
            {
                cinemaDTOs.Add(new CinemaDTO(manager.ManagedCinema));
                return APIDataResponse<List<CinemaDTO>>.Success(cinemaDTOs);
            }
            else
            {
                return APIResponse.Failaure("4001", "用户下无影厅");
            }
        }

        /// <summary>
        /// 管理端接口，查询所有Managers的信息
        /// </summary> 
        /// <param></param>
        /// <returns></returns>
        [HttpGet("managers")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<ManagerDTO>>))]
        public async Task<IAPIResponse> GetAllManager()
        {
            var managers = await _db.Managers
                               .Include(m => m.ManagedCinema)
                               .Select(m => new ManagerDTO(m))
                               .ToListAsync();
            return APIDataResponse<List<ManagerDTO>>.Success(managers);
        }

        /// <summary>
        /// 管理员接口，添加影厅相关信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut]
        //[Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<HallDTO>))]
        public async Task<IAPIResponse> AddHall([FromBody] HallDTO data)
        {
            var cinema = await _db.Cinemas.FindAsync(data.CinemaId);
            if (cinema == null)
            {
                return APIResponse.Failaure("4004", "影院不存在");
            }

            var halls = _db.Halls.Where(h => h.CinemaId == data.CinemaId);
            //var maxId = int.Parse(_db.Halls.Max(m => m.Id) ?? "0");
            var maxId = int.Parse(halls.Max(m => m.Id) ?? "0");
            var _Id = maxId+1;
            var hall = new Hall
            {
                CinemaId = data.CinemaId,
                HallType = data.HallType,
                Seat = data.Seat,
                Id = $"{_Id:000000}"
            };
            
            await _db.Halls.AddAsync(hall);
            await _db.SaveChangesAsync();
            return APIDataResponse<HallDTO>.Success(new HallDTO(hall));
        }

        /// <summary>
        /// 管理员端口，通过hallId删除对应影厅
        /// </summary>
        /// <param name="hallId"></param>
        /// <param name="cinemaId"></param>
        /// <returns></returns>
        [HttpDelete("{hallId}")]
        //[Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> DeleteHall([FromRoute] string hallId, [FromQuery][Required]string cinemaId)
        {
            var hall = await _db.Halls.FirstAsync(h => h.Id == hallId && h.CinemaId == cinemaId);
            if (hall == null)
            {
                return APIResponse.Failaure("4004", "影厅不存在");
            }
            _db.Halls.Remove(hall);
            await _db.SaveChangesAsync();
            return APIResponse.Success();
        }

        /// <summary>
        /// 管理端端口，更新影厅信息
        /// </summary>
        /// <param name="hall"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> EditHall([FromBody] HallDTO hall)
        {
            var hhall=await _db.Halls.FindAsync(hall.Id ,hall.CinemaId);
            if (hhall == null)
            {
                return APIResponse.Failaure("4004", "影厅不存在");
            }
            hhall.HallType = hall.HallType;
            hhall.Seat = hall.Seat;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return APIResponse.Failaure("5000", "服务器内部错误");
            }
            await _db.SaveChangesAsync();
            return APIResponse.Success();
        }

    }
}
