using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class MapBuildingDebug : MonoBehaviour
{

    Dictionary<Vector2Int, GameObject> debugObjects = new Dictionary<Vector2Int, GameObject>();

    public GameObject textPrefab;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        GameManager.Instance.currentMap.buildingPositions.ForEach((buildingPosition) =>
        {
            if (debugObjects.ContainsKey(buildingPosition)) {
                
            } else {
                GameObject newGO = Instantiate(textPrefab, transform);
                newGO.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.currentMap.buildingLocations[buildingPosition].buildingType.title;

                newGO.transform.position = Hexagons.HexToWorld(buildingPosition) + new Vector3(0, 1f, 0) ;
           
                debugObjects.Add(buildingPosition, newGO);
            }
        });

    }
}
