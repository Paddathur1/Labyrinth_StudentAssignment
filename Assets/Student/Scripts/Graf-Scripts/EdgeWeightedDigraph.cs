using System.Collections.Generic;
using UnityEngine;

public class EdgeWeightedDigraph : MonoBehaviour
{
    private readonly int vertices; // number of vertices
    private int edges; // number of edges
    private List<List<DirectedEdge>> adj; // adjacency lists
    public EdgeWeightedDigraph(int vertices)
    {
        this.vertices = vertices;
        this.edges = 0;
        adj = new List<List<DirectedEdge>>(vertices);
        for (int v = 0; v < vertices; v++)
            adj.Add(new List<DirectedEdge>());
    }

    // See Exercise 4.4.2.
    public int Vertices() { return vertices; }
    public int Edge() { return edges; }
    public void AddEdge(DirectedEdge e)
    {
        adj[e.From()].Add(e);
        edges++;
    }
    public IEnumerable<DirectedEdge> Adj(int v)
    { return adj[v]; }
    public IEnumerable<DirectedEdge> Edges()
    {
        List<DirectedEdge> list = new List<DirectedEdge>();
        for (int v = 0; v < vertices; v++)
            foreach (DirectedEdge e in adj[v])
                list.Add(e);
        return list;
    }
}
