using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HexMap : MonoBehaviour
{

    public Dictionary<Vector2Int, PlacedBuilding> buildingLocations = new Dictionary<Vector2Int, PlacedBuilding>();

    public List<Vector2Int> buildingPositions {
        get {
            return buildingLocations.Keys.ToList();
        }
    }

    public List<PlacedBuilding> placedBuildings = new List<PlacedBuilding>();

    void PlaceBuilding(PlacedBuilding building, Vector2Int position, int rotation) {

        placedBuildings.Add(building);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
