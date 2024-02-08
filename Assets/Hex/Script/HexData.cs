using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexData
{

    private int gridSizeX;
    private int gridSizeY;
    private int gridSizeZ;

    public int depth {
        get {
            return gridSizeZ;
        }
    }

    // HexDataKey holds basic information about high level state of the hex
    // the Z axis of the position is the layer, we use this as the first parameter of the array to make fetching the entire layer faster
    private HexDataKey[,,] hexData;
    // HexState holds extended information about a hex
    private Dictionary<HexDataKey, HexState> hexState = new Dictionary<HexDataKey, HexState>();

    public HexData(int sizeX, int sizeY, int sizeZ)
    {
        gridSizeX = sizeX;
        gridSizeY = sizeY;
        gridSizeZ = sizeZ;
        hexData = new HexDataKey[gridSizeX, gridSizeY, gridSizeZ];

        for (int x = -gridSizeX / 2; x < gridSizeX / 2; x++)
        {
            for (int y = -gridSizeY / 2; y < gridSizeY / 2; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3Int position = new Vector3Int(x, y, z);
                    HexDataKey flyweight = new HexDataKey(new Vector3Int(x, y, z));

                    Vector3Int indices = ConvertToIndices(flyweight.position);
                    hexData[indices.z, indices.x, indices.y] = flyweight;

                    hexState[flyweight] = new HexState(flyweight);
                }
            }
        }

    }

    private Vector3Int ConvertToIndices(Vector3Int position)
    {
        int x = position.x + gridSizeX / 2;
        int y = position.y + gridSizeY / 2;
        int z = position.z;

        return new Vector3Int(x, y, z);
    }

    private Vector2Int ConvertToPosition(Vector3Int indices)
    {
        int x = indices.x - gridSizeX / 2;
        int y = indices.y - gridSizeY / 2;

        return new Vector2Int(x, y);
    }

}
