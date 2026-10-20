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
    private Key[] keys;
    public IndexMinPQ(int MaxN)
    {
        keys = new Key[MaxN + 1];
        pq = new int[MaxN + 1];
        qp = new int[MaxN + 1];
        for (int i = 0; i <= MaxN; i++) qp[i] = -1;
    }

    public bool isEmpty()
    {
        return n == 0;
    }

    public void insert(int k, Key key)
    {
        n++;
        qp[k] = n;
        pq[n] = k;
        keys[k] = key;
        swim(n);
    }
    public  Key min()
    {
        return keys[pq[1]]; 
    }

    public int DelMin()
    {
        int indexOfMin = pq[1];
        exch(1, n--);
        sink(1);
        keys[pq[n + 1]] = default(Key);
        qp[pq[n + 1]] = -1;
        return indexOfMin;

    }

    private void swim(int k)
    {
        while (k > 1 && greater(k / 2, k))
        {
            exch(k / 2, k);
            k = k / 2;
        }
    }

    private void sink(int k)
    {
        while (2 * k <=n)
        {
            int j = 2 * k;

            if (j < n && greater(j, j + 1)) j++;

            if (!greater(k, j)) break;
        }
    }

    private void exch(int i, int j)
    {
        int swap = pq[i];
        pq[i] = pq[j];
        pq[j] = swap;

        qp[pq[i]] = i;
        qp[pq[j]] = j;
    }

    private bool greater(int i, int j)
    {
        return keys[pq[i]].CompareTo(keys[pq[j]]) > 0;
    }


}
