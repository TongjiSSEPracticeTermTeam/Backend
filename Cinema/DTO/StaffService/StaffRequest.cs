using System.Text.Json.Serialization;

namespace Cinema.DTO.StaffService;

/// <summary>
/// 添加影人的请求体定义
/// </summary>
public class AddStaffRequest
{
    /// <summary>
    /// 影人id
    /// </summary>
    [JsonPropertyName("staff_id")] public string StaffId { get; set;} = String.Empty;
    
    /// <summary> 
    /// 影人名字
    /// </summary>
    [JsonPropertyName("name")] public string Name { get; set;} = String.Empty;

    /// <summary>
    /// 影人性别
    /// </summary>
    [JsonPropertyName("gender")] public int Gender { get; set;} = 0;

    /// <summary>
    /// 影人介绍
    /// </summary>
    [JsonPropertyName("introduction")]  public string Introduction { get; set;} = String.Empty;

    /// <summary>
    /// 图片链接
    /// </summary>
    [JsonPropertyName("staff_img_url")] public string ImageUrl { get; set;} = String.Empty;
}

/// <summary>
/// 更新影人请求体
/// </summary>
public class UpdateStaffRequest
{
    /// <summary>
    /// 影人id
    /// </summary>
    [JsonPropertyName("staff_id")] public string StaffId { get; set; } = String.Empty;

    /// <summary> 
    /// 影人名字
    /// </summary>
    [JsonPropertyName("name")] public string Name { get; set; } = String.Empty;

    /// <summary>
    /// 影人性别
    /// </summary>
    [JsonPropertyName("gender")] public int Gender { get; set; } = 0;

    /// <summary>
    /// 影人介绍
    /// </summary>
    [JsonPropertyName("introduction")] public string Introduction { get; set; } = String.Empty;

    /// <summary>
    /// 图片链接
    /// </summary>
    [JsonPropertyName("staff_img_url")] public string ImageUrl { get; set; } = String.Empty;
}
