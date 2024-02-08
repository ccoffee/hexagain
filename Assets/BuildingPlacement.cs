using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;

public class BuildingPlacement : MonoBehaviour
{
    public PlaceableBuilding placeableBuilding; // Assign your PlaceableBuilding scriptable object in the inspector

    public Transform previewTransform;
    public BuildingPreview buildingPreview;

    int rotation = 0;

    public bool placing = false;

    // NOTE: this class does not support multi-layer maps

    public void StartPlacement(PlaceableBuilding building) {
        placing = true;
        placeableBuilding = building;
        buildingPreview.building = building;
    }

    bool ValidatePlacement(Vector2Int hexPosition) {


        Debug.Log(string.Format("position is empty: {0}", hexPosition));
        PlacedBuilding newBuilding = new PlacedBuilding(placeableBuilding, hexPosition, rotation);

        for (int i = 0; i < newBuilding.allCoordinates.Length; i++)
        {
            if (GameManager.Instance.buildingMap.buildingLocations.ContainsKey(newBuilding.allCoordinates[i]))
            {
                Debug.Log(string.Format("position is not empty: {0}", newBuilding.allCoordinates[i]));
                return false;
            }
            else
            {
                Debug.Log(string.Format("position is empty: {0}", newBuilding.allCoordinates[i]));
            }
        }
        return true;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.B)) {
            placing = !placing;
        }

        if (!placing) {
            buildingPreview.Hide();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector2Int hexPosition = Hexagons.WorldToHex(hit.point);

                // check position validity
                // create a new building object
                if (ValidatePlacement(hexPosition)) {
                    PlacedBuilding newPlacedBuilding = new PlacedBuilding(placeableBuilding, hexPosition, rotation);
                    GameObject constructionSitePrefab = placeableBuilding.constructionSitePrefab;
                    GameObject newGameObject = Instantiate(constructionSitePrefab, Hexagons.HexToWorld(hexPosition), Quaternion.Euler(new Vector3(0, -30 + (60 * rotation), 0)));
                    Building newBuilding = newGameObject.GetComponentInChildren<Building>();
                    newBuilding.placedBuilding = newPlacedBuilding;

                    GameManager.Instance.buildingMap.placedBuildings.Add(newPlacedBuilding);
                    for (int i = 0; i < newPlacedBuilding.allCoordinates.Length; i++)
                    {
                        Debug.Log(string.Format("Setting map location for building {0}", newPlacedBuilding.allCoordinates[i]));
                        GameManager.Instance.buildingMap.buildingLocations[newPlacedBuilding.allCoordinates[i]] = newPlacedBuilding;
                    }

                    FindObjectOfType<OverlayGrid>()?.RedrawMesh();

                    UpdateNavMesh();

                    placing = false;

                } else {
                    // Debug.Log("POSITION IS NOT EMPTY");
                }

            }
        } else if (Input.GetKeyDown(KeyCode.R)) {

            rotation = (int)Mathf.Repeat(rotation + 1, 6);
            buildingPreview.Rotate(rotation);
        } else {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Debug.Log(hit.point);
                Vector2Int hexPosition = Hexagons.WorldToHex(hit.point);
                // Debug.Log(hexPosition);
                Vector3 offset = new Vector3(0, GameManager.Instance.currentLevelView * BaseLayers.layerSize - BaseLayers.layerSize / 2, 0);

                previewTransform.position = Hexagons.HexToWorld(hexPosition) - offset;
                //BaseLayers.layerSize * BaseLayers.current.currentLayer; 

                // need to get the localized positions in the preview mesh based on the position to show any blocked tiles
                buildingPreview.DrawMesh();
            }
        }
    }

    private void Start()
    {
        UpdateNavMesh();
    }

    void UpdateNavMesh() {
        // Reference to your NavMeshSurface component
        NavMeshSurface navMeshSurface = FindObjectOfType<NavMeshSurface>();

        // Example method to trigger NavMesh baking
        // Check if the NavMeshSurface component is assigned
        if (navMeshSurface != null)
        {
            // Bake the NavMesh
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }
    }

}