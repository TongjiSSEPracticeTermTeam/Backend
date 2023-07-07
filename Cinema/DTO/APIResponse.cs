using System.Text.Json.Serialization;

namespace Cinema.DTO;

/// <summary>
/// API�ӿڷ��صĻ���
/// </summary>
public interface IAPIResponse
{
    /// <summary>
    /// Ϊ10000ʱ��ʾ�ɹ�������Ϊʧ��
    /// </summary>
    [JsonPropertyName("status")] public string Status { get; set; }

    /// <summary>
    ///���ظ�ǰ�˵���Ϣ
    /// </summary>
    [JsonPropertyName("message")] public string Message { get; set; }
}