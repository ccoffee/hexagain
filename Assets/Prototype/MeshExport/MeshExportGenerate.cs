using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshExportGenerate : MonoBehaviour
{

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        List<Vector2Int> positions = new List<Vector2Int>() { Vector2Int.zero };

        meshFilter.mesh = Hexagons.GenerateThickMesh(positions);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
