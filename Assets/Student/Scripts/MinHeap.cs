using UnityEngine;
using System;
using System.Collections.Generic;

public class MinHeap : MonoBehaviour
{
    private List<int[]> heap;

    public MinHeap()
    {
        heap = new List<int[]>();

    }

    public int Count => heap.Count;

    //enqueue
    public void Enqueue(int node, int dist)
    {
        heap.Add(new int[] { dist, node });
        SiftUp(heap.Count - 1);

    }

    private void SiftUp(int i)
    {
        while(i < 0)
        {
            int parent = (i - 1) / 2;
            if (heap[parent][0] <= heap[i][])
        }
    }
}
