using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ForcedDirectedGraph : MonoBehaviour
{
    public List<Node> nodes;
    // Start is called before the first frame update
    void Start()
    {
        nodes = new List<Node>();
        for(int i=0; i<20; i++)
        {
            nodes.Add(new Node() { children = nodes.Where(node => Random.value > 0.5).ToList(), position = Random.insideUnitSphere * 10, velocity = Vector3.zero });
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var node in nodes)
        {
            node.position += node.velocity * Time.deltaTime;
        }
    }

    void OnDrawGizmos1()
    {
        foreach(var node in nodes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(node.position, 0.125f);
            Gizmos.color = Color.green;
            foreach(var connectedNodes in node.children)
            {
                Gizmos.DrawLine(node.position, connectedNodes.position);
            }

        }
    }
}

public class Node
{
    public Vector3 position;
    public Vector3 velocity;
    public List<Node> children;
}