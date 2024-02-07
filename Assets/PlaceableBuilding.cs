using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Placeable Building", menuName = "Custom/Placeable Building")]
public class PlaceableBuilding : ScriptableObject
{
    public string title;
    public GameObject constructionSitePrefab;
    public GameObject completedBuildingPrefab;

    public Sprite uiImage;

    public List<Vector2Int> coordinates = new List<Vector2Int>();
}