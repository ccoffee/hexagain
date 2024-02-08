using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public PlaceableBuilding building;

    public Material previewMaterial;

    public LineRenderer outlineRenderer;

    public int rotationIndex = 0;

    public void Rotate(int i) {
        rotationIndex = i;
        DrawMesh();
    }

    public void RotateRight() {
        rotationIndex = (int) Mathf.Repeat(rotationIndex + 1, 6);
        DrawMesh();
    }

    // Start is called before the first frame update
    void Start()
    {

        

        Hexagons.setHexSize(10f);
        meshFilter = meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = previewMaterial;

        DrawMesh();
    }

    

    // draws the template mesh supporting rotation
    public void DrawMesh() {

        List<Vector2Int> positions = Hexagons.Rotate(building.coordinates, rotationIndex);

        Vector2Int gridPosition = Hexagons.WorldToHex(transform.position);
        List<Vector2Int> blockedPositions = new List<Vector2Int>();

        List<Vector2Int> buildingPositions = GameManager.Instance.buildingMap.buildingPositions;

        positions.ForEach((position) =>
        {
            Vector2Int newWorldPosition = gridPosition + position;
            if (buildingPositions.Contains(newWorldPosition)) {
                blockedPositions.Add(position);
            }
        });


        meshFilter.mesh = Hexagons.GenerateMesh(positions, blockedPositions);
        meshRenderer.enabled = true;

        List<Vector3> outlinePositions = Hexagons.GetOuterVertices(positions);

        outlineRenderer.positionCount = outlinePositions.Count;
        outlineRenderer.SetPositions(outlinePositions.ToArray());

    }

    public void DrawOutline(List<Vector2Int> positions) {
        List<Vector3> outlinePositions = Hexagons.GetOuterVertices(positions);

        outlineRenderer.positionCount = outlinePositions.Count;
        outlineRenderer.SetPositions(outlinePositions.ToArray());
        outlineRenderer.enabled = true;
    }

    public void Hide() {
        outlineRenderer.enabled = false;
        meshRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
