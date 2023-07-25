using Cinema.DTO.CinemaService;
using Cinema.DTO.StaffService;
using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StaffControllers;

/// <summary>
/// 影人实体类控制器类
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class StaffController
{
    private readonly CinemaDb _db;

    /// <summary>
    /// 控制器构造函数
    /// </summary>
    /// <param name="db"></param>
    public StaffController(CinemaDb db)
    {
        _db = db;
    }

    /// <summary>
    /// 获得所有影人信息
    /// </summary>
    /// <returns>
    /// 返回影人json列表
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetStaffs()
    {
        return new JsonResult(await _db.Staffs.ToListAsync());
    }

    /// <summary>
    /// 根据id获得指定影人信息
    /// </summary>
    /// <param name="id">影人id</param>
    /// <returns>
    /// 返回对应影人json
    /// </returns>
    [HttpGet("getStaffById/{id}")]
    public async Task<IActionResult> GetStaffById([FromRoute] string id)
    {
        var staff = await _db.Staffs.FindAsync(id);
        if(staff == null) 
        {
            return new JsonResult(new GetStaffByIdResponse
            {
                Status = "4001",
                Message = "该影人不存在"
            });
        }

        return new JsonResult(new GetStaffByIdResponse
        {
            Status = "10000",
            Message = "查询成功",
            Staff = staff
        });
    }

    /// <summary>
    /// 根据影人名称获得指定影人信息
    /// </summary>
    /// <param name="name">影人名称</param>
    /// <returns>
    /// 返回影人json
    /// </returns>
    [HttpGet("getStaffByName/{name}")]
    public async Task<IActionResult> GetStaffByName([FromRoute] string name)
    {
        var staffs = await _db.Staffs.Where(s => s.Name.Contains(name)).ToListAsync();
        if (staffs == null || staffs!.Count == 0)
        {
            return new JsonResult(new GetStaffByNameResponse
            {
                Status = "4001",
                Message = "影人不存在"
            });
        }

        var response = new GetStaffByNameResponse
        {
            Status = "10000",
            Message = "查询成功",
            Staffs = staffs!
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// 通过ID删除对应影人
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("deleteStaffById/{id}")]
    public async Task<IActionResult> DeleteStaffById([FromRoute] string id)
    {
        var staff = await _db.Staffs.FindAsync(id);

        if (staff == null)
        {
            return new JsonResult(new DeleteStaffByIdResponse
            {
                Status = "4001",
                Message = "该影人不存在"
            });
        }

        _db.Staffs.Remove(staff);
        await _db.SaveChangesAsync();

        var response = new DeleteStaffByIdResponse
        {
            Status = "10000",
            Message = "影人删除成功"
        };

        return new JsonResult(response);

    }

    /// <summary>
    /// 添加影人
    /// </summary>
    /// <param name="request"></param>
    /// <returns>响应信息</returns>
    [HttpPost("add")]
    public async Task<IActionResult> AddStaff([FromBody] AddStaffRequest request)
    {
        try
        {
            var staff = new Staff
            {
                StaffId = request.StaffId,
                Name = request.Name,
                Gender = (Gender_)request.Gender,
                Introduction= request.Introduction,
                ImageUrl = request.ImageUrl,
            };
            await _db.AddAsync(staff);
            await _db.SaveChangesAsync();
            return new JsonResult(new AddStaffResponse
            {
                Status = "10000",
                Message = "影人添加成功",
                Staff = staff
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new AddStaffResponse
            {
                Status = "10001",
                Message = "影人添加失败：" + ex.Message
            });
        }
    }

    /// <summary>
    /// 修改影人
    /// </summary>
    /// <param name="request"></param>
    /// <returns>响应信息</returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateStaff([FromBody] UpdateStaffRequest request)
    {
        try
        {
            var staff = new Staff
            {
                StaffId = request.StaffId,
                Name = request.Name,
                Gender = (Gender_)request.Gender,
                Introduction = request.Introduction,
                ImageUrl = request.ImageUrl,
            };
             _db.Update(staff);
            await _db.SaveChangesAsync();
            return new JsonResult(new UpdateStaffResponse
            {
                Status = "10000",
                Message = "影人修改成功",
                Staff = staff
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new UpdateStaffResponse
            {
                Status = "10001",
                Message = "影人修改失败：" + ex.Message
            });
        }
    }


}
