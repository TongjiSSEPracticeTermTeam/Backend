using Cinema.DTO.CinemaService;
using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers;

/// <summary>
/// 电影院实体类控制器类
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CinemaController
{
    private readonly CinemaDb _db;

    /// <summary>
    /// 控制器构造函数
    /// </summary>
    /// <param name="db"></param>
    public CinemaController(CinemaDb db)
    {
        _db = db;
    }

    /// <summary>
    /// 获得所有电影院信息
    /// </summary>
    /// <returns>
    /// 返回电影院json列表
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetCinemas()
    {
        return new JsonResult(await _db.Cinemas.ToListAsync());
    }

    /// <summary>
    /// 根据id获得指定电影院信息
    /// </summary>
    /// <param name="id">电影院id</param>
    /// <returns>
    /// 返回电影院json
    /// </returns>
    [HttpGet("getCinemaById/{id}")]
    public async Task<IActionResult> GetCinemaById(string id)
    {
        var cinema = await _db.Cinemas.FindAsync(id);
        if (cinema == null)
        {
            return new JsonResult(new GetCinemaByIdResponse
            {
                Status = "4001",
                Message = "该电影院不存在"
            });
        }

        return new JsonResult(new GetCinemaByIdResponse
        {
            Status = "10000",
            Message = "查询成功",
            Cinema = cinema
        });
    }

    /// <summary>
    /// 根据管理员id获得指定电影院信息
    /// </summary>
    /// <param name="id">管理员id</param>
    /// <returns>
    /// 返回电影院json
    /// </returns>
    [HttpGet("getCinemaByManagerId/{id}")]
    public async Task<IActionResult> GetCinemaByManagerId(string id)
    {
        var cinema = await _db.Cinemas.FirstOrDefaultAsync(c => c.ManagerId == id);
        if (cinema == null)
        {
            return new JsonResult(new GetCinemaByManagerIdResponse
            {
                Status = "4001",
                Message = "电影院不存在"
            });
        }

        return new JsonResult(new GetCinemaByManagerIdResponse
        {
            Status = "10000",
            Message = "查询成功",
            Cinema = cinema
        });
    }

    /// <summary>
    /// 根据电影院名称获得指定电影院信息,模糊查询
    /// </summary>
    /// <param name="name">电影院名称</param>
    /// <returns>
    /// 返回电影院json
    /// </returns>
    [HttpGet("getCinemaByName/{name}")]
    public async Task<IActionResult> GetCinemaByName(string name)
    {
        var cinemas = await _db.Cinemas.Where(c => c.Name.Contains(name)).ToListAsync();
        if (cinemas.Count == 0)
        {
            return new JsonResult(new GetCinemaByNameResponse
            {
                Status = "4001",
                Message = "电影院不存在"
            });
        }

        var response = new GetCinemaByNameResponse
        {
            Status = "10000",
            Message = "查询成功",
            Cinemas = cinemas
        };

        return new JsonResult(response);

    }




    /// <summary>
    /// 通过特点搜索到电影院列表
    /// </summary>
    /// <param name="feature"></param>
    /// <returns></returns>
    [HttpGet("getCinemaByFeature/{feature}")]
    public async Task<IActionResult> GetCinemaByFeature(string feature)
    {
        var cinemas = await _db.Cinemas.Where(c => c.Feature != null && c.Feature.Contains(feature)).ToListAsync();

        if (cinemas.Count == 0)
        {
            return new JsonResult(new GetCinemaByFeatureResponse
            {
                Status = "4001",
                Message = "电影院不存在"
            });
        }

        var response = new GetCinemaByFeatureResponse
        {
            Status = "10000",
            Message = "查询成功",
            Cinemas = cinemas.Select(c => c != null ? (Cinemas?)c : null).ToList()
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// 添加电影院
    /// </summary>
    /// <param name="request"></param>
    /// <returns>响应信息</returns>
    [HttpPut("add")]
    public async Task<IActionResult> AddCinema(AddCinemaRequest request)
    {
        try
        {
            var cinema = new Cinemas
            {
                CinemaId = request.CinemaId,
                Location = request.Location,
                Name = request.Name,
                ManagerId = request.ManagerId,
                CinemaImageUrl = request.CinemaImageUrl,
                Feature = request.Feature
            };
            await _db.AddAsync(cinema);
            await _db.SaveChangesAsync();
            return new JsonResult(new AddCinemaResponse
            {
                Status = "10000",
                Message = "添加成功",
                Cinema = cinema
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new AddCinemaResponse
            {
                Status = "10001",
                Message = "添加失败：" + ex.Message
            });
        }
    }
}