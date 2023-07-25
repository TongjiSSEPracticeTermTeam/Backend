using Cinema.DTO.CinemaService;
using Cinema.DTO.StaffService;
using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StaffControllers;

/// <summary>
/// Ӱ��ʵ�����������
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class StaffController
{
    private readonly CinemaDb _db;

    /// <summary>
    /// ���������캯��
    /// </summary>
    /// <param name="db"></param>
    public StaffController(CinemaDb db)
    {
        _db = db;
    }

    /// <summary>
    /// �������Ӱ����Ϣ
    /// </summary>
    /// <returns>
    /// ����Ӱ��json�б�
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetStaffs()
    {
        return new JsonResult(await _db.Staffs.ToListAsync());
    }

    /// <summary>
    /// ����id���ָ��Ӱ����Ϣ
    /// </summary>
    /// <param name="id">Ӱ��id</param>
    /// <returns>
    /// ���ض�ӦӰ��json
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
                Message = "��Ӱ�˲�����"
            });
        }

        return new JsonResult(new GetStaffByIdResponse
        {
            Status = "10000",
            Message = "��ѯ�ɹ�",
            Staff = staff
        });
    }

    /// <summary>
    /// ����Ӱ�����ƻ��ָ��Ӱ����Ϣ
    /// </summary>
    /// <param name="name">Ӱ������</param>
    /// <returns>
    /// ����Ӱ��json
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
                Message = "Ӱ�˲�����"
            });
        }

        var response = new GetStaffByNameResponse
        {
            Status = "10000",
            Message = "��ѯ�ɹ�",
            Staffs = staffs!
        };

        return new JsonResult(response);
    }

    /// <summary>
    /// ͨ��IDɾ����ӦӰ��
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
                Message = "��Ӱ�˲�����"
            });
        }

        _db.Staffs.Remove(staff);
        await _db.SaveChangesAsync();

        var response = new DeleteStaffByIdResponse
        {
            Status = "10000",
            Message = "Ӱ��ɾ���ɹ�"
        };

        return new JsonResult(response);

    }

    /// <summary>
    /// ���Ӱ��
    /// </summary>
    /// <param name="request"></param>
    /// <returns>��Ӧ��Ϣ</returns>
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
                Message = "Ӱ����ӳɹ�",
                Staff = staff
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new AddStaffResponse
            {
                Status = "10001",
                Message = "Ӱ�����ʧ�ܣ�" + ex.Message
            });
        }
    }

    /// <summary>
    /// �޸�Ӱ��
    /// </summary>
    /// <param name="request"></param>
    /// <returns>��Ӧ��Ϣ</returns>
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
                Message = "Ӱ���޸ĳɹ�",
                Staff = staff
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new UpdateStaffResponse
            {
                Status = "10001",
                Message = "Ӱ���޸�ʧ�ܣ�" + ex.Message
            });
        }
    }


}
