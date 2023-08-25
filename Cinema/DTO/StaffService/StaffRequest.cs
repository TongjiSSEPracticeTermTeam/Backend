using System.Text.Json.Serialization;

namespace Cinema.DTO.StaffService;

/// <summary>
/// ���Ӱ�˵������嶨��
/// </summary>
public class AddStaffRequest
{
    /// <summary>
    /// Ӱ��id
    /// </summary>
    [JsonPropertyName("staff_id")] public string StaffId { get; set;} = String.Empty;
    
    /// <summary> 
    /// Ӱ������
    /// </summary>
    [JsonPropertyName("name")] public string Name { get; set;} = String.Empty;

    /// <summary>
    /// Ӱ���Ա�
    /// </summary>
    [JsonPropertyName("gender")] public int Gender { get; set;} = 0;

    /// <summary>
    /// Ӱ�˽���
    /// </summary>
    [JsonPropertyName("introduction")]  public string Introduction { get; set;} = String.Empty;

    /// <summary>
    /// ͼƬ����
    /// </summary>
    [JsonPropertyName("staff_img_url")] public string ImageUrl { get; set;} = String.Empty;
}

/// <summary>
/// ����Ӱ��������
/// </summary>
public class UpdateStaffRequest
{
    /// <summary>
    /// Ӱ��id
    /// </summary>
    [JsonPropertyName("staff_id")] public string StaffId { get; set; } = String.Empty;

    /// <summary> 
    /// Ӱ������
    /// </summary>
    [JsonPropertyName("name")] public string Name { get; set; } = String.Empty;

    /// <summary>
    /// Ӱ���Ա�
    /// </summary>
    [JsonPropertyName("gender")] public int Gender { get; set; } = 0;

    /// <summary>
    /// Ӱ�˽���
    /// </summary>
    [JsonPropertyName("introduction")] public string Introduction { get; set; } = String.Empty;

    /// <summary>
    /// ͼƬ����
    /// </summary>
    [JsonPropertyName("staff_img_url")] public string ImageUrl { get; set; } = String.Empty;
}
