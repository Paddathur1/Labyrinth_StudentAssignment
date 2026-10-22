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
        pq = new int[MaxN + 1];
        qp = new int[MaxN + 1];
        for (int i = 0; i <= MaxN; i++) qp[i] = -1;
    }

    public bool IsEmpty()
    {
        return n == 0;
    }

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

    public void change(int i, double key)
    {
        keys[i] = key;
        Swim(qp[i]);
        Sink(qp[i]);
    }

    public void decreaseKey(int i, double key)
    {
        keys[i] = key;
        Swim(qp[i]);
    }

    private void Swim(int k)
    {
        while (k > 1 && Greater(k / 2, k))
        {
            Exch(k / 2, k);
            k = k / 2;
        }
    }

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
