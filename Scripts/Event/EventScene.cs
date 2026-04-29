using Godot;
using System.Collections.Generic;
using CardDemo.Event;
using CardDemo.Map;

namespace CardDemo.Event
{
    public partial class EventScene : Control
    {
        private Control _eventDeckContainer; // 用于放置事件卡牌的容器

        private string _currentEventId;

        public override void _Ready()
        {
            base._Ready();
            _eventDeckContainer = GetNode<Control>("EventDeck");
            // 从 MapManager 获取挂起的事件 ID 并初始化
            string pendingId = MapManager.Instance.PendingEventId;
            if (!string.IsNullOrEmpty(pendingId))
            {
                SetupEvent(pendingId);
            }
        }

        public void SetupEvent(string eventId)
        {
            _currentEventId = eventId;
            GD.Print($"[EventScene] 设置事件: {eventId}");

            var eventData = EventDataLoader.LoadEventData(eventId);
            if (eventData == null)
            {
                GD.PushError($"[EventScene] 无法加载事件数据: {eventId}");
                return;
            }

            LoadEventCards(eventData);
        }

        private void LoadEventCards(EventData eventData)
        {
            // 清理旧卡牌
            foreach (Node child in _eventDeckContainer.GetChildren())
            {
                child.QueueFree();
            }

            // 从 EventData 中加载卡牌信息，并实例化卡牌
            foreach (var cardInfo in eventData.Cards)
            {
                for (int i = 0; i < cardInfo.Count; i++)
                {
                    var cardInstance = GameNormalScene.CardScene.Instantiate<Card>();
                    _eventDeckContainer.AddChild(cardInstance);
                    // 这里需要根据 cardInfo.CardName 获取 CardData，然后设置给 cardInstance
                    // 假设 CardManager 有一个 GetCardDataByName 方法
                    var data = CardManager.Instance.GetCardDataByName(cardInfo.CardName);
                    if (data != null)
                    {
                        cardInstance.Setup(data);
                        // 订阅卡牌的点击事件
                        cardInstance.Clicked += () => HandleCardSelection(cardInstance);
                    }
                    else
                    {
                        GD.PushWarning($"[EventScene] 无法找到卡牌数据: {cardInfo.CardName}");
                    }
                }
            }
        }

        // 处理玩家选择的卡牌
        private void HandleCardSelection(Card selectedCard)
        {
            GD.Print($"[EventScene] 玩家选择了卡牌: {selectedCard.cardInfo.CardShowName}");
            // 根据卡牌效果执行逻辑，然后返回地图
            // 这里只是一个示例，实际逻辑会更复杂
            // 假设选择卡牌后事件就结束了
            GameEvents.RaiseOnEventCompleted();
            GlobalManager.GotoMapScreen();
        }
    }
}
