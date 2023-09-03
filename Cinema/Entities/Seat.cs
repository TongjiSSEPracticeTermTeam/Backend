using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    /// <summary>
    /// 座位信息（子类）
    /// </summary>
    public class Seat
    {
        /// <summary>
        /// 行数
        /// </summary>
        [JsonPropertyName("rows")]
        public int Rows { get; set; }

        /// <summary>
        /// 每行座位数
        /// </summary>
        [JsonPropertyName("cols")]
        public List<int> Cols { get; set; } = new List<int>();
    }
}
