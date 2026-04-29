using System.Collections.Generic;
using Godot;

namespace CardDemo.Event
{
    public class EventCardData
    {
        public string CardName { get; set; }
        public int Count { get; set; } // 数量
    }

    public class EventData
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public List<EventCardData> Cards { get; set; } = new List<EventCardData>();
    }
}
