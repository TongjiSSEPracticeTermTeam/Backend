using Cinema.DTO;
using Cinema.DTO.BoxOfficeService;
using Cinema.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{ 
    /// <summary>
    /// 票房控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BoxOfficeController : ControllerBase
    {
        private readonly BoxOfficeServices _boxOfficeServices;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="boxOfficeServices"></param>
        public BoxOfficeController(BoxOfficeServices boxOfficeServices)
        {
            _boxOfficeServices = boxOfficeServices;
        }
    
        /// <summary>
        /// 获取当日票房数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(APIDataResponse<BoxOfficeData>))]
        public async Task<IAPIResponse> GetBoxOffice()
        {
            var boxOfficeData = await _boxOfficeServices.GetBoxOfficeData();
            return APIDataResponse<BoxOfficeData>.Success(boxOfficeData);
        }
    }
}
