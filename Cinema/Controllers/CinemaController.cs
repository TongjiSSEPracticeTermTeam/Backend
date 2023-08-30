using Cinema.DTO;
using Cinema.DTO.CinemaService;
using Cinema.DTO.MoviesService;
using Cinema.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Cinema.Controllers;

/// <summary>
/// 电影院实体类控制器类
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CinemaController
{
    private readonly CinemaDb _db;
    private readonly ILogger _logger;

    private static int _cinemaId;

    /// <summary>
    /// 控制器构造函数
    /// </summary>
    /// <param name="db"></param>
    /// <param name="logger"></param>
    public CinemaController(CinemaDb db, ILogger<CinemaController> logger)
    {
        _db = db;
        _logger = logger;

        if (_cinemaId == 0)
        {
            _cinemaId = int.Parse(_db.Cinemas.Max(m => m.CinemaId) ?? "0");
        }
    }

    ///// <summary>
    ///// 获得所有电影院信息
    ///// </summary>
    ///// <returns>
    ///// 返回电影院json列表
    ///// </returns>
    //[HttpGet]
    //[ProducesDefaultResponseType(typeof(APIDataResponse<List<CinemaDTO>>))]
    //public async Task<IAPIResponse> GetCinemasAll()
    //{
    //    var cinemas = await _db.Cinemas.ToArrayAsync().OrderBy(c => c.CinemaId);
    //    var cinemaDTOs = cinemas.Select(c => new CinemaDTO(c)).ToList();
    //    return APIDataResponse<List<CinemaDTO>>.Success(cinemaDTOs);
    //}

    /// <summary>
    /// 管理端接口，获取所有电影院的信息（分页）
    /// </summary>
    /// <returns></returns>
    /// <remarks>提醒，要分页！分页从1开始，小于1出现未定义行为</remarks>
    [HttpGet]
    //[Authorize(Policy = "CinemaAdmin")]
    [ProducesDefaultResponseType(typeof(APIDataResponse<List<CinemaDTO>>))]
    public async Task<IAPIResponse> GetCinemas([FromQuery] ulong page_size, [FromQuery] ulong page_number)
    {
        var cinemas = await _db.Cinemas
                .Skip((int)((page_number - 1ul) * page_size))
                .Take((int)page_size)
                .OrderBy(m => m.CinemaId)
                .ToArrayAsync();
        var cinemaDTOs = cinemas.Select(c => new CinemaDTO(c)).ToList();
        return APIDataResponse<List<CinemaDTO>>.Success(cinemaDTOs);
    }

    /// <summary>
    /// 管理端接口，获取所有电影院的数量
    /// </summary>
    /// <returns></returns>
    /// <remarks>用于分页</remarks>
    [HttpGet("length")]
    [Authorize(Policy = "CinemaAdmin")]
    [ProducesDefaultResponseType(typeof(APIDataResponse<int>))]
    public async Task<IAPIResponse> GetCinemasLength()
    {
        var length = await _db.Cinemas.CountAsync();
        return APIDataResponse<int>.Success(length);
    }

    /// <summary>
    /// 管理端接口，添加电影院
    /// </summary>
    /// <param name="cinema"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Policy = "SysAdmin")]
    [ProducesDefaultResponseType(typeof(APIResponse))]
    public async Task<IAPIResponse> AddMovie([FromBody] CinemaDTO cinema)
    {
        var cinemaId = Interlocked.Increment(ref _cinemaId);
        cinema.CinemaId = $"{cinemaId:000000}";

        var cinemaEntity = new Cinemas
        {
            CinemaId = cinema.CinemaId,
            Name = cinema.Name,
            Location = cinema.Location,
            ManagerId = cinema.ManagerId,
            CinemaImageUrl = cinema.CinemaImageUrl,
            Feature = cinema.Feature,
        };

        await _db.Cinemas.AddAsync(cinemaEntity);
        await _db.SaveChangesAsync();
        return APIResponse.Success();
    }

    /// <summary>
    /// 管理端接口，修改电影院信息
    /// </summary>
    /// <param name="cinema"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "CinemaAdmin")]
    [ProducesDefaultResponseType(typeof(APIResponse))]
    public async Task<IAPIResponse> EditMovie([FromBody] CinemaDTO cinema)
    {
        var cinemaEntity = await _db.Cinemas.FindAsync(cinema.CinemaId);
        if (cinemaEntity == null)
        {
            return APIResponse.Failaure("4000", "电影院不存在");
        }

        cinemaEntity.CinemaId = cinema.CinemaId;
        cinemaEntity.Name = cinema.Name;
        cinemaEntity.Location = cinema.Location;
        cinemaEntity.ManagerId = cinema.ManagerId;
        cinemaEntity.CinemaImageUrl = cinema.CinemaImageUrl;
        cinemaEntity.Feature = cinema.Feature;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return APIResponse.Failaure("5000", "服务器内部错误");
        }

        //_db.Cinemas.Update(cinemaEntity);
        //await _db.SaveChangesAsync();
        return APIResponse.Success();
    }

    /// <summary>
    /// 根据id获得指定电影院信息
    /// </summary>
    /// <param name="id">电影院id</param>
    /// <returns>
    /// 返回电影院json
    /// </returns>
    [HttpGet("{id}")]
    [ProducesDefaultResponseType(typeof(APIDataResponse<CinemaDTO>))]
    public async Task<IAPIResponse> GetCinemaById(string id)
    {
        var cinema = await _db.Cinemas.FindAsync(id);
        if (cinema == null)
        {
            //return new JsonResult(new GetCinemaByIdResponse
            //{
            //    Status = "4001",
            //    Message = "该电影院不存在"
            //});
            return APIDataResponse<CinemaDTO>.Failaure("4001", "电影院不存在");
        }

        //return new JsonResult(new GetCinemaByIdResponse
        //{
        //    Status = "10000",
        //    Message = "查询成功",
        //    Cinema = cinema
        //});
        var cinemaDTO = new CinemaDTO(cinema);
        return APIDataResponse<CinemaDTO>.Success(cinemaDTO);
    }

    /// <summary>
    /// 根据管理员id获得指定电影院信息
    /// </summary>
    /// <param name="id">管理员id</param>
    /// <returns>
    /// 返回电影院json
    /// </returns>
    [HttpGet("byManagerId/{id}")]
    [ProducesDefaultResponseType(typeof(APIDataResponse<CinemaDTO>))]
    public async Task<IAPIResponse> GetCinemaByManagerId(string id)
    {
        var cinema = await _db.Cinemas.FirstOrDefaultAsync(c => c.ManagerId == id);
        if (cinema == null)
        {
            //return new JsonResult(new GetCinemaByManagerIdResponse
            //{
            //    Status = "4001",
            //    Message = "电影院不存在"
            //});
            return APIDataResponse<CinemaDTO>.Failaure("4001", "电影院不存在");
        }

        //return new JsonResult(new GetCinemaByManagerIdResponse
        //{
        //    Status = "10000",
        //    Message = "查询成功",
        //    Cinema = cinema
        //});
        var cinemaDTO = new CinemaDTO(cinema);
        return APIDataResponse<CinemaDTO>.Success(cinemaDTO);
    }

    /// <summary>
    /// 根据电影院名称获得指定电影院信息,模糊查询
    /// </summary>
    /// <param name="name">电影院名称</param>
    /// <returns>
    /// 返回电影院json
    /// </returns>
    [HttpGet("byName/{name}")]
    [ProducesDefaultResponseType(typeof(APIDataResponse<List<CinemaDTO>>))]
    public async Task<APIResponse> GetCinemaByName(string name)
    {
        var cinemas = await _db.Cinemas.Where(c => c.Name.Contains(name)).ToListAsync();
        if (cinemas!.Count == 0)
        {
            //return new JsonResult(new GetCinemaByNameResponse
            //{
            //    Status = "4001",
            //    Message = "电影院不存在"
            //});
            return APIDataResponse<List<CinemaDTO>>.Failaure("4001", "电影院不存在");
        }

        //var response = new GetCinemaByNameResponse
        //{
        //    Status = "10000",
        //    Message = "查询成功",
        //    Cinemas = cinemas!
        //};
        //return new JsonResult(response);
        var cinemaDTOs = cinemas.Select(c => new CinemaDTO(c)).ToList();
        return APIDataResponse<List<CinemaDTO>>.Success(cinemaDTOs);
    }

    /// <summary>
    /// 通过特点搜索到电影院列表
    /// </summary>
    /// <param name="feature">电影院特点</param>
    /// <returns></returns>
    [HttpGet("byFeature/{feature}")]
    [ProducesDefaultResponseType(typeof(APIDataResponse<List<CinemaDTO>>))]
    public async Task<IAPIResponse> GetCinemaByFeature(string feature)
    {
        var cinemas = await _db.Cinemas.Where(c => c.Feature != null && c.Feature.Contains(feature)).ToListAsync();

        if (cinemas.Count == 0)
        {
            return APIResponse.Failaure("4001","电影院不存在");
        }

        var cinemaDTOs = cinemas.Select(c => new CinemaDTO(c)).ToList();
        return APIDataResponse<List<Cinemas>>.Success(cinemas);
    }

    /// <summary>
    /// 通过ID删除对应电影院
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "SysAdmin")]
    [ProducesDefaultResponseType(typeof(APIResponse))]
    public async Task<IAPIResponse> DeleteCinemaById([FromRoute] string id)
    {
        var cinema = await _db.Cinemas.FindAsync(id);

        if (cinema == null)
        {
            return APIResponse.Failaure("4001","电影院不存在");
        }

        _db.Cinemas.Remove(cinema);
        await _db.SaveChangesAsync();

        return APIResponse.Success();
    }
}