using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickMeshTest : MonoBehaviour
{

    MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        Hexagons.setHexSize(10f);

        List<Vector2Int> layerHexes = Hexagons.GetNeighborsInRange(new Vector2Int(0, 0), 5);
        meshFilter.mesh = Hexagons.GenerateThickMesh(layerHexes);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
