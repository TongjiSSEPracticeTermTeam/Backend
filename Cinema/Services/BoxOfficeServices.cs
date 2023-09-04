using Cinema.DTO.BoxOfficeService;
using Cinema.Helpers;
using StackExchange.Redis;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Cinema.Services
{
    /// <summary>
    /// 票房服务类
    /// </summary>
    public class BoxOfficeServices
    {
        private readonly IConnectionMultiplexer _redis;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="redis"></param>
        public BoxOfficeServices(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        /// <summary>
        /// 获取当日票房数据
        /// </summary>
        /// <returns></returns>
        public async Task<BoxOfficeData> GetBoxOfficeData()
        {
            var db = _redis.GetDatabase();
            string? res = db.StringGet("boxOffice");
            if(res!=null)
            {
                return JsonSerializer.Deserialize<BoxOfficeData>(res)!;
            }
            else
            {
                var httpClient = HttpHelper.GetHttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // 制作“2023-08-27"形式的日期字符串
                var date = DateTime.Now.ToString("yyyy-MM-dd");

                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("datatype","Day"),
                    new KeyValuePair<string, string>("date",date),
                    new KeyValuePair<string, string>("sdate",date),
                    new KeyValuePair<string, string>("edate",date),
                    new KeyValuePair<string, string>("bserviceprice","1"),
                    new KeyValuePair<string, string>("columnslist","100,102,103,119,105,107,109,106,112,129,142,143,163,164,165"),
                    new KeyValuePair<string, string>("pageindex","1"),
                    new KeyValuePair<string, string>("pagesize","20"),
                    new KeyValuePair<string, string>("order","103"),
                    new KeyValuePair<string, string>("ordertype","desc"),
                });

                var response = await httpClient.PostAsync("https://ys.endata.cn/enlib-api/api/movie/getMovie_BoxOffice_Day_List.do", formData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var rawBoxOfficeData = QuickType.BoxOfficePerDay.FromJson(result)!;

                    var boxOfficeData = new BoxOfficeData();
                    boxOfficeData.GeneralData.UpdateTime = rawBoxOfficeData.Data.Table0[0].UpTime.ToString("T");
                    boxOfficeData.GeneralData.SumBoxOffice = rawBoxOfficeData.Data.Table0[0].SumBoxOffice;
                    boxOfficeData.GeneralData.SumShowCount = rawBoxOfficeData.Data.Table0[0].SumShowCount;
                    boxOfficeData.GeneralData.SumAudienceCount = rawBoxOfficeData.Data.Table0[0].SumAudienceCount;

                    boxOfficeData.MovieData = rawBoxOfficeData.Data.Table1.Select(t =>
                    new MovieData{
                        MovieName = t.MovieName,
                        Irank = t.Irank,
                        EnMovieName = t.EnMovieName,
                        BoxOffice = t.BoxOffice,
                        ShowCount = t.ShowCount,
                        AudienceCount = t.AudienceCount,
                    }).ToList();

                    var boxOfficeDataJSON = JsonSerializer.Serialize(boxOfficeData);

                    db.StringSet("boxOffice", boxOfficeDataJSON, TimeSpan.FromHours(1));
                    return boxOfficeData;
                }
                else
                {
                    throw new Exception("获取票房数据失败");
                }
            }
        }
    }
}
