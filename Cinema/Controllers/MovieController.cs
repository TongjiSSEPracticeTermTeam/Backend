using Cinema.DTO.MovieService;
using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
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
    /// 控制器构造函数
    /// </summary>
    /// <param name="db"></param>
    public MovieController(CinemaDb db)
    {
        _db = db;
    }

    /// <summary>
    /// 获得所有电影院信息
    /// </summary>
    /// <returns>
    /// 返回电影json列表
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetMovie()
    {
        return new JsonResult(await _db.Movies.ToListAsync());
    }

    /// <summary>
    /// 根据id获得指定电影信息
    /// </summary>
    /// <param name="id">电影id</param>
    /// <returns>
    /// 返回电影json
    /// </returns>
    [HttpGet("getMovieById/{id}")]
    public async Task<IActionResult> GetMovieById(string id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie == null)
        {
            return new JsonResult(new GetMovieByIdResponse
            {
                Status = "4001",
                Message = "该电影不存在"
            });
        }

        return new JsonResult(new GetMovieByIdResponse
        {
            Status = "10000",
            Message = "查询成功",
            Movie = movie
        });
    }

    /// <summary>
    /// 根据电影名称获得指定电影信息,模糊查询
    /// </summary>
    /// <param name="name">电影名称</param>
    /// <returns>
    /// 返回电影json
    /// </returns>
    [HttpGet("getMovieByName/{name}")]
    public async Task<IActionResult> GetMovieByName(string name)
    {
        var movies = await _db.Movies.Where(c => c.Name.Contains(name)).ToListAsync();
        if (movies == null || movies!.Count == 0)
        {
            return new JsonResult(new GetMovieByNameResponse
            {
                Status = "4001",
                Message = "电影不存在"
            });
        }

        var response = new GetMovieByNameResponse
        {
            Status = "10000",
            Message = "查询成功",
            Movies = movies!
        };

        return new JsonResult(response);

    }

    /// <summary>
    /// 通过特点搜索到电影院列表
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    [HttpGet("getMovieByTags/{tags}")]
    public async Task<IActionResult> GetMovieByTags(string tags)
    {
        var movies = await _db.Movies.Where(c => c.Tags != null && c.Tags.Contains(tags)).ToListAsync();

        if (movies.Count == 0)
        {
            return new JsonResult(new GetMovieByTagsResponse
            {
                Status = "4001",
                Message = "电影不存在"
            });
        }

        var response = new GetMovieByTagsResponse
        {
            Status = "10000",
            Message = "查询成功",
            Movies = movies.Select(c => c != null ? (Movie?)c : null).ToList()
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// 通过ID删除对应电影
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("deleteMovieById/{id}")]
    public async Task<IActionResult> DeleteMovieById(string id)
    {
        var movie = await _db.Movies.FindAsync(id);

        if (movie == null)
        {
            return new JsonResult(new DeleteMovieByIdResponse
            {
                Status = "4001",
                Message = "该电影不存在"
            });
        }

        _db.Movies.Remove(movie);
        await _db.SaveChangesAsync();

        var response = new DeleteMovieByIdResponse
        {
            Status = "10000",
            Message = "删除成功"
        };

        return new JsonResult(response);

    }

    /// <summary>
    /// 添加电影
    /// </summary>
    /// <param name="request"></param>
    /// <returns>响应信息</returns>
    [HttpPost("add")]
    public async Task<IActionResult> AddMovie(AddMovieRequest request)
    {
        try
        {
            var movie = new Movie
            {
                MovieId = request.MovieId,
                Name = request.Name,
                Duration = request.Duration,
                Instruction = request.Instruction,
                PostUrl = request.PostUrl,
                Tags = request.Tags,
                ReleaseDate = DateTime.ParseExact(request.ReleaseDate, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture),
                RemovalDate = DateTime.ParseExact(request.RemovalDate, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture)
            };
            await _db.AddAsync(movie);
            await _db.SaveChangesAsync();
            return new JsonResult(new AddMovieResponse
            {
                Status = "10000",
                Message = "电影添加成功",
                Movie = movie
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new AddMovieResponse
            {
                Status = "10001",
                Message = "电影添加失败：" + ex.Message
            });
        }
    }
}

