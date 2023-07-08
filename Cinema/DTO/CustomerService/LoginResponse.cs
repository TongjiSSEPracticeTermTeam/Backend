using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.CustomerService;

/// <summary>
/// ��¼���
/// </summary>
public class LoginResponse : IAPIResponse
{
    /// <summary>
    /// �û�����
    /// </summary>
    [JsonPropertyName("userdata")] public Customer? UserData { get; set; }

    /// <summary>
    /// ���ƣ��÷�������HTTP����ͷ��Authorization: Bearer [����]��������Ҫ�����֤�ĵط����������ã�
    /// </summary>
    [JsonPropertyName("token")] public string? Token { get; set; }

    /// <summary>
    /// ״̬,10000Ϊ�ɹ�
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// ��Ϣ
    /// </summary>
    public string Message { get; set; } = String.Empty;
}