namespace Cinema.Helpers
{
    /// <summary>
    /// HTTP请求的相关工具类
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// 获取HttpClient实例
        /// </summary>
        /// <returns></returns>
        public static HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(10);
            return client;
        }
    }
}
