using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcavationFlagController : MonoBehaviour
{

    BaseLayers baseLevels;

    // Start is called before the first frame update
    void Start()
    {
        baseLevels = FindObjectOfType<BaseLayers>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            // the top layer is not excavatable
            if (GameManager.Instance.currentLevelView != 0)
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Debug.DrawLine(Vector3.zero, hit.point, Color.red, 10f);

                    // only allow excavation of tiles which have an excavated neighbor
                    Vector2Int hexPosition = Hexagons.WorldToHex(hit.point);
                    if (baseLevels.layers[GameManager.Instance.currentLevelView].positionMap[hexPosition] == -2) return;
                    if (baseLevels.layers[GameManager.Instance.currentLevelView].hasExposedNeighbors(hexPosition))
                    {
                        // Version 1 - base prototype
                        // clear a space on the board by a click - like you would during excavation.
                        // Debug.Log(string.Format("Hit position {0}, object {1}", hexPosition, hit.collider.gameObject.name));
                        // layers[currentLayer].positionMap[hexPosition] = -2;
                        // layers[currentLayer].UpdateMesh(currentLayer != 0);

                        // Version 2 - queue excavation
                        // mark a space on the board as being ready to excavate
                        baseLevels.layers[GameManager.Instance.currentLevelView].MarkExcavation(hexPosition);

                    }


                }
            }
        }
    }
}
