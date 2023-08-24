using COSSTS;
using COSXML;
using COSXML.Auth;
using COSXML.Transfer;
using Newtonsoft.Json.Linq;

namespace Cinema.Services
{
    class CinemaDbQCosCredentialProvider: DefaultSessionQCloudCredentialProvider
    {
        const string bucket = "cinemadb-1305284863";
        const string region = "ap-shanghai";

        public CinemaDbQCosCredentialProvider() : base(null, null, 0L, null)
        {
        }

        struct TempCredential
        {
            public string Token;
            public string TmpSecretId;
            public string TmpSecretKey;
            public long ExpiredTime;
            public long StartTime;
        }

        /// <summary>
        /// 生成临时凭据
        /// </summary>
        static TempCredential GetCredential()
        {
            const string secretId = "AKIDdHwdHtV3Vr1wc4IRO8euvhEVcWBbmiQs";
            string secretKey = Environment.GetEnvironmentVariable("TCCLOUD_SECRETKEY")!;
            const string allowPrefix = "userdata/*";
            string[] allowActions = new string[] {
                "name/cos:PutObject",
                "name/cos:PostObject",
                "name/cos:InitiateMultipartUpload",
                "name/cos:ListMultipartUploads",
                "name/cos:ListParts",
                "name/cos:UploadPart",
                "name/cos:CompleteMultipartUpload"
            };

            Dictionary<string, object> values = new()
            {
                { "secretId", secretId },
                { "secretKey", secretKey },
                { "bucket", bucket },
                { "region", region },
                { "allowPrefix", allowPrefix },
                { "allowActions", allowActions },
            };

            Dictionary<string, object> credential = STSClient.genCredential(values);

            if (credential.GetValueOrDefault("Credentials") is not JObject credentialDetailJObj 
                || credential.GetValueOrDefault("ExpiredTime") is not long expiredTime 
                || credential.GetValueOrDefault("StartTime") is not int startTime)
            {
                throw new Exception("获取临时凭据失败");
            }

            var credentialDetail = credentialDetailJObj.ToObject<Dictionary<string, object>>();
            if (credentialDetail.GetValueOrDefault("Token") is not string token || credentialDetail.GetValueOrDefault("TmpSecretId") is not string tmpSecretId || credentialDetail.GetValueOrDefault("TmpSecretKey") is not string tmpSecretKey)
            {
                throw new Exception("获取临时凭据失败");
            }

            return new TempCredential
            {
                Token = token,
                TmpSecretId = tmpSecretId,
                TmpSecretKey = tmpSecretKey,
                ExpiredTime = expiredTime,
                StartTime = startTime
            };
        }

        public override void Refresh()
        {
            var credential = GetCredential();

            SetQCloudCredential(credential.TmpSecretId, credential.TmpSecretKey,
                String.Format("{0};{1}", credential.StartTime, credential.ExpiredTime), credential.Token);

            base.Refresh();
        }
    }

    /// <summary>
    /// 腾讯云COS服务类
    /// </summary>
    public class QCosSrvice
    {
        const string bucket = "cinemadb-1305284863";
        const string region = "ap-shanghai";

        private CosXml cosXml;

        /// <summary>
        /// 默认初始化
        /// </summary>
        public QCosSrvice()
        {
            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetConnectionTimeoutMs(60000)  //设置连接超时时间，单位毫秒，默认45000ms
                .SetReadWriteTimeoutMs(40000)  //设置读写超时时间，单位毫秒，默认45000ms
                .IsHttps(true)  //设置默认 HTTPS 请求
                .SetRegion(region)  //设置腾讯云账户的账户标识 APPID 及地域信息 Region
                .SetDebugLog(true)  //显示日志
                .Build();  //创建 CosXmlConfig 对象
            cosXml = new CosXmlServer(config, new CinemaDbQCosCredentialProvider());
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="key">文件Key</param>
        /// <param name="localPath">本地路径</param>
        /// <returns></returns>
        public async Task<string> UploadFile(string key, string localPath)
        {
            var transferConfig = new TransferConfig();
            var transferManager = new TransferManager(cosXml, transferConfig);
            
            var uploadTask = new COSXMLUploadTask(bucket, key);
            uploadTask.SetSrcPath(localPath);

            try
            {
                var result = await transferManager.UploadAsync(uploadTask);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            var uriBuilder = new UriBuilder()
            {
                Scheme = "https",
                Host = "cinemadb-1305284863.cos.accelerate.myqcloud.com",
                Path = key
            };
            return uriBuilder.ToString();
        }
    }
}
