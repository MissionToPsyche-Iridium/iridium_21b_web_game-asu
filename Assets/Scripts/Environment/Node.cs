using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node next;
    public Transform node_obj;
    public float x;
    public float y;

    public Node(Transform node, float x_offset, float y_offset)
    {
        this.node_obj = node;
        this.x = node.position.x + x_offset;
        this.y = node.position.y + y_offset;
    }

    public void setNext(Node new_node)
    {
        this.next = new_node;
    }
}