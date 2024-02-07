using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum HexOrientation
{
    PointyTop,
    FlatTop
}
public class HexGridTest : MonoBehaviour
{
    public const float outerRadius = 10f;

    public float vertexComparisonThreshold = 0.01f;

    public const float innerRadius = outerRadius * 0.866025404f;

    public HexOrientation hexOrientation = HexOrientation.PointyTop;
    public Material hexMaterial; // Assign a material with a hexagon texture in the Unity Editor

    public LineRenderer lineRenderer;

    Mesh hexMesh;

    void Start()
    {

        hexMesh = GenerateHexMesh();
        // hexMesh = SimplifyMesh(hexMesh);
        DrawHexagon(hexMesh);

        // DrawMeshOutline(hexMesh);

        // TestAxialConversion();
        
    }

    private void DrawHexagon(Mesh hexMesh)
    {
        // Create a GameObject to represent the hexagon
        GameObject hexagonObject = new GameObject("HexagonObject");
        hexagonObject.AddComponent<MeshFilter>().mesh = hexMesh;

        MeshRenderer meshRenderer = hexagonObject.AddComponent<MeshRenderer>();
        meshRenderer.material = hexMaterial;
    }

    private List<Vector2Int> GenerateAxialHexGrid(int width, int height)
    {
        List<Vector2Int> grid = new List<Vector2Int>();

        for (int row = 0; row < height; row++)
        {
            int offset = row / 2;
            for (int col = -offset; col < width - offset; col++)
            {
                grid.Add(new Vector2Int(col, row));
            }
        }

        return grid;
    }

    private Mesh GenerateHexMeshOld()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                Vector3 center;
                center.x = (col + row * 0.5f - row / 2) * (HexMetrics.innerRadius * 2f);
                center.y = 0f;
                center.z = row * (HexMetrics.outerRadius * 1.5f);
                for (int i = 0; i < 6; i++)
                {

                    int vertexIndex = vertices.Count;
                    vertices.Add(center);
                    vertices.Add(center + corners[i]);
                    vertices.Add(center + corners[i + 1]);
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);

