//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class DijkstraAlgorith
//{
//    private DirectedEdge[] edgeTo;
//    private double[] distTo;
//    private IndexMinPQ prioQueue;

//    public DijkstraAlgorith(EdgeWeightedDigraph graf, int startIndex)
//    {
        
//        edgeTo = new DirectedEdge[startIndex];
//        distTo = new double[startIndex];
//        prioQueue = new IndexMinPQ(graf.Edge());

//        for (int vertex = 0; vertex < graf.Vertices(); vertex++)
//        {
//            distTo[vertex] = double.PositiveInfinity;
//        }
//        distTo[startIndex] = 0;

//        prioQueue.Insert(startIndex, 0);

//        while (!prioQueue.IsEmpty())
//        {
//            relax(graf, prioQueue.Dequeue());
//        }
//    }
//    private void relax(EdgeWeightedDigraph graf, int vertexIndex)
//    {
//        foreach (DirectedEdge edge in graf.Adj(vertexIndex))
//        {
//            int w = edge.To();
//            if (distTo[w] > distTo[vertexIndex] + edge.Weight())
//            {
//                distTo[w] = distTo[vertexIndex] + edge.Weight();
//                edgeTo[w] = edge;
//                if (prioQueue.contains(w)) prioQueue.change(w, distTo[w]);
//                else prioQueue.insert(w, distTo[w]);
//            }
//        }
//    }

//    private void relax(DirectedEdge edge)
//    {
//        int v = edge.From(), w = edge.To();
//        if (distTo[w] > distTo[v] + edge.Weight())
//        {
//            distTo[w] = distTo[v] + edge.Weight();
//            edgeTo[w] = edge;
//            if (prioQueue.contains(w)) prioQueue.decreaseKey(w, distTo[w]);
//            else prioQueue.insert(w, distTo[w]);
//        }
//    }


//}
