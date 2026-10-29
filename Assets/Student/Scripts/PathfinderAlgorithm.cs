using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class PathfindingAlgorithm
{
    // TODO: Implement the pathfinding assignment here.
    // Find the lowest-cost path from start to goal using the map dimensions, wall costs and directed jumps exposed by IMapData.
    public static List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int goal, IMapData mapData)
    {
        
        EdgeWeightedDigraph graph = GraphCreator.MakeGraf(start, mapData);
        List<Vector2Int> vertexPositions = graph.GetVertexPositions();

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        Debug.Log("starting pathfinding");
        stopwatch.Start();

        int src = vertexPositions.IndexOf(start);
        int dest = vertexPositions.IndexOf(goal);

        var path = new List<Vector2Int>();
        if (src == -1 || dest == -1) return path;
        if (src == dest) { path.Add(start); return path; }

        var algo = new DijkstraAlgorith(graph, src);

        if (!algo.HasPathTo(dest)) return path;

        path.Add(vertexPositions[src]);
        foreach (DirectedEdge e in algo.pathTo(dest))
            path.Add(vertexPositions[e.To()]);



        //List<Vector2Int> path = BreadthFirstSearch(mapData, graph, start, goal);

        stopwatch.Stop();
        Debug.Log("Stopped Pathfinding: Elapsed time: " + stopwatch.ElapsedMilliseconds + " milliseconds");

        return path;
    }

    public static List<Vector2Int> BreadthFirstSearch(IMapData mapData, EdgeWeightedDigraph graph, Vector2Int start, Vector2Int goal)
    {
        int v = graph.AdjTotal();
        bool[] visited = new bool[v];
        List<Vector2Int> path = new List<Vector2Int>();
        List<Vector2Int> vertexPositions = graph.GetVertexPositions();

        int src = vertexPositions.IndexOf(start);
        int dest = vertexPositions.IndexOf(goal);

        //prevent crash if src or dest are out of bounds
        if (src == -1 || dest == -1)
            return path;

        //travelpath to keep track of the traveled indices for reconstruction of the path
        int[] travelpath = new int[v];
        for (int i = 0; i < v; i++) travelpath[i] = -1;

        Queue<int> queue = new Queue<int>();
        visited[src] = true;
        queue.Enqueue(src);

        bool found = false;

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();

            if (current == dest)
            {
                found = true;
                break;
            }
            List<DirectedEdge> adjList = (List<DirectedEdge>)graph.Adj(current);
            //every directed edgw in the adjacency list is visited, neighbors added to the travelpath list and enqueued
            foreach (DirectedEdge edge in adjList)
            {
                int next = edge.To();
                if (!visited[next])
                {
                    visited[next] = true;
                    travelpath[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        //if we dont find the goal we return
        if (!found)
            return path;

        //add the travelpath into our res and reverse it for out path
        for (int at = dest; at != -1; at = travelpath[at])
            path.Add(vertexPositions[at]);

        path.Reverse();
        return path;
    }

    

    public static float GetMovementCost(Vector2Int from, Vector2Int to, IMapData mapData)
    {
        if (mapData.TryGetJumpCost(from, to, out float jumpCost))
            return jumpCost;

        float baseCost = 1f;
        int deltaX = to.x - from.x;
        int deltaY = to.y - from.y;

        if (deltaX != 0)
        {
            int wallX = deltaX > 0 ? to.x : from.x;
            float wallCost = mapData.GetVerticalWallCost(wallX, from.y);
            if (wallCost > 1f) baseCost = wallCost;
        }
        else if (deltaY != 0)
        {
            int wallY = deltaY > 0 ? to.y : from.y;
            float wallCost = mapData.GetHorizontalWallCost(from.x, wallY);
            if (wallCost > 1f) baseCost = wallCost;
        }

        return baseCost;
    }

    public static bool IsMovementBlocked(Vector2Int from, Vector2Int to, IMapData mapData)
    {
        if (mapData.TryGetJumpCost(from, to, out _))
            return false;

        int deltaX = to.x - from.x;
        int deltaY = to.y - from.y;

        if (Mathf.Abs(deltaX) + Mathf.Abs(deltaY) != 1)
            return true;

        if (deltaX != 0)
        {
            int wallX = deltaX > 0 ? to.x : from.x;
            float wallCost = mapData.GetVerticalWallCost(wallX, from.y);
            return float.IsPositiveInfinity(wallCost) || wallCost >= float.MaxValue;
        }

        int wallY = deltaY > 0 ? to.y : from.y;
        float horizontalWallCost = mapData.GetHorizontalWallCost(from.x, wallY);
        return float.IsPositiveInfinity(horizontalWallCost) || horizontalWallCost >= float.MaxValue;
    }
}
