using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{

	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;
	MeshCollider meshCollider;
	List<Color> colors;
	List<Vector2> uvs;

	public LineRenderer lineRenderer;

	public bool simplifyMesh = false;

	float vertexComparisonThreshold = 0.4f;

	void Awake()
	{
		meshCollider = gameObject.AddComponent<MeshCollider>();
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		colors = new List<Color>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
	}

	public void Triangulate(HexCell[] cells, bool drawOutline = false)
	{
		hexMesh.Clear();
		vertices.Clear();
		colors.Clear();
		triangles.Clear();
		uvs.Clear();

		List<Vector2> newUVs = new List<Vector2>();
		for (int i = 0; i < cells.Length; i++)
		{
			Triangulate(cells[i]);
		}
		
		hexMesh.vertices = vertices.ToArray();
		hexMesh.uv = uvs.ToArray();
		hexMesh.colors = colors.ToArray();
		hexMesh.triangles = triangles.ToArray();

		if (simplifyMesh)
		{
			Mesh newMesh = SimplifyMesh(hexMesh);

			// MoveRandomVertex(newMesh);
			hexMesh.triangles = newMesh.triangles;
			hexMesh.vertices = newMesh.vertices;
			hexMesh.uv = newMesh.uv;
			hexMesh.colors = newMesh.colors;
			
		}
		Debug.Log(hexMesh.vertices.Count());

		hexMesh.RecalculateNormals();
		
		meshCollider.sharedMesh = hexMesh;

		if (drawOutline)
		{
			DrawMeshOutline(hexMesh);
		}
	}

	void Triangulate(HexCell cell)
	{
		Vector3 center = cell.transform.localPosition;
		for (int i = 0; i < 6; i++)
		{
			AddTriangle(
				center,
				center + HexMetrics.corners[i],
				center + HexMetrics.corners[i + 1]
			);
			AddTriangleColor(cell.color);

			// uvs for individual tile texture, like the grid outline
			Vector2[] triangleUVs = new Vector2[3] {
				new Vector2(0.5f, 0.5f),
				HexMetrics.uvCorners[i],
				HexMetrics.uvCorners[i + 1]
			};
			uvs.AddRange(triangleUVs);
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


	void MoveRandomVertex(Mesh simplifiedMesh)
	{
		Vector3[] vertices = simplifiedMesh.vertices;

		// Choose a random index
		int randomIndex = Random.Range(0, vertices.Length);

		// Move the vertex 1 unit on the X-axis
		vertices[randomIndex] = new Vector3(vertices[randomIndex].x + 1f, vertices[randomIndex].y, vertices[randomIndex].z);

		// Assign the modified vertices back to the simplified mesh
		simplifiedMesh.vertices = vertices;
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
		Debug.DrawLine(orderedPoints[orderedPoints.Count - 1], orderedPoints[0], Color.red, 10f);
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

	void AddTriangleColor (Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}
}