using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.StaffService;

/// <summary>
/// 添加影人响应体
/// </summary>
public class AddStaffResponse : IAPIResponse
{
    /// <summary>
    /// 影人实体
    /// </summary>
    [JsonPropertyName("staff")] public Staff? Staff { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// 修改影人响应体
/// </summary>
public class UpdateStaffResponse : IAPIResponse
{
    /// <summary>
    /// 影人实体
    /// </summary>
    [JsonPropertyName("staff")] public Staff? Staff { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// 通过id查询影人响应
/// </summary>
public class GetStaffByIdResponse : IAPIResponse
{
    /// <summary>
    /// 影人实体
    /// </summary>
    [JsonPropertyName("staff")] public Staff? Staff { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// 通过影人姓名查询影人响应
/// </summary>
public class GetStaffByNameResponse : IAPIResponse
{
    /// <summary>
    /// 查询得到的影人实体列表
    /// </summary>
    [JsonPropertyName("staffs")] public List<Staff?>? Staffs { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// 通过id删除对应影人
/// </summary>
public class DeleteStaffByIdResponse : IAPIResponse
{
    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}