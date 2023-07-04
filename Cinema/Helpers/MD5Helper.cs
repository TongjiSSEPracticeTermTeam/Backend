using System.Security.Cryptography;
using System.Text;

namespace Cinema.Helpers;

public class Md5Helper
{
    /// <summary>
    /// 计算一个字符串的MD5
    /// </summary>
    /// <param name="input">待计算的字符串</param>
    /// <returns>结果</returns>
    public static string CalculateMd5Hash(string input)
    {
        using var md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        StringBuilder sb = new();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2")); // 将每个字节转换为16进制字符串
        }
        return sb.ToString();
    }
}