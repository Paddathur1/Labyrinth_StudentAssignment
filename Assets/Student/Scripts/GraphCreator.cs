using System.Collections.Generic;
using UnityEngine;

public static class GraphCreator
{
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
        Debug.Log("Stopped MakeGraf: Elapsed time: " + stopwatch.ElapsedMilliseconds + " milliseconds");



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
            if (PathfindingAlgorithm.IsMovementBlocked(position, neighborPos, mapData))
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
                    float cost = PathfindingAlgorithm.GetMovementCost(position, neighborPos, mapData);
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
                    float cost = PathfindingAlgorithm.GetMovementCost(position, neighborPos, mapData);
                    DirectedEdge newEdge = new DirectedEdge(fromIndex, toIndex, cost);
                    graph.AddEdge(newEdge);
                }
            }
        }

        List<JumpConnection> jumps = mapData.GetJumpsFrom(position);

        foreach (JumpConnection item in jumps)
        {
            if (!createdVertices.Contains(item.To))
            {
                createdVertices.Add(item.To);
            }
            Vector2Int to = item.To;
            int fromIndex = createdVertices.IndexOf(position);
            int toIndex = createdVertices.IndexOf(to);
            float cost = 0;
            mapData.TryGetJumpCost(position, to, out cost);
            graph.AddEdge(new DirectedEdge(fromIndex, toIndex, cost));
        }


        // Mark position as visited if we can get a valid index.
        int posIndex = createdVertices.IndexOf(position);
        //if (posIndex >= 0 && posIndex < visted.Length)
        //{
        //    visted[posIndex] = true;
        //}
    }
}
