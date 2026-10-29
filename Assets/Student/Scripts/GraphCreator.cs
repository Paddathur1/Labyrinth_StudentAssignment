using System.Collections.Generic;
using UnityEngine;

public static class GraphCreator
{
    private static readonly Vector2Int[] Directions =
    {
        new Vector2Int(0, 1),   // Up
        new Vector2Int(0, -1),  // Down
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(1, 0)    // Right
    };

    public static EdgeWeightedDigraph MakeGraf(Vector2Int start,  IMapData mapData)
    {
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        Debug.Log("Starting MakeGraf");

        // Upper bound of the vertices.
        int amountOfVertices = mapData.Width * mapData.Height;
        EdgeWeightedDigraph graph = new EdgeWeightedDigraph(amountOfVertices);

        List<Vector2Int> createdVertices = new List<Vector2Int>();
        Dictionary<Vector2Int, int> vertexIndices = new Dictionary<Vector2Int, int>();

        AddVertex(start, createdVertices, vertexIndices);

        for (int i = 0; i < createdVertices.Count; i++)
        {
            GetNeighbors(createdVertices[i], mapData, graph, createdVertices, vertexIndices);
        }

        graph.SetVertexPositions(createdVertices);

        stopwatch.Stop();

        Debug.Log("Stopped MakeGraf: Elapsed time: " + stopwatch.Elapsed.TotalMilliseconds.ToString("F3") + " milliseconds");

        return graph;
    }

    public static void GetNeighbors(
        Vector2Int position,
        IMapData mapData,
        EdgeWeightedDigraph graph,
        List<Vector2Int> createdVertices,
        Dictionary<Vector2Int, int> vertexIndices)
    {
        // Ensure the current position is present in createdVertices.
        int fromIndex = AddVertex(
            position,
            createdVertices,
            vertexIndices);

        for (int d = 0; d < Directions.Length; d++)
        {
            Vector2Int neighborPos = position + Directions[d];

            // Bounds check: skip neighbors outside the map.
            if (neighborPos.x < 0 || neighborPos.x >= mapData.Width || neighborPos.y < 0 || neighborPos.y >= mapData.Height)
            {
                continue;
            }

            // Skip if movement blocked.
            if (PathfindingAlgorithm.IsMovementBlocked(position, neighborPos, mapData))
            {
                continue;
            }

            int toIndex = AddVertex(neighborPos, createdVertices, vertexIndices);

            float cost = PathfindingAlgorithm.GetMovementCost(position, neighborPos, mapData);

            graph.AddEdge(new DirectedEdge(fromIndex, toIndex, cost));
        }

        List<JumpConnection> jumps = mapData.GetJumpsFrom(position);

        foreach (JumpConnection item in jumps)
        {
            Vector2Int to = item.To;

            int toIndex = AddVertex(to, createdVertices, vertexIndices);

            float cost = 0f;
            mapData.TryGetJumpCost(position, to, out cost);

            graph.AddEdge(new DirectedEdge(fromIndex, toIndex, cost));
        }

        // Mark position as visited if we can get a valid index.
        // The vertex index dictionary now provides the visited/index lookup.
    }

    private static int AddVertex(
        Vector2Int position,
        List<Vector2Int> createdVertices,
        Dictionary<Vector2Int, int> vertexIndices)
    {
        if (vertexIndices.TryGetValue(position, out int existingIndex))
        {
            return existingIndex;
        }

        int newIndex = createdVertices.Count;

        createdVertices.Add(position);
        vertexIndices.Add(position, newIndex);

        return newIndex;
    }
}
