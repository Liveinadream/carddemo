using System.Collections.Generic;
using Godot;

namespace CardDemo.Map
{
    public class MapNodeData
    {
        public int Id { get; set; }
        public string NodeType { get; set; } // "Battle", "Shop", "Event", "Exit"
        public Vector2 Position { get; set; }
        public List<int> ConnectedNodes { get; set; } = new List<int>();
        public bool IsVisited { get; set; }
        public string EventId { get; set; } // 关联的具体事件 ID
    }

    public class MapSegmentData
    {
        public int SegmentId { get; set; }
        public string SegmentName { get; set; }
        public List<MapNodeData> Nodes { get; set; } = new List<MapNodeData>();
        public int NextSegmentId { get; set; } // 出口通往的下一个片段
    }
}
