using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;

//https://www.geeksforgeeks.org/dsa/kruskals-algorithm-simple-implementation-for-adjacency-matrix/
// Based on the Java implementation of Kruskal's Algorithm from GeeksForGeeks
// Adapted to work with Unity + DelaunatorSharp's IEdge for dungeon generation.
public class KruskalsAlgorithm
{
    // The number of node
    public int Vertices;

    public int[] parent;

    int Find(int i)
    {
        while (parent[i] != i)
        {
            i = parent[i];
        }
        return i;
    }

    
    // Finds the roots of both, then makes one root the parent of the other, combining the sets
    void Union(int i, int j)
    {
        int a = Find(i);
        int b = Find(j);
        parent[a] = b;
    }

    // Takes in the vertex count a list of edges and the points list.
    // Returns the Minimum Spanning Tree
    public List<IEdge> Kruskal(int Vertices, List<IEdge> edges, List<IPoint> points)
    {
        List<IEdge> minimalSpanningTree = new List<IEdge>();

        // Sort all edges by distance bewtween em
        edges.Sort((a, b) =>
        {
            
            float distA = Vector2.Distance(
                new Vector2((float)a.P.X, (float)a.P.Y),
                new Vector2((float)a.Q.X, (float)a.Q.Y)
            );

            float distB = Vector2.Distance(
                new Vector2((float)b.P.X, (float)b.P.Y),
                new Vector2((float)b.Q.X, (float)b.Q.Y)
            );

            //negative if distA < distB (so A goes before B),
            //zero if equal,
            //positive if distA > distB (so B goes before A).
            return distA.CompareTo(distB);
        });

        
        parent = new int[Vertices];
        for (int i = 0; i < Vertices; i++)
        {
            parent[i] = i; //node is its parent
        }

        int edgeCount = 0;

        int index = 0;

        //Main loop: keep going until we have V-1 edges (the MST condition)
        while (edgeCount < Vertices - 1 && index < edges.Count)
        {
            // Get the next candidate edge (smallest available).
            IEdge next = edges[index++];
            // ^ Increment index immediately, so "next" is the current edge.

            // Convert the edge's IPoints into integer indices, so we can use Union Find.
            int from = points.IndexOf(next.P); // get index of start point
            int to   = points.IndexOf(next.Q); // get index of end point

            // Find the root representatives of each endpoint.
            int x = Find(from);
            int y = Find(to);

            // If the endpoints are in different sets, adding this edge does not create a cycle.
            if (x != y)
            {
                // Accept this edge as part of the MST.
                minimalSpanningTree.Add(next);

                // Merge the two sets in Union Find.
                Union(x, y);

                // Increase count of edges added to MST.
                edgeCount++;
            }
        }

        // Return the final MST edges.
        return minimalSpanningTree;
    }
}
