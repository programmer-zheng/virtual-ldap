using System.Collections.Generic;

namespace DingDingSync.Application.Jobs.EventInfo
{
    public class DingDingEventBaseInfo<T>
    {
        public string CorpId { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; }

        public virtual List<T> ID { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string TimeStamp { get; set; }
    }
}