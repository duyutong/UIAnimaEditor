using System;
using System.Collections.Generic;
using UnityEngine;

public class BTObjectPool
{
    private static readonly Dictionary<string, Queue<object>> pool = new Dictionary<string, Queue<object>>();

    public static object GetObject(string className)
    {
        if (string.IsNullOrEmpty(className))
        {
            Debug.LogError("类名不能为空。");
            return null;
        }

        // 检查池中是否已有该类的对象
        if (pool.TryGetValue(className, out var objectQueue) && objectQueue.Count > 0)
        {
            var obj = objectQueue.Dequeue();
            (obj as BehaviorTreeBaseState)?.OnRefresh();

            return obj;
        }

        // 如果池中没有，则创建新对象
        var type = Type.GetType(className);
        if (type == null)
        {
            Debug.LogError($"未找到类 {className}。");
            return null;
        }

        return Activator.CreateInstance(type);
    }
    public static T GetObject<T>(Type type) where T : BehaviorTreeBaseState
    {
        string className = type.FullName;
        // 检查池中是否已有该类的对象
        if (pool.TryGetValue(className, out var objectQueue) && objectQueue.Count > 0)
        {
            T obj = objectQueue.Dequeue() as T;
            obj?.OnRefresh();
            return obj;
        }

        return Activator.CreateInstance(type) as T;
    }
    public static T GetObject<T>() where T : BehaviorTreeBaseState
    {
        string className = typeof(T).FullName;

        T obj = GetObject(className) as T;
        obj?.OnRefresh();

        return obj;
    }
    /// <summary>
    /// 将对象归还到对象池。
    /// </summary>
    /// <param name="obj">要归还的对象</param>
    public static void ReturnObject(object obj)
    {
        if (obj == null)
        {
            Debug.LogError("归还的对象不能为空。");
            return;
        }

        var className = obj.GetType().FullName;
        if (!pool.TryGetValue(className, out var objectQueue))
        {
            objectQueue = new Queue<object>();
            pool[className] = objectQueue;
        }

        objectQueue.Enqueue(obj);
    }

    /// <summary>
    /// 清空对象池。
    /// </summary>
    public static void ClearPool()
    {
        pool.Clear();
    }
}
