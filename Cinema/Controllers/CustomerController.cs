using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController
{
    private readonly CinemaDb _db;

    public CustomerController(CinemaDb db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        return new JsonResult(await _db.Customers.ToListAsync());
    }
}