                    // uvs for individual tile texture, like the grid outline
                    Vector2[] triangleUVs = new Vector2[3] {
                        new Vector2(0.5f, 0.5f),
                        uvCorners[i],
                        uvCorners[i + 1]
                    };
                    uv.AddRange(triangleUVs);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }


    private Mesh GenerateHexMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        List<Vector2Int> coordinates = GenerateAxialHexGrid(2, 2);


        coordinates.ForEach((coordinate) =>
        {
            Vector3 center = AxialToWorld(coordinate);

            for (int i = 0; i < 6; i++)
            {

                int vertexIndex = vertices.Count;
                vertices.Add(center);
                vertices.Add(center + corners[i]);
                vertices.Add(center + corners[i + 1]);
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                // uvs for individual tile texture, like the grid outline
                Vector2[] triangleUVs = new Vector2[3] {
                            new Vector2(0.5f, 0.5f),
                            HexMetrics.uvCorners[i],
                            HexMetrics.uvCorners[i + 1]
                        };
                uv.AddRange(triangleUVs);
            }
        });

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }



    private Vector3 AxialToLocal(Vector2Int axialCoordinates)
    {
        float x = axialCoordinates.x * (HexMetrics.innerRadius * 2f) * 1.5f;
        float z = axialCoordinates.y * (HexMetrics.outerRadius * 1.5f);

        // Adjust for parent object's position
        return new Vector3(x, 0f, z);
    }

    private Vector3 AxialToWorld(Vector2Int axialCoordinates)
    {
        float x = (axialCoordinates.x + axialCoordinates.y * 0.5f) * (HexMetrics.innerRadius * 2f);
        float z = axialCoordinates.y * (HexMetrics.outerRadius * 1.5f);

        // Adjust for parent object's position
        return transform.TransformPoint(new Vector3(x, 0f, z));
    }

    private void TestAxialConversion()
    {
        for (int i = 0; i < 10; i++)
        {
            // Generate random axial coordinates for testing
            int randomX = Random.Range(-10, 11);
            int randomY = Random.Range(-10, 11);
            Vector2Int axialCoordinates = new Vector2Int(randomX, randomY);

            // Convert axial coordinates to world position
            Vector3 worldPos = AxialToWorld(axialCoordinates);

            // Convert world position back to axial coordinates
            Vector2Int convertedAxial = WorldToAxial(worldPos);

            // Log the results
            Debug.Log($"Test {i + 1} - Original Axial: {axialCoordinates}, World Position: {worldPos}, Converted Axial: {convertedAxial}");

            // Check if the converted axial coordinates match the original
            if (axialCoordinates != convertedAxial)
            {
                Debug.LogError("Conversion error!");
            }
        }
    }

    Mesh SimplifyMesh(Mesh originalMesh)
    {
        Mesh simplifiedMesh = new Mesh();
        Vector3[] originalVertices = originalMesh.vertices;
        int[] originalTriangles = originalMesh.triangles;
        Vector2[] originalUVs = originalMesh.uv;

        // Create a dictionary to store shared vertices
        Dictionary<Vector3, int> sharedVertices = new Dictionary<Vector3, int>();
        List<Vector3> uniqueVertices = new List<Vector3>();
        List<Vector2> uniqueUVs = new List<Vector2>();

        // Iterate over triangles to identify shared vertices and copy UVs
        for (int i = 0; i < originalTriangles.Length; i++)
        {
            int originalVertexIndex = originalTriangles[i];
            Vector3 originalVertex = originalVertices[originalVertexIndex];
            Vector2 originalUV = originalUVs[originalVertexIndex];

            // Check if a similar vertex already exists
            int newVertexIndex;
            if (TryGetSimilarVertex(originalVertex, uniqueVertices, sharedVertices, out newVertexIndex))
            {
                // If yes, update the triangle index
                originalTriangles[i] = newVertexIndex;
            }
            else
            {
                // If no, add the vertex to the dictionary and keep the triangle index
                newVertexIndex = uniqueVertices.Count;
                sharedVertices.Add(originalVertex, newVertexIndex);
                uniqueVertices.Add(originalVertex);
                uniqueUVs.Add(originalUV);
                originalTriangles[i] = newVertexIndex;
            }
        }

        // Assign simplified vertices, triangles, and UVs to the new mesh
        simplifiedMesh.vertices = uniqueVertices.ToArray();
        simplifiedMesh.triangles = originalTriangles;
        simplifiedMesh.uv = uniqueUVs.ToArray();

        return simplifiedMesh;
    }

    private void Update()
    {
        List<Vector2Int> coordinates = GenerateAxialHexGrid(2, 2);
        List<Vector3> outerVerts = GetOuterVertices(coordinates);
        lineRenderer.positionCount = outerVerts.Count;
        lineRenderer.SetPositions(outerVerts.ToArray());


    }

    private List<Vector3> GetOuterVertices(List<Vector2Int> hexagonCoordinates)
    {
        List<Vector3> outerVertices = new List<Vector3>();

        foreach (Vector2Int coordinate in hexagonCoordinates)
        {

            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighborCoordinate = coordinate + HexMetrics.GetNeighborOffsets(i);
                // Check if the neighbor is not in the collection
                if (!hexagonCoordinates.Contains(neighborCoordinate))
                {
                    // Get the corners of the missing edge
                    Vector3 corner1 = AxialToWorld(coordinate) + corners[i];
                    Vector3 corner2 = AxialToWorld(coordinate) + corners[i + 1];

                    // Add corners to the list
                    outerVertices.Add(corner1);
                    outerVertices.Add(corner2);
                }
            }
        }

        // Remove duplicate and very close points
        outerVertices = RemoveDuplicateAndClosePoints(outerVertices);


        // Arrange the list of points in clockwise order
        outerVertices = ArrangeClockwise(outerVertices);

        outerVertices.Add(outerVertices[0]);

        return outerVertices;
    }

    private List<Vector3> RemoveDuplicateAndClosePoints(List<Vector3> points)
    {
        List<Vector3> cleanedPoints = new List<Vector3>();

        foreach (Vector3 point in points)
        {
            bool isClose = false;

            // Check if the point is close to any existing point in the cleaned list
            foreach (Vector3 cleanedPoint in cleanedPoints)
            {
                if (Vector3.Distance(point, cleanedPoint) < 0.001f)
                {
                    isClose = true;
                    break;
                }
            }

            if (!isClose)
            {
                cleanedPoints.Add(point);
            }
        }

        return cleanedPoints;
    }

    private List<Vector3> ArrangeClockwise(List<Vector3> points)
    {
        // Calculate the center as the average of all points
        Vector3 center = Vector3.zero;
        foreach (Vector3 point in points)
        {
            center += point;
        }
        center /= points.Count;

        // Sort points based on the angle with respect to the center
        points.Sort((a, b) => Mathf.Atan2(a.z - center.z, a.x - center.x).CompareTo(Mathf.Atan2(b.z - center.z, b.x - center.x)));

        return points;
    }

    private bool IsClockwise(Vector3 a, Vector3 b, Vector3 c)
    {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x) < 0;
    }
    bool TryGetSimilarVertex(Vector3 vertex, List<Vector3> uniqueVertices, Dictionary<Vector3, int> sharedVertices, out int index)
    {
        for (int i = 0; i < uniqueVertices.Count; i++)
        {
            Vector3 uniqueVertex = uniqueVertices[i];
            if (Vector3.Distance(vertex, uniqueVertex) < vertexComparisonThreshold)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private Vector2Int WorldToAxial(Vector3 worldPosition)
    {
        float x = worldPosition.x / (HexMetrics.innerRadius * 2f);
        float y = -x;

        float offset = worldPosition.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        // fix rounding errors clicking on edges of hexes.
        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new Vector2Int(iX, iZ);
    }

    private Vector2Int RoundToHexCoordinates(float q, float r)
    {
        int rx = Mathf.RoundToInt(q);
        int ry = Mathf.RoundToInt(r);
        int rz = Mathf.RoundToInt(-q - r);

        float x_diff = Mathf.Abs(rx - q);
        float y_diff = Mathf.Abs(ry - r);
        float z_diff = Mathf.Abs(rz - (-q - r));

        if (x_diff > y_diff && x_diff > z_diff)
        {
            rx = -ry - rz;
        }
        else if (y_diff > z_diff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }

        return new Vector2Int(rx, rz);
    }

    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    public static Vector2[] uvCorners = {
        new Vector2(0.5f, 1f),
        new Vector2(1f, 0.75f),
        new Vector2(1f, 0.25f),
        new Vector2(0.5f, 0f),
        new Vector2(0f, 0.25f),
        new Vector2(0f, 0.75f),
        new Vector2(0.5f, 1f)
    };

    void DrawMeshOutline(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Find outer edges
        List<Edge> outerEdges = FindOuterEdges(vertices, triangles);

        // Create a list to store the ordered points
        List<Vector3> orderedPoints = new List<Vector3>();

        // Start with the first edge
        Edge currentEdge = outerEdges[0];
        orderedPoints.Add(vertices[currentEdge.Vertex1]);
        orderedPoints.Add(vertices[currentEdge.Vertex2]);

        outerEdges.RemoveAt(0);

        // Find the next connected edges until the loop is closed
        int safety = 0;
        while (outerEdges.Count > 0 && safety < 1000)
        {
            safety++;
            Edge nextEdge = FindConnectedEdge(currentEdge, outerEdges);

            // Remove the edge from the list
            outerEdges.Remove(nextEdge);

            // Add the next vertex to the ordered list
            int nextVertex = nextEdge.Vertex1 == currentEdge.Vertex2 ? nextEdge.Vertex2 : nextEdge.Vertex1;
            orderedPoints.Add(vertices[nextVertex]);

            // Update the current edge
            currentEdge = nextEdge;
        }

        // Draw the ordered points using Debug.DrawLine
        for (int i = 0; i < orderedPoints.Count - 1; i++)
        {
            orderedPoints[i] += new Vector3(0, 0.1f, 0);

        }

        // Draw the last line connecting the last and first points to close the loop
        
        orderedPoints.Add(orderedPoints[1]);
        lineRenderer.positionCount = orderedPoints.Count;
        lineRenderer.SetPositions(orderedPoints.ToArray());
    }

    Edge FindConnectedEdge(Edge currentEdge, List<Edge> edges)
    {
        foreach (Edge edge in edges)
        {
            if (edge.Vertex1 == currentEdge.Vertex2 || edge.Vertex2 == currentEdge.Vertex2)
            {
                return edge;
            }
        }

        // This should not happen if the loop is closed
        return currentEdge;
    }
    List<Edge> FindOuterEdges(Vector3[] vertices, int[] triangles)
    {
        List<Edge> outerEdges = new List<Edge>();

        // Count the number of triangles each edge is part of
        Dictionary<Edge, int> edgeTriangleCount = new Dictionary<Edge, int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int j = 0; j < 3; j++)
            {
                Edge edge = new Edge(triangles[i + j], triangles[i + (j + 1) % 3]);

                if (edgeTriangleCount.ContainsKey(edge))
                {
                    edgeTriangleCount[edge]++;
                }
                else
                {
                    edgeTriangleCount[edge] = 1;
                }
            }
        }

        // Identify edges shared by only one triangle as outer edges
        foreach (var pair in edgeTriangleCount)
        {
            if (pair.Value == 1)
            {
                outerEdges.Add(pair.Key);
            }
        }

        return outerEdges;
    }

    bool IsOuterEdge(Edge edge, HashSet<Edge> allEdges)
    {
        return allEdges.Contains(edge) && allEdges.Count(e => e.Equals(edge) || e.Equals(edge.Reversed())) == 1;
    }

    struct Edge
    {
        public int Vertex1;
        public int Vertex2;

        public Edge(int vertex1, int vertex2)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }

        public Edge Reversed()
        {
            return new Edge(Vertex2, Vertex1);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge)) return false;
            Edge other = (Edge)obj;
            return (Vertex1 == other.Vertex1 && Vertex2 == other.Vertex2) ||
                   (Vertex1 == other.Vertex2 && Vertex2 == other.Vertex1);
        }

        public override int GetHashCode()
        {
            return Vertex1.GetHashCode() ^ Vertex2.GetHashCode();
        }
    }

}

public class TriangleInfo
{
    public int[] vertices;
    public Vector2[] uv;

    public TriangleInfo(int[] vertices, Vector2[] uv)
    {
        this.vertices = vertices;
        this.uv = uv;
    }
}