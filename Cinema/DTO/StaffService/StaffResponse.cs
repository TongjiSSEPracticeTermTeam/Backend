using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.StaffService;

/// <summary>
/// ���Ӱ����Ӧ��
/// </summary>
public class AddStaffResponse : IAPIResponse
{
    /// <summary>
    /// Ӱ��ʵ��
    /// </summary>
    [JsonPropertyName("staff")] public Staff? Staff { get; set; }

    /// <summary>
    /// ��Ӧ״̬
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// ��Ӧ��Ϣ
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// �޸�Ӱ����Ӧ��
/// </summary>
public class UpdateStaffResponse : IAPIResponse
{
    /// <summary>
    /// Ӱ��ʵ��
    /// </summary>
    [JsonPropertyName("staff")] public Staff? Staff { get; set; }

    /// <summary>
    /// ��Ӧ״̬
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// ��Ӧ��Ϣ
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// ͨ��id��ѯӰ����Ӧ
/// </summary>
public class GetStaffByIdResponse : IAPIResponse
{
    /// <summary>
    /// Ӱ��ʵ��
    /// </summary>
    [JsonPropertyName("staff")] public Staff? Staff { get; set; }

    /// <summary>
    /// ��Ӧ״̬
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// ��Ӧ��Ϣ
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// ͨ��Ӱ��������ѯӰ����Ӧ
/// </summary>
public class GetStaffByNameResponse : IAPIResponse
{
    /// <summary>
    /// ��ѯ�õ���Ӱ��ʵ���б�
    /// </summary>
    [JsonPropertyName("staffs")] public List<Staff?>? Staffs { get; set; }

    /// <summary>
    /// ��Ӧ״̬
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// ��Ӧ��Ϣ
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// ͨ��idɾ����ӦӰ��
/// </summary>
public class DeleteStaffByIdResponse : IAPIResponse
{
    /// <summary>
    /// ��Ӧ״̬
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// ��Ӧ��Ϣ
    /// </summary>
    public string Message { get; set; } = String.Empty;
}