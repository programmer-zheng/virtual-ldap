using System;

namespace DingDingSync.Application.IKuai
{
    public class IKuaiException : Exception
    {
        public IKuaiException(int errorCode, string errorMsg, string deviceId = "")
        {
            DeviceId = deviceId;
            ErrorCode = errorCode;
            ErrorMsg = errorMsg;
        }

        public string DeviceId { get; set; }

        public override string Message
        {
            get
            {
                string v = !string.IsNullOrWhiteSpace(DeviceId) ? $"设备ID{DeviceId}，" : string.Empty;
                return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 调用爱快API时发生异常，{v}状态码：{ErrorCode}，说明：{ErrorMsg}。";
            }
        }

        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }
    }
}