using System.Text.Json.Serialization;

namespace Cinema.DTO;

/// <summary>
/// API返回数据，带有一个Data
/// </summary>
public class APIDataResponse<T> : APIResponse
{
    /// <summary>
    /// 返回的Data
    /// </summary>
    [JsonPropertyName("data")] public T? Data { get; set; } = default;

    /// <summary>
    /// 构造一个成功的API Data结果并返回
    /// </summary>
    /// <param name="Data"></param>
    /// <returns></returns>
    public static APIDataResponse<T> Success(T Data)
    {
        return new APIDataResponse<T>
        {
            Status = "10000",
            Message = "成功",
            Data = Data
        };
    }

    /// <summary>
    /// 构造一个失败的API Data结果返回
    /// </summary>
    /// <param name="Status"></param>
    /// <param name="Message"></param>
    /// <returns></returns>
    //public static APIDataResponse<T> Failaure(string Status, string Message)
    //{
    //    return new APIDataResponse<T>
    //    {
    //        Status = Status,
    //        Message = Message
    //    };
    //}
}