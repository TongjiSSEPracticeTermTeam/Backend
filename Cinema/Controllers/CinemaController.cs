using Cinema.DTO.CinemaService;
using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CinemaController
{
    private readonly CinemaDb _db;

    public CinemaController(CinemaDb db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetCinemas()
    {
        return new JsonResult(await _db.Cinemas.ToListAsync());
    }

    [HttpPut]
    public async Task<IActionResult> AddCinema(AddCinemaRequest request)
    {
        var cinema = new Cinemas
        {
            CinemaId = Guid.NewGuid().ToString(),
            Location = request.Location,
            Name = request.Name,
            ManagerId = request.ManagerId,
            CinemaImageUrl = request.CinemaImageUrl,
            Feature = request.Feature
        };
        await _db.Cinemas.AddAsync(cinema);
        await _db.SaveChangesAsync();
        return new JsonResult(new AddCinemaResponse
        {
            Status = "10000",
            Message = "添加成功",
            Cinema = cinema
        });
    }

}