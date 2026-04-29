using Godot;
using System.Collections.Generic;
using CardDemo.Data;

namespace CardDemo.Map
{
    public partial class MapManager : Singleton<MapManager>
    {
        private MapSegmentData _currentSegment;
        private int _currentNodeId = -1;

        public string PendingEventId { get; private set; }

        // 当前地图所有节点的数据
        public MapSegmentData CurrentSegment => _currentSegment;
        public int CurrentNodeId => _currentNodeId;

        /*
         * 加载特定的地图片段
         * @param segmentId
         */
        public void LoadSegment(int segmentId)
        {
            GD.Print($"[MapManager] 正在加载地图片段: {segmentId}");

            // 从 res:// 加载静态地图片段数据
            string resPath = $"res://Data/Maps/map_segment_{segmentId}.json";
            _currentSegment = StorageManager.Instance.LoadResourceData<MapSegmentData>(resPath);

            if (_currentSegment == null)
            {
                GD.PushError($"[MapManager] 无法加载片段数据: {resPath}");
                return;
            }

            if (OS.IsDebugBuild())
            {
                int nodeCount = _currentSegment.Nodes?.Count ?? 0;
                GD.Print($"[MapManager] SegmentLoaded: res={resPath} id={_currentSegment.SegmentId} name={_currentSegment.SegmentName} next={_currentSegment.NextSegmentId} nodes={nodeCount}");
                if (_currentSegment.Nodes != null)
                {
                    foreach (var node in _currentSegment.Nodes)
                    {
                        int connectedCount = node.ConnectedNodes?.Count ?? 0;
                        GD.Print($"[MapManager] Node: id={node.Id} type={node.NodeType} pos={node.Position} connected={connectedCount} visited={node.IsVisited} eventId={node.EventId}");
                    }
                }
            }

            // 同步存档中的访问状态
            SyncVisitedState();

            if (OS.IsDebugBuild())
            {
                ZLog.D($"[MapManager] SyncVisitedState: currentNodeId={_currentNodeId}");

            }

            // 通知地图 UI 更新
            GameEvents.RaiseOnMapSegmentLoaded();
        }

        private void SyncVisitedState()
        {
            var playerSave = GlobalManager.GetInfos().save;
            if (playerSave == null) return;

            if (playerSave.CurrentMapSegmentId == _currentSegment.SegmentId)
            {
                _currentNodeId = playerSave.CurrentMapNodeId;
            }

            if (playerSave.VisitedNodesBySegment.ContainsKey(_currentSegment.SegmentId))
            {
                var visitedIds = playerSave.VisitedNodesBySegment[_currentSegment.SegmentId];
                foreach (var node in _currentSegment.Nodes)
                {
                    if (visitedIds.Contains(node.Id))
                    {
                        node.IsVisited = true;
                    }
                }
            }
        }

        /*
         * 处理节点点击事件
         * @param nodeData
         */
        public void OnNodeSelected(MapNodeData nodeData)
        {
            // 检查是否可以移动到该节点 (例如是否相邻)
            if (CanMoveTo(nodeData))
            {
                _currentNodeId = nodeData.Id;
                nodeData.IsVisited = true;

                // 更新存档数据
                UpdatePlayerSave(nodeData);

                // 触发事件
                TriggerNodeEvent(nodeData);

                // 通知所有 MapNode 更新视觉状态
                GameEvents.RaiseOnMapNodeSelected(nodeData);

                // 自动保存进度
                SaveProgress();
            }
        }

        private void UpdatePlayerSave(MapNodeData nodeData)
        {
            var playerSave = GlobalManager.GetInfos().save;
            if (playerSave == null) return;

            playerSave.CurrentMapSegmentId = _currentSegment.SegmentId;
            playerSave.CurrentMapNodeId = nodeData.Id;

            if (!playerSave.VisitedNodesBySegment.ContainsKey(_currentSegment.SegmentId))
            {
                playerSave.VisitedNodesBySegment[_currentSegment.SegmentId] = new Godot.Collections.Array<int>();
            }

            if (!playerSave.VisitedNodesBySegment[_currentSegment.SegmentId].Contains(nodeData.Id))
            {
                playerSave.VisitedNodesBySegment[_currentSegment.SegmentId].Add(nodeData.Id);
            }
        }

        private bool CanMoveTo(MapNodeData nodeData)
        {
            // 如果是初始状态，可以选第一个节点
            if (_currentNodeId == -1) return true;

            // 检查当前节点是否连接到目标节点
            var currentNode = _currentSegment.Nodes.Find(n => n.Id == _currentNodeId);
            return currentNode != null && currentNode.ConnectedNodes.Contains(nodeData.Id);
        }

        private void TriggerNodeEvent(MapNodeData nodeData)
        {
            GD.Print($"[MapManager] 触发节点事件: {nodeData.NodeType} (ID: {nodeData.EventId})");

            switch (nodeData.NodeType)
            {
                case "Battle":
                    GlobalManager.GotoSite1();
                    break;
                case "Shop":
                    GlobalManager.GotoSceneByPath("res://Scenes/Deck/shop_card_deck.tscn");
                    break;
                case "Event":
                    PendingEventId = nodeData.EventId;
                    GlobalManager.GotoSceneByPath(GameNormalScene.EventScene);
                    break;
                case "Exit":
                    // 加载下一个片段
                    LoadSegment(_currentSegment.NextSegmentId);
                    break;
            }
        }

        private void SaveProgress()
        {
            var infos = GlobalManager.GetInfos();
            if (infos != null && infos.save != null)
            {
                // 假设存档名称为 "auto_save" 或从 playerInfo 获取
                infos.SavePlayerInfo("auto_save");
                GD.Print("[MapManager] 已自动保存地图进度。");
            }
        }
    }
}
