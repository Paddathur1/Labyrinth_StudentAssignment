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
        EdgeWeightedDigraph graph = MakeGraf(start, mapData);
        // Most of your solution should be implemented in this method.

        Debug.Log(graph.Edges());

        Debug.Log("FindShortestPath is not implemented.");
        List<Vector2Int> path = BreadthFirstSearch(mapData, graph, start, goal);
        return path;
    }

    public static void DijkstraSearch(IMapData mapData, EdgeWeightedDigraph graph, Vector2Int start, Vector2Int goal)
    {

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

    public static EdgeWeightedDigraph MakeGraf(Vector2Int start, IMapData mapData)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        Debug.Log("starting MakeGraf");
        stopwatch.Start();

        //upper bounds of the vertices
        int amountOfVertices = mapData.Width * mapData.Height;
        EdgeWeightedDigraph graph = new EdgeWeightedDigraph(amountOfVertices);
        List<Vector2Int> createdVertices = new List<Vector2Int>();
        

        createdVertices.Add(start);
        //GetNeighbors(start, mapData, graph, createdVertices, visted);

        for (int i = 0; i < createdVertices.Count; i++)
        {
            GetNeighbors(createdVertices[i], mapData, graph, createdVertices);
        }

        graph.SetVertexPositions(createdVertices);

        stopwatch.Stop();
        Debug.Log("Stopped MakeGraf: Elapsed time: " + stopwatch.ElapsedMilliseconds /1000 + " seconds");

        

        return graph;
    }

    public static void GetNeighbors(Vector2Int position, IMapData mapData, EdgeWeightedDigraph graph, List<Vector2Int> createdVertices)
    {
        // Ensure the current position is present in createdVertices.
        if (!createdVertices.Contains(position))
        {
            createdVertices.Add(position);
        }

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0)    // Right
        };

        for (int d = 0; d < directions.Length; d++)
        {
            Vector2Int dir = directions[d];
            Vector2Int neighborPos = position + dir;

            // Bounds check: skip neighbors outside the map.
            if (neighborPos.x < 0 || neighborPos.x >= mapData.Width || neighborPos.y < 0 || neighborPos.y >= mapData.Height)
                continue;

            // Skip if movement blocked.
            if (IsMovementBlocked(position, neighborPos, mapData))
                continue;

            // Ensure we have a valid index for 'position'.
            int fromIndex = createdVertices.IndexOf(position);
            if (fromIndex == -1)
            {
                // Shouldn't happen because we ensured containment above, but guard anyway.
                createdVertices.Add(position);
                fromIndex = createdVertices.IndexOf(position);
            }

            // If neighbor not yet created, add it and then add edge.
            if (!createdVertices.Contains(neighborPos))
            {
                createdVertices.Add(neighborPos);
                int toIndex = createdVertices.IndexOf(neighborPos);
                if (toIndex != -1)
                {
                    float cost = GetMovementCost(position, neighborPos, mapData);
                    DirectedEdge newEdge = new DirectedEdge(fromIndex, toIndex, cost);
                    graph.AddEdge(newEdge);
                }
            }
            else
            {
                // Neighbor already exists; get its index safely and add edge if valid.
                int toIndex = createdVertices.IndexOf(neighborPos);
                if (toIndex != -1 && fromIndex != toIndex)
                {
                    float cost = GetMovementCost(position, neighborPos, mapData);
                    DirectedEdge newEdge = new DirectedEdge(fromIndex, toIndex, cost);
                    graph.AddEdge(newEdge);
                }
            }
        }

        // Mark position as visited if we can get a valid index.
        int posIndex = createdVertices.IndexOf(position);
        //if (posIndex >= 0 && posIndex < visted.Length)
        //{
        //    visted[posIndex] = true;
        //}
    }

    private static float GetMovementCost(Vector2Int from, Vector2Int to, IMapData mapData)
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
