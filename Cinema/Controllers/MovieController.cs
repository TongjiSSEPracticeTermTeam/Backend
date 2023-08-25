using Cinema.DTO.CinemaService;
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
    /// 获得所有电影信息
    /// </summary>
    /// <returns>
    /// 返回电影json列表
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetMovie()
    {
        var movies = await _db.Movies.ToListAsync();
        movies = movies.OrderBy(movie => movie.MovieId).ToList();
        //return new JsonResult(await _db.Movies.ToListAsync());
        return new JsonResult(movies);
    }

    /// <summary>
    /// 根据id获得指定电影信息
    /// </summary>
    /// <param name="id">电影id</param>
    /// <returns>
    /// 返回电影json
    /// </returns>
    [HttpGet("getMovieById/{id}")]
    public async Task<IActionResult> GetMovieById([FromRoute] string id)
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
            Movie = (Movie)movie
        });
    }

    /// <summary>
    /// 根据id获取指定电影和相关影人
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// 返回电影、导演和主演json
    /// </returns>
    [HttpGet("getMovieByIdwithStaff/{id}")]
    public async Task<IActionResult> GetMovieByIdwithStaff([FromRoute] string id)
    {
        var movie = await _db.Movies.FindAsync(id);
        //var movie = await _db.Movies
        //    .Include(movie => movie.Acts)
        //    .ThenInclude(act => act.Staff)
        //    .FirstOrDefaultAsync(movie => movie.MovieId == id);

        if (movie == null)
        {
            return new JsonResult(new GetMovieByIdwithStaffResponse
            {
                Status = "4001",
                Message = "该电影不存在"
            });
        }

        //添加影人
        var acts = await _db.Acts.Where(act => act.MovieId == movie.MovieId).Include(act => act.Staff).ToListAsync();
        var actor = new List<Staff?>();
        var director = new Staff();

        if(acts !=  null && acts!.Count > 0)
        {
            foreach (var act in acts)
            {
                if (act.Role.Equals("0"))
                    actor.Add(act.Staff);
                else
                    director = act.Staff;
            }
        }

        return new JsonResult(new GetMovieByIdwithStaffResponse
        {
            Status = "10000",
            Message = "查询成功",
            Movie = (Movie)movie,
            Director = director.StaffId == String.Empty? null : director,
            Actors  = actor
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
    public async Task<IActionResult> GetMovieByName([FromRoute] string name)
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
    public async Task<IActionResult> GetMovieByTags([FromRoute] string tags)
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
    public async Task<IActionResult> DeleteMovieById([FromRoute] string id)
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
    public async Task<IActionResult> AddMovie([FromBody] AddMovieRequest request)
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

    /// <summary>
    /// 修改电影
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateMovie([FromBody] UpdateMovieRequest request)
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

            var preActs = await _db.Acts.Where(a => a.MovieId == movie.MovieId).ToListAsync();
            var directorId = request.DirectorId;
            var actorIds = request.ActorIds;
            var newActs = new List<Act>();

            if(preActs.Count != 0)
            {
                _db.Acts.RemoveRange(preActs);
                _db.SaveChanges();
            }
            if (directorId != "-1")
            {
                newActs.Add(new Act
                {
                    StaffId = directorId,
                    MovieId = movie.MovieId,
                    Role = "1"
                });
            }
            foreach (var id in actorIds)
            {
                newActs.Add(new Act
                {
                    StaffId = id,
                    MovieId = movie.MovieId,
                    Role = "0"
                });
            }
            if(newActs.Count != 0)
            {
                await _db.Acts.AddRangeAsync(newActs);
            }

            _db.Movies.Update(movie);
            await _db.SaveChangesAsync();

            return new JsonResult(new UpdateMovieResponse
            {
                Status = "10000",
                Message = "电影修改成功",
                Movie = movie
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new UpdateMovieResponse
            {
                Status = "10001",
                Message = "电影修改失败：" + ex.Message
            });
        }
    }
}