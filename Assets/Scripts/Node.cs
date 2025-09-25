using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float length, width;

    public Vector3 center;

    public BoxCollider box;
    public Node aChild, bChild;
    public bool isLeaf;

    public float getLength()
    {
        return box.bounds.size.x;
    }

    public float getWidth()
    {
        return box.bounds.size.z;
    }

    public Vector3 GetCenter()
    {
        return new Vector3(length / 2f, 0, width / 2f);
    }

    public Node(float length, float width, Vector3 center)
    {
        this.length = length;
        this.width = width;
        this.center = center;
        this.aChild = null;
        this.bChild = null;
        this.isLeaf = true;
    }
}
