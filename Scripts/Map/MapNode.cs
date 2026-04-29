using Godot;
using CardDemo.Map;

namespace CardDemo.Map
{
    public partial class MapNode : Control
    {
        private MapNodeData _data;
        [Export]
        private Button _button;
        [Export]
        private Label _label;

    public void Setup(MapNodeData data)
    {
        _data = data;
        Position = data.Position;
        
        // _button = GetNode<Button>("Button");
        // _label = GetNode<Label>("Label");

        _label.Text = data.NodeType;
        _button.Pressed += OnPressed;

        UpdateVisuals();
    }

    private void OnPressed()
    {
        MapManager.Instance.OnNodeSelected(_data);
        // UpdateVisuals(); // 视觉更新由 MapScreen 统一处理
    }

    public void UpdateVisuals()
    {
        // 如果是当前节点，高亮显示
        if (MapManager.Instance.CurrentNodeId == _data.Id)
        {
            _button.Modulate = new Color(1, 1, 0); // 黄色表示当前
        }
        else if (_data.IsVisited)
        {
            _button.Modulate = new Color(0.5f, 0.5f, 0.5f); // 灰色表示已访问
        }
        else
        {
            _button.Modulate = new Color(1, 1, 1);
        }
    }
}
}
