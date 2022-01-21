using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField]
    private List<Vector2> nodes;
    [SerializeField]
    private List<Transform> transforms;

    private void Start()
    {
        nodes = new List<Vector2>();
        foreach (var transform in transforms)
        {
            nodes.Add(transform.position);
        }
    }

    public List<Vector2> Nodes
    {
        get { return nodes; }
    }

    public void AddNode(Vector2 node)
    {
        nodes.Add(node);
    }

}
