using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingHexagons : MonoBehaviour
{

    public int width = 8;
    public int height = 8;
    public int depth = 8;

    int[,,] mapData;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public int GetVoxelData(Vector3Int position) {
        if (position.x < width && position.y < height && position.z < depth) {
            return mapData[position.x, position.y, position.z];
        }
        return 0;
    }

    public void SetVoxelData(Vector3Int position, int newValue)
    {
        if (position.x < width && position.y < height && position.z < depth)
        {
            mapData[position.x, position.y, position.z] = newValue;
        }
    }

    public void SetVoxelData(Vector4 position) {
        SetVoxelData(new Vector3Int((int)position.x, (int)position.y, (int)position.z), (int) position.w);
    }

    void InitializeVoxelData()
    {
        mapData = new int[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    mapData[x, y, z] = 1;
                    // Set voxel value based on some condition
                    /*
                    if (x < width / 2 && y < height / 2 && z < depth / 2)
                    {
                        mapData[x, y, z] = 1; // Voxel is active
                    }
                    else
                    {
                        mapData[x, y, z] = 0; // Voxel is inactive
                    }
                    */
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Hexagons.CornerGetHexTest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
