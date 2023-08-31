using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.ManagerService
{
    /// <summary>
    /// ӰԺ����Ա�İװ幹������Ŀǰ�����ˣ�
    /// </summary>
    public class ManagerCreator
    {
        [Required]
        [JsonPropertyName("managerName")]
        public string ManagerName { get; set; } = String.Empty;

        [Required]
        [JsonPropertyName("managerPassword")]
        public string ManagerPassword { get; set; } = String.Empty;

        [Required]
        [JsonPropertyName("managerEmail")]
        public string ManagerEmail { get; set; } = String.Empty;
    }
}
