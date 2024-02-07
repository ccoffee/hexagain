using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct HexCoordinates
{

	[SerializeField]
	public int x, z;


	public int X { 
		get
		{
			return x;
		}
	}

	public int Z
	{
		get
		{
			return z;
		}
	}

	public HexCoordinates(int x, int z)
	{
		this.x = x;
		this.z = z;
	}

	public static HexCoordinates FromOffsetCoordinates(int x, int z)
	{
		return new HexCoordinates(x - z / 2, z);
	}

	public int Y
	{
		get
		{
			return -X - Z;
		}
	}

	public override string ToString()
	{
		return "(" +
			X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines()
	{
		return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
	}

	public static HexCoordinates FromPosition(Vector3 position)
	{
		float x = position.x / (HexMetrics.innerRadius * 2f);
		float y = -x;

		float offset = position.z / (HexMetrics.outerRadius * 3f);
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

		return new HexCoordinates(iX, iZ);
	}

	public List<HexCoordinates> GetNeighborsInRange(int range)
	{
		List<HexCoordinates> neighbors = new List<HexCoordinates>();

		for (int dx = -range; dx <= range; dx++)
		{
			for (int dz = Mathf.Max(-range, -dx - range); dz <= Mathf.Min(range, -dx + range); dz++)
			{
				int dy = -dx - dz;

				HexCoordinates neighbor = new HexCoordinates(X + dx, Z + dz);

				neighbors.Add(neighbor);
			}
		}

		return neighbors;
	}

	public static List<HexCoordinates> GetNeighborsInRange(HexCoordinates coordinates, int range)
	{
		List<HexCoordinates> neighbors = new List<HexCoordinates>();

		for (int dx = -range; dx <= range; dx++)
		{
			for (int dz = Mathf.Max(-range, -dx - range); dz <= Mathf.Min(range, -dx + range); dz++)
			{
				int dy = -dx - dz;

				HexCoordinates neighbor = new HexCoordinates(coordinates.X + dx, coordinates.Z + dz);

				neighbors.Add(neighbor);
			}
		}

		neighbors.Add(coordinates);

		return neighbors;
	}
}