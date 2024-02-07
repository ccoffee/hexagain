using UnityEngine;

public class Building : MonoBehaviour
{
    public float constructionTime = 10f; // Adjust as needed
    private float constructionProgress = 0f;

    public PlacedBuilding placedBuilding; // Reference to the PlaceableBuilding scriptable object

    public void ConstructBuilding(float points)
    {
        constructionProgress += points;
        // Update visual representation or perform other actions based on construction progress

        if (constructionProgress >= constructionTime)
        {
            CompleteConstruction();
        }
    }

    void CompleteConstruction()
    {
        // Instantiate the completed building prefab
        Instantiate(placedBuilding.buildingType.completedBuildingPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}