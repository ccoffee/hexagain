using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExcavationIndicators : MonoBehaviour
{

    public ExcavationIndicator indicatorPrefab;

    public List<ExcavationIndicator> spawnedIndicators = new List<ExcavationIndicator>();

    public void UpdateIndicators() {
        BaseLayer currentLevel = BaseLayers.current.layers[GameManager.Instance.currentLevelView];

        List<Vector2Int> excavations = currentLevel.excavationData.Keys.ToList();

        while (excavations.Count > spawnedIndicators.Count) {
            spawnedIndicators.Add(Instantiate<ExcavationIndicator>(indicatorPrefab, transform));
        }

        for (int i = 0; i < excavations.Count || i < spawnedIndicators.Count; i++) {

            if (excavations.Count > i) {
                Vector3 position = Camera.main.WorldToScreenPoint(Hexagons.HexToWorld(excavations[i]) + new Vector3(0, -BaseLayers.layerSize * GameManager.Instance.currentLevelView, 0));
                spawnedIndicators[i].transform.position = position;
                spawnedIndicators[i].progressBar.sizeDelta = new Vector2(currentLevel.excavationData[excavations[i]] * 32f, spawnedIndicators[i].progressBar.sizeDelta.y);
                spawnedIndicators[i].gameObject.SetActive(true);

            }
            else {
                spawnedIndicators[i].gameObject.SetActive(false);
            }

        }


    }
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateIndicators", 1f/60f, 1f/60f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
