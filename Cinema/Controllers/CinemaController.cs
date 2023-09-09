using Cinema.DTO.ManagerService;
using Cinema.DTO;
using Cinema.DTO.CinemaService;
using Cinema.DTO.MoviesService;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using TencentCloud.Kms.V20190118.Models;

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
    /// 管理端接口，获取所有电影院信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("all")]
    [ProducesDefaultResponseType(typeof(APIDataResponse<List<CinemaDTO>>))]
    public async Task<IAPIResponse> GetAllCinemas()
    {
        var cinemas = await _db.Cinemas.OrderBy(c => c.CinemaId).ToArrayAsync();
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

    ///// <summary>
    ///// 管理端接口，添加电影院
    ///// </summary>
    ///// <param name="cinema"></param>
    ///// <returns></returns>
    //[HttpPut]
    //[Authorize(Policy = "SysAdmin")]
    //[ProducesDefaultResponseType(typeof(APIResponse))]
    //public async Task<IAPIResponse> AddMovie([FromBody] CinemaDTO cinema)
    //{
    //    var cinemaId = Interlocked.Increment(ref _cinemaId);
    //    cinema.CinemaId = $"{cinemaId:000000}";

    //    var cinemaEntity = new Cinemas
    //    {
    //        CinemaId = cinema.CinemaId,
    //        Name = cinema.Name,
    //        Location = cinema.Location,
    //        ManagerId = cinema.ManagerId,
    //        CinemaImageUrl = cinema.CinemaImageUrl,
    //        Feature = cinema.Feature,
    //    };

    //    await _db.Cinemas.AddAsync(cinemaEntity);
    //    await _db.SaveChangesAsync();
    //    return APIResponse.Success();
    //}

    /// <summary>
    /// 管理员接口，添加电影院和对应管理员
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Policy = "SysAdmin")]
    [ProducesDefaultResponseType(typeof(APIDataResponse<CinemaDTO>))]
    public async Task<IAPIResponse> AddMoviewithManager([FromBody] CinemaCreator data)
    {
        var cinemaId = Interlocked.Increment(ref _cinemaId);
        data.CinemaId = $"{cinemaId:000000}";

        var cinemaEntity = new Cinemas
        {
            CinemaId = data.CinemaId,
            Name = data.Name,
            Location = data.Location,
            ManagerId = data.CinemaId,
            CinemaImageUrl = data.CinemaImageUrl,
            Feature = data.Feature,
        };
        var cinemaDTO = new CinemaDTO(cinemaEntity);

        var managerEntity = new Manager
        {
            Id = data.CinemaId,
            Name = "m-"+data.CinemaId,
            Password = Md5Helper.CalculateMd5Hash(data.ManagerPassword),
            Email = "m-"+data.CinemaId+"@cinema.com",
            AvatarUrl = String.Empty
        };

        await _db.Managers.AddAsync(managerEntity);
        await _db.Cinemas.AddAsync(cinemaEntity);
        await _db.SaveChangesAsync();
        return APIDataResponse<CinemaDTO>.Success(cinemaDTO);
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

        await _db.SaveChangesAsync();
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
            return APIDataResponse<CinemaDTO>.Failaure("4001", "电影院不存在");
        }

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
            return APIDataResponse<List<CinemaDTO>>.Failaure("4001", "电影院不存在");
        }

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


    ///<summary>
    ///获取所有影院的所有feature
    ///</summary>
    [HttpGet("features")]
    [ProducesDefaultResponseType(typeof(List<String>))]
    public async Task<IAPIResponse> GetAllfeatures()
    {
        var cinemaFeatures = new List<string>();
        var cinemas = await _db.Cinemas.ToListAsync();
        foreach (var cinema in cinemas)
        {
            if (cinema.Feature != null)
            {
                var features = cinema.Feature.Split(',');
                foreach (var feature in features)
                {
                    if (!cinemaFeatures.Contains(feature))
                    {
                        cinemaFeatures.Add(feature);
                    }
                }
            }
        }
        return APIDataResponse<List<string>>.Success(cinemaFeatures);
    }

}