﻿using Cinema.DTO;
using Cinema.DTO.MoviesService;
using Cinema.DTO.StaffService;
using Cinema.Entities;
using Cinema.Helpers;
using Cinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TencentCloud.Es.V20180416.Models;
using TencentCloud.Tic.V20201117.Models;

namespace Cinema.Controllers
{
    /// <summary>
    /// 影人控制器类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly CinemaDb _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly JwtHelper _jwtHelper;
        private readonly QCosSrvice _qCosSrvice;

        private static int _staffId;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logger"></param>
        /// <param name="jwtHelper"></param>
        /// <param name="qCosSrvice"></param>
        public StaffController(CinemaDb db, IHttpContextAccessor httpContextAccessor, ILogger<StaffController> logger, JwtHelper jwtHelper, QCosSrvice qCosSrvice)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _jwtHelper = jwtHelper;
            _qCosSrvice = qCosSrvice;

            if (_staffId == 0)
            {
                _staffId = int.Parse(_db.Staffs.Max(m => m.StaffId) ?? "0");
            }
        }

        /// <summary>
        /// 获得所有影人的ID和名字，管理员接口
        /// </summary>
        /// <returns>
        /// 返回影人json列表
        /// </returns>
        [HttpGet("all")]
        //[Authorize(Policy = "SysAdmin")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<EStaff>>))]
        public async Task<IAPIResponse> GeteStaffs()
        {
            var staffs = await _db.Staffs.OrderBy(staff => staff.StaffId).ToArrayAsync();
            var eStaffs = staffs.Select(s => new EStaff(s)).ToList();
            //return new JsonResult(await _db.Staffs.ToListAsync());
            return APIDataResponse<List<EStaff>>.Success(eStaffs);
        }

        /// <summary>
        /// 获取所有影人的信息（分页）
        /// </summary>
        /// <returns></returns>
        /// <remarks>提醒，要分页！分页从1开始，小于1出现未定义行为</remarks>
        [HttpGet]
        //[Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<Staff>>))]
        public async Task<IAPIResponse> GetStaffs([FromQuery] ulong page_size, [FromQuery] ulong page_number)
        {
            var staffs = await _db.Staffs
                    .Skip((int)((page_number - 1ul) * page_size))
                    .Take((int)page_size)
                    .OrderBy(m => m.StaffId)
                    .ToListAsync();
            return APIDataResponse<List<Staff>>.Success(staffs);
        }

        /// <summary>
        /// 获取所有影人的数量
        /// </summary>
        /// <returns></returns>
        /// <remarks>用于分页</remarks>
        [HttpGet("length")]
        //[Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<int>))]
        public async Task<IAPIResponse> GetStaffsLength()
        {
            var length = await _db.Staffs.CountAsync();
            return APIDataResponse<int>.Success(length);
        }

        /// <summary>
        /// 根据id获得指定影人信息
        /// </summary>
        /// <param name="staffId">影人id</param>
        /// <returns>
        /// 返回对应影人json
        /// </returns>
        [HttpGet("{staffId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<Staff>))]
        public async Task<IAPIResponse> GetStaffById([FromRoute] string staffId)
        {
            var staff = await _db.Staffs.FindAsync(staffId);
            if (staff == null)
            {
                return APIResponse.Failaure("4001", "该影人不存在");
            }

            return APIDataResponse<Staff>.Success(staff);
        }

        /// <summary>
        /// 根据名字获取影人信息，模糊匹配
        /// </summary>
        /// <param name="name">影人名字</param>
        /// <returns>
        /// 返回影人json列表
        /// </returns>
        [HttpGet("ByName/{name}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<StaffDTO>>))]
        public async Task<IAPIResponse> GetStaffByName([FromRoute] string name)
        {
            var staffs = await _db.Staffs
                .Where(s => s.Name.Contains(name))
                .ToListAsync();
            if (staffs!.Count() == 0)
            {
                return APIResponse.Failaure("4001", "影人不存在");
            }
            var staffDTOs = staffs.Select(s => new StaffDTO(s)).ToList();
            return APIDataResponse<List<StaffDTO>>.Success(staffDTOs);
        }

        /// <summary>
        /// 通过ID删除对应影人
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [HttpDelete("{staffId}")]
        [Authorize(Policy = "SysAdmin")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> DeleteStaffById([FromRoute] string staffId)
        {
            var staff = await _db.Staffs.FindAsync(staffId);

            if (staff == null)
            {
                return APIResponse.Failaure("4001", "该影人不存在");
            }

            _db.Staffs.Remove(staff);

            try
            {
                await _db.SaveChangesAsync();

                return APIResponse.Success();
            }
            catch
            {
                return APIResponse.Failaure("10001", "删除失败");
            }
        }

        /// <summary>
        /// 添加影人
        /// </summary>
        /// <param name="staff"></param>
        /// <returns>响应信息</returns>
        [HttpPut]
        [Authorize(Policy = "SysAdmin")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<Staff>))]
        public async Task<IAPIResponse> AddStaff([FromBody] StaffDTO staff)
        {
            var staffId = Interlocked.Increment(ref _staffId);
            staff.StaffId = String.Format("{0:000000}", staffId);

            try
            {
                var staffEntity = new Staff
                {
                    StaffId = staff.StaffId,
                    Name = staff.Name,
                    Introduction = staff.Introduction,
                    ImageUrl = staff.ImageUrl,
                };
                //if (staff.Gender == Gender_.male)
                //{
                //    staffEntity.Gender = Gender_.male;
                //}
                //else
                //{
                //    staffEntity.Gender = Gender_.female;
                //}
                staffEntity.Gender = staff.Gender;
                await _db.Staffs.AddAsync(staffEntity);
                await _db.SaveChangesAsync();
                return APIDataResponse<Staff>.Success(staffEntity);
            }
            catch
            {
                return APIResponse.Failaure("10001", "影人添加失败");
            }
        }

        /// <summary>
        /// 修改影人
        /// </summary>
        /// <param name="staff"></param>
        /// <returns>响应信息</returns>
        [HttpPost]
        [Authorize(Policy = "SysAdmin")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<Staff>))]
        public async Task<IAPIResponse> UpdateStaff([FromBody] StaffDTO staff)
        {
            try
            {
                var staffEntity = await _db.Staffs.FindAsync(staff.StaffId);
                if (staffEntity == null)
                {
                    return APIResponse.Failaure("4000", "影人不存在");
                }

                staffEntity.StaffId = staff.StaffId;
                staffEntity.Name = staff.Name;
                //if (staff.Gender == 0)
                //{
                //    staffEntity.Gender = Gender_.male;
                //}
                //else
                //{
                //    staffEntity.Gender = Gender_.female;
                //}\
                staffEntity.Gender = staff.Gender;
                staffEntity.Introduction = staff.Introduction;
                staffEntity.ImageUrl = staff.ImageUrl;

                _db.Staffs.Update(staffEntity);
                await _db.SaveChangesAsync();
                return APIDataResponse<Staff>.Success(staffEntity);
            }
            catch
            {
                return APIResponse.Failaure("10001", "影人修改失败");
            }
        }

        /// <summary>
        /// 获得影人详情（含出演电影）
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [HttpGet("detail/{staffId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<StaffDetail>))]
        public async Task<IAPIResponse> GetStaffDetail([FromRoute] string staffId)
        {
            var staff = await _db.Staffs
                .Include(s => s.Acts)
                .ThenInclude(a => a.Movie)
                .FirstOrDefaultAsync(s => s.StaffId == staffId);

            if (staff == null)
            {
                return APIResponse.Failaure("10001", "影人不存在");
            }

            var staffDetail = new StaffDetail(staff);
            return APIDataResponse<StaffDetail>.Success(staffDetail);
        }

        /// <summary>
        /// 根据演员ID获取其导演的所有电影
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetMovieAsDirector/{id}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<MovieDTO>>))]
        public async Task<IAPIResponse> GetMovieAsDirector([FromRoute] string id)
        {
            var staff = await _db.Staffs.FindAsync(id);

            if (staff == null)
            {
                return APIResponse.Failaure("4001", "影人ID不存在");
            }

            var acts = await _db.Acts
                .Where(a => (a.StaffId == id) && (a.Role == "1"))
                .Include(a => a.Movie).ToListAsync();

            var movies = acts.Select(a => new MovieDTO(a.Movie))
                .OrderBy(m => m.MovieId)
                .ToList();

            return APIDataResponse<List<MovieDTO>>.Success(movies);
        }

        /// <summary>
        /// 根据演员ID获取其参演的所有电影
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetMovieAsActor/{id}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<MovieDTO>>))]
        public async Task<IAPIResponse> GetMovieAsActor([FromRoute] string id)
        {
            var staff = await _db.Staffs.FindAsync(id);

            if (staff == null)
            {
                return APIResponse.Failaure("4001", "影人ID不存在");
            }

            var acts = await _db.Acts
                .Where(a => (a.StaffId == id) && (a.Role == "0"))
                .Include(a => a.Movie).ToListAsync();

            var movies = acts.Select(a => new MovieDTO(a.Movie))
                .OrderBy(m => m.MovieId)
                .ToList();

            return APIDataResponse<List<MovieDTO>>.Success(movies);
        }
    }
}
