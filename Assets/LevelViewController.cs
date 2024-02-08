using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelViewController : MonoBehaviour
{

    int currentLevel;

    Transform cameraBoomTransform;
    BaseLayers baseLevels;

    // Start is called before the first frame update
    void Start()
    {
        cameraBoomTransform = Camera.main.transform.parent.parent;
        baseLevels = FindObjectOfType<BaseLayers>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            float mouseScroll = Input.mouseScrollDelta.y;

            int unlockedDepth = GameManager.Instance.elevator.depth;

            currentLevel = (int)Mathf.Clamp(currentLevel + (mouseScroll > 0 ? 1 : -1), 0, unlockedDepth);
            GameManager.Instance.currentLevelView = currentLevel;

            cameraBoomTransform.transform.position = baseLevels.layers[currentLevel].transform.position;

            for (int i = 0; i < currentLevel; i++)
            {
                baseLevels.layers[i].gameObject.SetActive(false);
            }
            for (int i = currentLevel; i < GameManager.Instance.tileMap.depth; i++)
            {
                baseLevels.layers[i].UpdateMesh(i == currentLevel && i != 0); // i == currentLayer && i != 0, i != currentLayer);
                baseLevels.layers[i].gameObject.SetActive(true);
                if (i != currentLevel)
                {
                    baseLevels.layers[i].meshCollider.enabled = false;
                }
                else
                {
                    baseLevels.layers[i].meshCollider.enabled = true;
                }
            }
        }
    }
}
