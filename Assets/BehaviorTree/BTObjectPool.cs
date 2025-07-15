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
            Debug.LogError("��������Ϊ�ա�");
            return null;
        }

        // �������Ƿ����и���Ķ���
        if (pool.TryGetValue(className, out var objectQueue) && objectQueue.Count > 0)
        {
            return objectQueue.Dequeue();
        }

        // �������û�У��򴴽��¶���
        var type = Type.GetType(className);
        if (type == null)
        {
            Debug.LogError($"δ�ҵ��� {className}��");
            return null;
        }

        return Activator.CreateInstance(type);
    }
    public static T GetObject<T>(Type type) where T:class 
    {
        string className = type.FullName;
        // �������Ƿ����и���Ķ���
        if (pool.TryGetValue(className, out var objectQueue) && objectQueue.Count > 0)
        {
            return objectQueue.Dequeue() as T;
        }

        return Activator.CreateInstance(type) as T;
    }
    public static T GetObject<T>() where T : class
    {
        string className = typeof(T).FullName;
        return GetObject(className) as T;
    }
    /// <summary>
    /// ������黹������ء�
    /// </summary>
    /// <param name="obj">Ҫ�黹�Ķ���</param>
    public static void ReturnObject(object obj)
    {
        if (obj == null)
        {
            Debug.LogError("�黹�Ķ�����Ϊ�ա�");
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
    /// ��ն���ء�
    /// </summary>
    public static void ClearPool()
    {
        pool.Clear();
    }
}
