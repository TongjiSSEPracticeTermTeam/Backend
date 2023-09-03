using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;
using Cinema.DTO.StaffService;

namespace Cinema.DTO.MoviesService
{
    /// <summary>
    /// 电影的DTO
    /// </summary>
    public class MovieDTO
    {
        /// <summary>
        /// 电影ID
        /// </summary>
        [Required]
        [JsonPropertyName("movieId")]
        public string MovieId { get; set; } = "";

        /// <summary>
        /// 名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 时长
        /// </summary>
        [JsonPropertyName("duration")]
        public string Duration { get; set; } = "";

        /// <summary>
        /// 介绍
        /// </summary>
        [JsonPropertyName("instruction")]
        public string? Instruction { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        [JsonPropertyName("score")]
        public float? Score { get; set; }

        /// <summary>
        /// 海报图片
        /// </summary>
        [JsonPropertyName("postUrl")]
        public string? PostUrl { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [JsonPropertyName("tags")]
        public string? Tags { get; set; }

        /// <summary>
        /// 上映日期
        /// </summary>
        [JsonPropertyName("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// 到期日期
        /// </summary>
        [JsonPropertyName("removalDate")]
        public DateTime RemovalDate { get; set; }

        /// <summary>
        /// 导演的ID和姓名
        /// </summary>
        [JsonPropertyName("director")]
        public EStaff? Director { get; set; }

        /// <summary>
        /// 演员的ID和姓名
        /// </summary>
        [JsonPropertyName("actors")]
        public List<EStaff>? Actors { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public MovieDTO()
        {
        }

        /// <summary>
        /// 实体构造
        /// </summary>
        /// <param name="entity"></param>
        public MovieDTO(Movie entity)
        {
            MovieId = entity.MovieId;
            Name = entity.Name;
            Duration = entity.Duration;
            Instruction = entity.Instruction;
            Score = entity.Score;
            PostUrl = entity.PostUrl;
            Tags = entity.Tags;
            ReleaseDate = entity.ReleaseDate;
            RemovalDate = entity.RemovalDate;

            foreach (var act in entity.Acts)
            {
                //Console.WriteLine(act.StaffId + " " + act.MovieId + " " + act.Role);

                if (act.Role == "1")
                {
                    Director = new EStaff(act.Staff);
                }
                else if (act.Role == "0")
                {
                    if (Actors == null)
                        Actors = new List<EStaff>();

                    Actors.Add(new EStaff(act.Staff));
                }
            }
        }

    }
}
