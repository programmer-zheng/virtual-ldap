namespace DingDingSync.Application.Jobs
{
    public class DingDingCallbackBackgroundJobArgs
    {
        /// <summary>
        /// 钉钉消息原文
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; }
    }
}