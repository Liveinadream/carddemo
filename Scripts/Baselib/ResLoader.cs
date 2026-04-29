using Godot;
using System.Collections.Generic;

/*
 * 资源加载器，封装 Godot 的资源加载逻辑，并提供简单的缓存功能。
 */
public partial class ResLoader : Singleton<ResLoader>
{
    private readonly Dictionary<string, Resource> _cache = [];

    /*
 * 加载资源并缓存。
 */
    public T Load<T>(string path, bool cache = true) where T : Resource
    {
        if (cache && _cache.TryGetValue(path, out var cachedRes))
        {
            return cachedRes as T;
        }

        T res = GD.Load<T>(path);
        if (res == null)
        {
            GD.PushError($"[ResLoader] 资源加载失败: {path}");
            return null;
        }

        if (cache)
        {
            _cache[path] = res;
        }

        return res;
    }

    /*
 * 加载场景 (PackedScene)。
 */
    public PackedScene LoadScene(string path, bool cache = true)
    {
        return Load<PackedScene>(path, cache);
    }

    /*
 * 实例化场景。
 */
    public Node InstantiateScene(string path, bool cache = true)
    {
        var scene = LoadScene(path, cache);
        if (scene != null)
        {
            return scene.Instantiate();
        }
        return null;
    }

    /*
 * 清除特定缓存。
 */
    public void ClearCache(string path)
    {
        if (_cache.ContainsKey(path))
        {
            _cache.Remove(path);
        }
    }

    /*
 * 清空所有缓存。
 */
    public void ClearAll()
    {
        _cache.Clear();
    }
}