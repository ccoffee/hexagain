using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcavationAgent : MonoBehaviour
{

    // this script periodically searches for excavation tiles to do work on and contributes progress until completed - when completed the tile is cleared.

    // the current layer this excavation agent is on;
    public int layer;

    // current target, if any
    public Vector2Int currentTarget;
    public bool working = false;

    // Start is called before the first frame update
    void Start()
    {

        InvokeRepeating("checkStatus", 0.5f, 0.5f);

    }

    void checkStatus() {

        if (working) return;

        Dictionary<Vector2Int, float> excavationData = BaseLayers.current.layers[layer].excavationData;

        float shortestDistance = 10000f;
        Vector2Int closestTile = Vector2Int.zero;
        bool foundTarget = false;
        foreach (Vector2Int key in excavationData.Keys) {

            float testDistance = Hexagons.GetDistance(Hexagons.WorldToHex(transform.position), key);
            if (testDistance < shortestDistance) {
                shortestDistance = testDistance;
                closestTile = key;
                foundTarget = true;
            }

        }

        if (foundTarget) {
            currentTarget = closestTile;
            // in the future the unit will need to path to the space in order to start working, for prototype we just start working right away.
            working = true;
            if (excavationData[closestTile] == 0) {
                excavationData[closestTile] = float.Epsilon;
            }
        }

    }
    // Update is called once per frame
    void Update()
    {

        // if we have a target, work on it
        if (working) {
            if (BaseLayers.current.layers[layer].excavationData.ContainsKey(currentTarget)) {
                float completionValue = BaseLayers.current.layers[layer].excavationData[currentTarget];
                BaseLayers.current.layers[layer].excavationData[currentTarget] = Mathf.Clamp(completionValue + Time.deltaTime, 0, 1);

                // if the work is completed, clear the map, allowing building and visibly show the tile as clear
                if (BaseLayers.current.layers[layer].excavationData[currentTarget] == 1) {
                    working = false;
                    BaseLayers.current.layers[layer].positionMap[currentTarget] = -2;
                    BaseLayers.current.layers[layer].excavationData.Remove(currentTarget);
                    currentTarget = Vector2Int.zero;
                    BaseLayers.current.layers[layer].UpdateMesh(BaseLayers.current.currentLayer == layer);
                    GameManager.Instance.excavatedTiles++;
                }
            }
        }

    }
}
