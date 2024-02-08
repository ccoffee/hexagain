using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseLayer : MonoBehaviour
{

    public Dictionary<Vector2Int, int> positionMap = new Dictionary<Vector2Int, int>();
    public Dictionary<Vector2Int, float> excavationData = new Dictionary<Vector2Int, float>(); 

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public bool hasExposedNeighbors(Vector2Int position) {

        bool value = false;

        Vector2Int[] neighbors = Hexagons.GetNeighbors(position);
        for (int n = 0; n < neighbors.Length; n++)
        {
            int foundVal;
            if (positionMap.TryGetValue(neighbors[n], out foundVal))
            {
                // is the position cleared?
                if (foundVal == -2)
                {
                    value = true;
                }
            }
        }

        return value;
    }

    public void MarkExcavation(Vector2Int target) {
        if (!excavationData.ContainsKey(target)) {
            excavationData[target] = 0;
            positionMap[target] = -1;
        }
    }

    public void UpdateMesh(bool drawOnlyExposed = false, bool highlightAll = false) {

        List<Vector2Int> generatePositions = new List<Vector2Int>();
        List<Vector2Int> hiddenPositions = new List<Vector2Int>();

        positionMap.Keys.ToList().ForEach((key) =>
        {
            if (positionMap[key] > -2) {

                bool draw = false;

                if (drawOnlyExposed)
                {
                    // hexagons on the borders are displayed differently than those surrounded
                    Vector2Int[] neighbors = Hexagons.GetNeighbors(key);
                    for (int n = 0; n < neighbors.Length; n++)
                    {
                        int foundVal;
                        if (positionMap.TryGetValue(neighbors[n], out foundVal))
                        {
                            // is the position cleared?
                            if (foundVal == -2)
                            {
                                draw = true;
                            }
                        }
                    }
                }

                generatePositions.Add(key);

                if (!draw && drawOnlyExposed || highlightAll) {
                    hiddenPositions.Add(key);
                }

            } else {
                // Debug.Log(string.Format("Position not in mesh, removing hex: {0}", key));
            }
        });

        meshFilter.mesh = Hexagons.GenerateThickMesh(generatePositions, hiddenPositions);
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
