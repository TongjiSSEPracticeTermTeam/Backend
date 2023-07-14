using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
using Cinema.DTO.MovieService;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers;

/// <summary>
/// 电影实体类控制器类
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MovieController
{
    private readonly CinemaDb _db;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="="db"><param>
    public MovieController(CinemaDb db)
    {
        _db = db;
    }

    /// <summary>
    ///根据电影id获取电影演员信息
    /// </summary>
    /// <returns>
    /// 返回电影演员json列表
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetActorList(String id)
    {
        var tmp_movie = await _db.Movies.Where(t => 
        (t.MovieId == id)).FirstOrDefaultAsync()!;
        var tmp_actors = tmp_movie!.Acts;

        return new JsonResult(new GetActorsByMovieIdResponse
        {
            Status = "10000",
            Message = "查询成功",
            Act = tmp_actors,
        });

        ///return new JsonResult(tmp_actors.ToList());
    }

}
