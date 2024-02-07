using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonsDebug : MonoBehaviour
{

    public List<Vector2Int> hexDebugPositions = new List<Vector2Int>();
    public float debugDuration = 100f;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        Hexagons.setHexSize(10f);

        // debug corner positions
        for (int i = 0; i < 6; i++) {
            Debug.Log(string.Format("Corner Position {0} Calc: {1}, Static: {2}", i, Hexagons.PointyHexCorner(i), Hexagons.corners[i] * Hexagons.hexSize));
        }

        // debug uv positions
        for (int i = 0; i < 6; i++)
        {
            Debug.Log(string.Format("UV Corner Position {0} Calc: {1}, Static: {2}", i,(Hexagons.PointyHexCorner2D(i, 0.5f) + new Vector2(0.5f, 0.5f)), Hexagons.uvCorners[i] * Hexagons.hexSize));
        }

        meshFilter = meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter) {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material ? material : new Material(Shader.Find("Standard"));

        meshCollider = gameObject.AddComponent<MeshCollider>();

        // Draw the Hexagon UV texture and save as a file.
        Hexagons.HexUVTexture("HexagonUVDebug");

        // Draw all the hexagons in the provided debug positions list
        hexDebugPositions.ForEach((hexDebugPosition) =>
        {
            Hexagons.DebugDrawHex(hexDebugPosition, debugDuration);
        });

        // test rotation of hexagon pattern
        List<Vector2Int> rotationInputPositions = new List<Vector2Int>() {
            new Vector2Int(3,3), new Vector2Int(3,4), new Vector2Int(4,4)
        };

        rotationInputPositions.ForEach((hexDebugPosition) =>
        {
            Hexagons.DebugDrawHex(hexDebugPosition, debugDuration);
        });

        List<Vector2Int> rotationOutputPositions = new List<Vector2Int>();

        for (int i = 0; i < 6; i++) {
            rotationOutputPositions = new List<Vector2Int>();

            rotationOutputPositions.Add(rotationInputPositions[0]);
            rotationOutputPositions.Add(Hexagons.Rotate(rotationInputPositions[0], rotationInputPositions[1], i+1));
            rotationOutputPositions.Add(Hexagons.Rotate(rotationInputPositions[0], rotationInputPositions[2], i+1));

            rotationOutputPositions.ForEach((hexDebugPosition) =>
            {
                Debug.Log(hexDebugPosition);
                Hexagons.DebugDrawHex(hexDebugPosition + new Vector2Int(4 * (i+1), 0), debugDuration, i);
            });
        }

        meshFilter.mesh = Hexagons.GenerateMesh(Hexagons.GenerateHexGrid(50,50, true));
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
