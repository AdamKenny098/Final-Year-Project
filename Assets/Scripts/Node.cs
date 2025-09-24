using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int left, right, up, down;

    public int getLeft()
    {
        return left;
    }

    public int getRight()
    {
        return right;
    }

    public int getUp()
    {
        return up;
    }

    public int getDown()
    {
        return down;
    }

    public int getHeight()
    {
        return up - down;
    }

    public int getWidth()
    {
        return left - right;
    }

    public Vector2 GetCenter()
    {
        return new Vector2((left + right) / 2f, (down + up) / 2f);
    }
}
