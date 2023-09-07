using Cinema.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.IO;
using Cinema.DTO.StaffService;
using TencentCloud.Pds.V20210701.Models;
using TencentCloud.Ssl.V20191205.Models;

namespace Cinema.DTO.CustomerService
{
    /// <summary>
    /// 修改密码的DTO
    /// </summary>
    public class ChangePwdDTO
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; } = "";

        /// <summary>
        /// 旧密码
        /// </summary>
        [JsonPropertyName("oldPwd")]
        public string OldPwd { get; set; } = "";

        /// <summary>
        /// 新密码
        /// </summary>
        [JsonPropertyName("newPwd")]
        public string NewPwd { get; set; } = "";

        /// <summary>
        /// 默认构造
        /// </summary>
        public ChangePwdDTO()
        {
        }

        /// <summary>
        /// 实体构造
        /// </summary>
        /// <param name="entity"></param>
        public ChangePwdDTO(ChangePwdInput input)
        {
            CustomerId = input.customerId;
            OldPwd = input.oldPwd;
            NewPwd = input.newPwd;
        }

    }

    public struct ChangePwdInput
    {
        public string customerId { get; set; }
        public string oldPwd { get; set; }
        public string newPwd { get; set; }
    }
}
