using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacedBuilding {

    public Vector2Int centerCoordinate = Vector2Int.zero;

    public Vector2Int[] allCoordinates;

    public int rotationIndex = 0;

    public float currentHealth = 1f;

    public bool isConstructed;
    public bool isPlaced;
    public float constructionProgress;

    public PlaceableBuilding buildingType;

    public PlacedBuilding(PlaceableBuilding buildingType, Vector2Int centerCoordinate, int rotationIndex, bool isPlaced = true, bool isConstructed = false, float constructionProgress = 0f) {
        this.buildingType = buildingType;
        this.centerCoordinate = centerCoordinate;
        this.rotationIndex = rotationIndex;

        List<Vector2Int> coordinates = Hexagons.Rotate(buildingType.coordinates, rotationIndex);
        List<Vector2Int> updatedCoordinates = new List<Vector2Int>();
        coordinates.ForEach((coordinate) =>
        {
            updatedCoordinates.Add(coordinate + centerCoordinate);
        });
        this.allCoordinates = updatedCoordinates.ToArray();

        this.isConstructed = isConstructed;
        this.isPlaced = isPlaced;
        this.constructionProgress = constructionProgress;

    }

}
