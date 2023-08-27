using Cinema.DTO;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Cinema.Services
{
    /// <summary>
    /// 图片服务
    /// </summary>
    public class ImageService
    {
        /// <summary>
        /// 预处理外部上传的图片
        /// </summary>
        /// <param name="fileName">图片临时保存的地址</param>
        /// <returns></returns>
        public static bool PreprocessUploadedImage(string fileName)
        {
            try
            {
                using var image = Image.Load(fileName);
                image.Save(fileName, new JpegEncoder()
                {
                    Quality = 80
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 提供上传图片的接口
        /// </summary>
        /// <param name="prefix">前缀，例如“cinema/”，这样表示保存的图片是影院的照片</param>
        /// <param name="imageName">【可选】意向图片名，留空使用GUID</param>
        /// <param name="file">来自HTTP请求的文件</param>
        /// <param name="qCosSrvice">当前控制器类的QCosService单例</param>
        /// <returns></returns>
        public static async Task<IAPIResponse> UploadImage(string prefix, string? imageName, IFormFile file, QCosSrvice qCosSrvice)
        {
            if (file == null || file.Length == 0)
            {
                return APIResponse.Failaure("4002", "未上传文件");
            }

            // Save to temp file
            var tempFileName = Path.GetTempFileName();
            await using (var tempFile = new FileStream(tempFileName, FileMode.Create))
            {
                await file.CopyToAsync(tempFile);
            }

            var isValidImage = ImageService.PreprocessUploadedImage(tempFileName);
            if (!isValidImage)
            {
                return APIResponse.Failaure("4000", "不是有效的图片");
            }

            var imagePath = $"userdata/{prefix}/{imageName ?? Guid.NewGuid().ToString()}.jpg";
            string? imageUrl;
            try
            {
                imageUrl = await qCosSrvice.UploadFile(imagePath, tempFileName);
            }
            catch
            {
                System.IO.File.Delete(tempFileName);
                return APIResponse.Failaure("4000", "上传图片失败（内部错误）");
            }
            System.IO.File.Delete(tempFileName);
            return APIDataResponse<string>.BuildAPIDataSuccessResponse(imageUrl);
        }
    }
}
