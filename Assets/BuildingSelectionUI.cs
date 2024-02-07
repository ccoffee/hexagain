using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelectionUI : MonoBehaviour
{

    public List<PlaceableBuilding> placeableBuildings = new List<PlaceableBuilding>();

    public GameObject buttonPrefab;

    public List<BuildingButtonUI> spawnedButtons = new List<BuildingButtonUI>();

    public Transform buttonContainerTransform;

    public void UpdateMenu() {
        
        for (int i = 0; i < placeableBuildings.Count; i++) {
            if (spawnedButtons.Count <= i) {
                spawnedButtons.Add(Instantiate(buttonPrefab, buttonContainerTransform).GetComponent<BuildingButtonUI>());
            }
            spawnedButtons[i].UpdateButton(this, placeableBuildings[i]);
        }

        while (spawnedButtons.Count > placeableBuildings.Count) {
            BuildingButtonUI toRemove = spawnedButtons[spawnedButtons.Count - 1];
            spawnedButtons.Remove(toRemove);
            if (Application.isPlaying) {
                Destroy(toRemove.gameObject);
            } else {
                DestroyImmediate(toRemove.gameObject);
            }
        }

    }

    public void OnEnable()
    {
        UpdateMenu();
    }

    public void StartPlacement(PlaceableBuilding building) {
        FindObjectOfType<BuildingPlacement>().StartPlacement(building);
    }

    public void CloseMenu() {
        
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
