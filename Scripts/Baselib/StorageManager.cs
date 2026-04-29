using Godot;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// 基础存储管理类，提供简单的 JSON 数据保存与加载。
/// </summary>
public partial class StorageManager : Singleton<StorageManager>
{
    private string _savePath = "user://save_data/";
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };
        options.Converters.Add(new Vector2JsonConverter());
        return options;
    }

    private sealed class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                float x = 0f;
                float y = 0f;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        continue;
                    }

                    string propertyName = reader.GetString();
                    if (!reader.Read())
                    {
                        continue;
                    }

                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        continue;
                    }

                    float value = reader.GetSingle();
                    if (string.Equals(propertyName, "x", System.StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(propertyName, "X", System.StringComparison.OrdinalIgnoreCase))
                    {
                        x = value;
                    }
                    else if (string.Equals(propertyName, "y", System.StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(propertyName, "Y", System.StringComparison.OrdinalIgnoreCase))
                    {
                        y = value;
                    }
                }

                return new Vector2(x, y);
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string s = reader.GetString();
                if (s != null)
                {
                    string trimmed = s.Trim();
                    if (trimmed.StartsWith("(") && trimmed.EndsWith(")"))
                    {
                        trimmed = trimmed.Substring(1, trimmed.Length - 2);
                    }

                    string[] parts = trimmed.Split(',');
                    if (parts.Length == 2 &&
                        float.TryParse(parts[0], out float x) &&
                        float.TryParse(parts[1], out float y))
                    {
                        return new Vector2(x, y);
                    }
                }
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.X);
            writer.WriteNumber("y", value.Y);
            writer.WriteEndObject();
        }
    }

    public override void _Ready()
    {
        base._Ready();
        if (!DirAccess.DirExistsAbsolute(_savePath))
        {
            DirAccess.MakeDirRecursiveAbsolute(_savePath);
        }
    }

    /// <summary>
    /// 保存数据到指定文件名
    /// </summary>
    public void SaveData<T>(string fileName, T data)
    {
        string path = ProjectSettings.GlobalizePath(_savePath + fileName + ".json");
        try
        {
            string jsonString = JsonSerializer.Serialize(data, JsonOptions);
            File.WriteAllText(path, jsonString);
            GD.Print($"[StorageManager] 数据已成功保存到: {path}");
        }
        catch (System.Exception e)
        {
            GD.PushError($"[StorageManager] 保存数据失败: {e.Message}");
        }
    }

    /// <summary>
    /// 加载指定文件名的数据
    /// </summary>
    public T LoadData<T>(string fileName)
    {
        string path = ProjectSettings.GlobalizePath(_savePath + fileName + ".json");
        if (!File.Exists(path))
        {
            GD.PushWarning($"[StorageManager] 尝试加载不存在的文件: {path}");
            return default;
        }

        try
        {
            string jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(jsonString, JsonOptions);
        }
        catch (System.Exception e)
        {
            GD.PushError($"[StorageManager] 加载数据失败: {e.Message}");
            return default;
        }
    }

    /// <summary>
    /// 从 res:// 加载只读数据
    /// </summary>
    public T LoadResourceData<T>(string resPath)
    {
        if (!Godot.FileAccess.FileExists(resPath))
        {
            GD.PushWarning($"[StorageManager] 尝试加载不存在的资源文件: {resPath}");
            return default;
        }

        try
        {
            using var file = Godot.FileAccess.Open(resPath, Godot.FileAccess.ModeFlags.Read);
            string jsonString = file.GetAsText();
            return JsonSerializer.Deserialize<T>(jsonString, JsonOptions);
        }
        catch (System.Exception e)
        {
            GD.PushError($"[StorageManager] 加载资源数据失败: {e.Message}");
            return default;
        }
    }

    /// <summary>
    /// 删除存档文件
    /// </summary>
    public void DeleteData(string fileName)
    {
        string path = ProjectSettings.GlobalizePath(_savePath + fileName + ".json");
        if (File.Exists(path))
        {
            File.Delete(path);
            GD.Print($"[StorageManager] 数据文件已删除: {path}");
        }
    }
}
