using System.Collections.Generic;

namespace DingDingSync.Application.Jobs.EventInfo
{
    public class EventBaseInfo<T>
    {
        public string CorpId { get; set; }

        public string EventType { get; set; }

        public virtual List<T> ID { get; set; }

        public string TimeStamp { get; set; }
    }
}
