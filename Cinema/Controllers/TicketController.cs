using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema
{
    /// <summary>
    /// 电影票控制器类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        /// <summary>
        /// 获取所有电影票
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "CinemaAdmin")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 获取特定电影票
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 本接口包含鉴权：普通客户只能查看自己的电影票，管理和经理随意
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(Policy = "RegUser")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }
    }
}