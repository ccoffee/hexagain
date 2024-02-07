using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics {
    public const float outerRadius = 10f;
	public const float innerScale = 0.866025404f;
    public const float innerRadius = outerRadius * innerScale;

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

	public static Vector2[] UVCoordinates = new Vector2[] {
		new Vector2(0.5f, 0.5f), new Vector2()
	};

	public static Vector2Int GetNeighborOffsets(int index)
	{
		// Define the six possible neighbors in axial coordinates
		Vector2Int[] neighbors = new Vector2Int[]
		{
		new Vector2Int(0, 1),   // Top-Right
		new Vector2Int(1, 0),  // Right
        new Vector2Int(1, -1), // Bottom-Right
        new Vector2Int(0, -1), // Bottom-Left
        new Vector2Int(-1, 0), // Left
        new Vector2Int(-1, 1), // Top-Left
        
		};

		return neighbors[index];
	}
}