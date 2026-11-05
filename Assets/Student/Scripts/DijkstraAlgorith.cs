using System;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraAlgorith
{
    private DirectedEdge[] edgeTo;
    private double[] distTo;
    private IndexMinPQ prioQueue;

    public DijkstraAlgorith(EdgeWeightedDigraph graf, int startIndex)
    {
        int v = graf.Vertices();
        edgeTo = new DirectedEdge[v];
        distTo = new double[v];
        prioQueue = new IndexMinPQ(v);

        //sets all ditances to infinity except for the starting vertex, which is set to 0. The starting vertex is then added to the priority queue.
        for (int vertex = 0; vertex < graf.Vertices(); vertex++)
        {
            distTo[vertex] = double.PositiveInfinity;
        }
        distTo[startIndex] = 0;

        prioQueue.Insert(startIndex, 0);

        //relaxes the edges of the graph until the priority queue is empty,
        //which means that all vertices have been processed and
        //the shortest paths from the starting vertex to all other vertices have been found.
        while (!prioQueue.IsEmpty())
        {
            relax(graf, prioQueue.DelMin());
        }
    }

    public double DistTo(int v) => distTo[v];
    public bool HasPathTo(int v) => distTo[v] < double.PositiveInfinity;
    public IEnumerable<DirectedEdge> pathTo(int v)
    {
        if (!HasPathTo(v)) yield break;

        var stack = new Stack<DirectedEdge>();
        for (var e = edgeTo[v]; e != null; e = edgeTo[e.From()])
            stack.Push(e);
        foreach (var e in stack) yield return e;
    }

    //relaxes all edges adjacent to a given vertex, updating the shortest path estimates and the priority queue as necessary.
    private void relax(EdgeWeightedDigraph graf, int vertexIndex)
    {
        foreach (DirectedEdge edge in graf.Adj(vertexIndex))
        {
            int w = edge.To();
            if (distTo[w] > distTo[vertexIndex] + edge.Weight())
            {
                distTo[w] = distTo[vertexIndex] + edge.Weight();
                edgeTo[w] = edge;
                if (prioQueue.contains(w)) prioQueue.change(w, distTo[w]);
                else prioQueue.Insert(w, distTo[w]);
            }
        }
    }

    //same as the relax method above but for a single edge instead of all edges adjacent to a vertex
    //made becouse we did not know if we would use this or the other relax method, so we made both
    private void relax(DirectedEdge edge)
    {
        int v = edge.From(), w = edge.To();
        if (distTo[w] > distTo[v] + edge.Weight())
        {
            distTo[w] = distTo[v] + edge.Weight();
            edgeTo[w] = edge;
            if (prioQueue.contains(w)) prioQueue.decreaseKey(w, distTo[w]);
            else prioQueue.Insert(w, distTo[w]);
        }
    }


}
