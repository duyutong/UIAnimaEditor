using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 松散四叉树节点，适合动态场景或宽松分区
/// </summary>
public class LooseQuadTree
{
    private const int MAX_OBJECTS = 4;
    private const int MAX_LEVELS = 5;

    private int level;
    private List<Rect> objects;
    private Rect bounds;
    private LooseQuadTree[] children;

    // 控制松散边界放大比例（例如 2 表示边界扩大 2 倍）
    private float looseness = 2.0f;

    public LooseQuadTree(int level, Rect bounds)
    {
        this.level = level;
        this.bounds = bounds;
        objects = new List<Rect>();
        children = null;
    }

    /// <summary>
    /// 清空当前节点及其子节点
    /// </summary>
    public void Clear()
    {
        objects.Clear();

        if (children != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (children[i] != null)
                {
                    children[i].Clear();
                    children[i] = null;
                }
            }
            children = null;
        }
    }

    /// <summary>
    /// 创建子节点，并应用松散边界
    /// </summary>
    private void Subdivide()
    {
        children = new LooseQuadTree[4];

        float subWidth = bounds.width / 2f;
        float subHeight = bounds.height / 2f;
        float x = bounds.x;
        float y = bounds.y;

        children[0] = new LooseQuadTree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight)); // 左上
        children[1] = new LooseQuadTree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight)); // 右上
        children[2] = new LooseQuadTree(level + 1, new Rect(x, y, subWidth, subHeight)); // 左下
        children[3] = new LooseQuadTree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight)); // 右下
    }

    /// <summary>
    /// 获取对象属于哪个子节点（返回 -1 表示跨象限）
    /// </summary>
    private int GetIndex(Rect rect)
    {
        for (int i = 0; i < 4; i++)
        {
            if (children == null) break;

            Rect looseBounds = GetLooseBounds(children[i].bounds);
            if (looseBounds.Contains(rect.min) && looseBounds.Contains(rect.max))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 获取放大后的松散边界
    /// </summary>
    private Rect GetLooseBounds(Rect original)
    {
        float width = original.width * looseness;
        float height = original.height * looseness;
        float centerX = original.x + original.width / 2f;
        float centerY = original.y + original.height / 2f;
        return new Rect(centerX - width / 2f, centerY - height / 2f, width, height);
    }

    /// <summary>
    /// 插入一个矩形
    /// </summary>
    public void Insert(Rect rect)
    {
        // 超出整个树的边界就不插入
        if (!bounds.Overlaps(rect)) return;

        if (children != null)
        {
            int index = GetIndex(rect);
            if (index != -1)
            {
                children[index].Insert(rect);
                return;
            }
        }

        objects.Add(rect);

        if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
        {
            if (children == null)
                Subdivide();

            for (int i = objects.Count - 1; i >= 0; i--)
            {
                int index = GetIndex(objects[i]);
                if (index != -1)
                {
                    Rect obj = objects[i];
                    objects.RemoveAt(i);
                    children[index].Insert(obj);
                }
            }
        }
    }

    /// <summary>
    /// 检索与区域重叠的对象
    /// </summary>
    public List<Rect> Retrieve(List<Rect> returnList, Rect area)
    {
        if (!bounds.Overlaps(area))
            return returnList;

        if (children != null)
        {
            for (int i = 0; i < 4; i++)
            {
                Rect looseBounds = GetLooseBounds(children[i].bounds);
                if (looseBounds.Overlaps(area))
                {
                    children[i].Retrieve(returnList, area);
                }
            }
        }

        returnList.AddRange(objects);
        return returnList;
    }
}
