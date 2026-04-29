// MapNode.cs - 定义地图节点的数据
using System.Collections.Generic;
using Godot;

public class MapNode
{
    public int Id;
    public string NodeType; // "Battle", "Shop", "Event", etc.
    public Vector2 Position; // 在地图界面上的坐标
    public List<int> ConnectedNodes = new List<int>(); // 连接的下一个节点 ID
    public bool IsVisited = false;
    public bool IsSelectable = false;
}