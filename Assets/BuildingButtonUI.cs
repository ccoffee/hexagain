using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButtonUI : MonoBehaviour
{

    public BuildingSelectionUI parent;
    public PlaceableBuilding placeableBuilding;

    public Image buildingImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clicked() {
        parent.StartPlacement(placeableBuilding);
    }

    public void UpdateButton(BuildingSelectionUI _parent, PlaceableBuilding building) {

        placeableBuilding = building;
        parent = _parent;
        buildingImage.sprite = building.uiImage;

    }
}
