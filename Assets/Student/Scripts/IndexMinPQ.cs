using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.InputSystem;

public class IndexMinPQ 
{
    private int n;
    private int[] pq;
    private int[] qp;
    private double[] keys;
    public IndexMinPQ(int MaxN)
    {
        keys = new double[MaxN + 1];
        //indexing arrays for the priority queue and its inverse, which are used to efficiently manage the elements in the queue.
        pq = new int[MaxN + 1];
        qp = new int[MaxN + 1];
        //initializes the inverse array to -1, indicating that no elements are currently in the priority queue.
        for (int i = 0; i <= MaxN; i++) qp[i] = -1;
    }

    public bool IsEmpty()
    {
        return n == 0;
    }
    //inserts a new element with a given index and key into the priority queue, updating the necessary data structures to maintain the heap property.
    public void Insert(int k, double key)
    {
        n++;
        qp[k] = n;
        pq[n] = k;
        keys[k] = key;
        Swim(n);
    }
    public  int min()
    {
        return pq[1]; 
    }
    //removes and returns the index of the minimum key in the priority queue, updating the necessary data structures to maintain the heap property.
    public int DelMin()
    {
        int indexOfMin = pq[1];
        Exch(1, n--);
        Sink(1);
        keys[indexOfMin] = 0;
        qp[pq[n + 1]] = -1;
        return indexOfMin;

    }

    public  bool contains(int i)
    {
        return qp[i] !=- 1;
    }
    //changes the key associated with a given index in the priority queue, updating the necessary data structures to maintain the heap property.
    public void change(int i, double key)
    {
        keys[i] = key;
        Swim(qp[i]);
        Sink(qp[i]);
    }
    //decreases the key associated with a given index in the priority queue, updating the necessary data structures to maintain the heap property.
    public void decreaseKey(int i, double key)
    {
        keys[i] = key;
        Swim(qp[i]);
    }
    //increases the key associated with a given index in the priority queue, updating the necessary data structures to maintain the heap property.
    private void Swim(int k)
    {
        while (k > 1 && Greater(k / 2, k))
        {
            Exch(k / 2, k);
            k = k / 2;
        }
    }
    //sinks an element down the heap to restore the heap property after a deletion or key change, ensuring that the minimum key is always at the root of the heap.
    private void Sink(int k)
    {
        while (2 * k <=n)
        {
            int j = 2 * k;

            if (j < n && Greater(j, j + 1)) j++;

            if (!Greater(k, j)) break;

            Exch(k, j);
            k = j;
        }
    }
    //exchanges the positions of two elements in the priority queue and updates the inverse array accordingly, maintaining the correct mapping between indices and their positions in the heap.
    private void Exch(int i, int j)
    {
        int swap = pq[i];
        pq[i] = pq[j];
        pq[j] = swap;

        qp[pq[i]] = i;
        qp[pq[j]] = j;
    }

    private bool Greater(int i, int j)
    {
        return keys[pq[i]].CompareTo(keys[pq[j]]) > 0;
    }


}
