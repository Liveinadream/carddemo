using Godot;
using System.Collections.Generic;
using System.Linq;
using CardDemo.Map;

namespace CardDemo.Map
{
    public partial class MapScreen : Control
    {
        private PackedScene _nodeScene;
        private Control _nodeLayer;
        private Control _pathLayer;
        private Control _playerIcon;
        private Camera2D _camera;

    public override void _Ready()
    {
        base._Ready();
        _nodeScene = GD.Load<PackedScene>("res://Scenes/Map/map_node.tscn");
        _nodeLayer = GetNode<Control>("MapContainer/NodeLayer");
        _pathLayer = GetNode<Control>("MapContainer/PathLayer");
        _playerIcon = GetNode<Control>("MapContainer/PlayerIcon");
        _camera = GetNode<Camera2D>("Camera2D");

        // 订阅事件
        GameEvents.OnMapSegmentLoaded += UpdateMapUI;
        GameEvents.OnMapNodeSelected += OnMapNodeSelected;

        // 初始化加载第一个片段
        MapManager.Instance.LoadSegment(1);
        // UpdateMapUI(); // 首次加载会通过事件触发
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        // 取消订阅事件
        GameEvents.OnMapSegmentLoaded -= UpdateMapUI;
        GameEvents.OnMapNodeSelected -= OnMapNodeSelected;
    }

    private void OnMapNodeSelected(MapNodeData selectedNode)
    {
        // 1. 遍历所有 MapNode，更新其视觉状态
        foreach (MapNode nodeInstance in _nodeLayer.GetChildren().Cast<MapNode>())
        {
            nodeInstance.UpdateVisuals();
        }

        // 2. 播放玩家移动动画
        MovePlayerToNode(selectedNode);
    }

    private void MovePlayerToNode(MapNodeData nodeData)
    {
        // 计算目标位置（节点中心）
        Vector2 targetPos = nodeData.Position + new Vector2(50, 50) - (_playerIcon.Size / 2);
        
        // 使用 Tween 播放移动动画
        Tween tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_playerIcon, "position", targetPos, 0.5f);
        
        // 相机跟随（可选）
        // tween.Parallel().TweenProperty(_camera, "position", nodeData.Position - new Vector2(960, 540), 0.5f);
    }

    public void UpdateMapUI()
    {
        // 清理旧节点
        foreach (Node child in _nodeLayer.GetChildren())
        {
            child.QueueFree();
        }
        foreach (Node child in _pathLayer.GetChildren())
        {
            child.QueueFree();
        }

        var segment = MapManager.Instance.CurrentSegment;
        if (segment == null) return;

        if (OS.IsDebugBuild())
        {
            int nodeCount = segment.Nodes?.Count ?? 0;
            GD.Print($"[MapScreen] UpdateMapUI: segmentId={segment.SegmentId} nodes={nodeCount} currentNodeId={MapManager.Instance.CurrentNodeId} nodeLayerSize={_nodeLayer.Size} mapScreenSize={Size}");
        }

        // 创建新节点
        foreach (var nodeData in segment.Nodes)
        {
            var nodeInstance = _nodeScene.Instantiate<MapNode>();
            _nodeLayer.AddChild(nodeInstance);
            nodeInstance.Setup(nodeData);

            if (OS.IsDebugBuild())
            {
                GD.Print($"[MapScreen] NodeInstance: id={nodeData.Id} type={nodeData.NodeType} dataPos={nodeData.Position} instancePos={nodeInstance.Position} visible={nodeInstance.Visible}");
            }

            // 如果是当前节点，直接设置玩家位置
            if (MapManager.Instance.CurrentNodeId == nodeData.Id)
            {
                _playerIcon.Position = nodeData.Position + new Vector2(50, 50) - (_playerIcon.Size / 2);
            }

            // 绘制连接线
            foreach (int connectedId in nodeData.ConnectedNodes)
            {
                var targetNode = segment.Nodes.Find(n => n.Id == connectedId);
                if (targetNode != null)
                {
                    DrawPath(nodeData.Position + new Vector2(50, 50), targetNode.Position + new Vector2(50, 50));
                }
            }
        }
    }

    private void DrawPath(Vector2 from, Vector2 to)
    {
        // 这里只是简单示例，实际上需要为每对连接创建一个 Line2D 节点
        // 或者使用多个 Line2D 绘制
        Line2D line = new Line2D();
        line.AddPoint(from);
        line.AddPoint(to);
        line.Width = 2;
        line.DefaultColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        _pathLayer.AddChild(line);
    }
}
}
