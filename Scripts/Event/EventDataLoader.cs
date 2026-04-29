using Godot;
using System.Collections.Generic;
using System.Text.Json;

namespace CardDemo.Event
{
    public static class EventDataLoader
    {
        /// <summary>
        /// 从 res:// 加载事件数据
        /// </summary>
        public static EventData LoadEventData(string eventId)
        {
            string resPath = $"res://Data/Events/{eventId}.json";
            if (!FileAccess.FileExists(resPath))
            {
                GD.PushWarning($"[EventDataLoader] 尝试加载不存在的事件文件: {resPath}");
                return null;
            }

            try
            {
                using var file = FileAccess.Open(resPath, FileAccess.ModeFlags.Read);
                string jsonString = file.GetAsText();
                return JsonSerializer.Deserialize<EventData>(jsonString);
            }
            catch (System.Exception e)
            {
                GD.PushError($"[EventDataLoader] 加载事件数据失败: {e.Message}");
                return null;
            }
        }
    }
}
