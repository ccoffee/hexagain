using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayeredMap : MonoBehaviour
{

    public int currentViewLayer = 0;

    List<Transform> layerTransforms = new List<Transform>();
    public List<BaseLayer> layers = new List<BaseLayer>();

    public int maxDepth {
        get {
            return layers.Count - 1;
        }
    }

    public int unlockedDepth = 0;
    public Material layerMaterial;
    public Transform cameraBoomTransform;
    public static float layerSize = 5f;

    public void CreateGroundLayers() {

        int numberOfLayers = 6;

        List<Vector2Int> generatePositions = new List<Vector2Int>();
        generatePositions = Hexagons.GetNeighborsInRange(new Vector2Int(0, 0), 5);

        List<Vector2Int> outerPositions = new List<Vector2Int>();
        outerPositions = Hexagons.GetNeighborsInRange(new Vector2Int(0, 0), 6);

        for (int i = 0; i < numberOfLayers; i++)
        {
            GameObject newLayer = new GameObject("Layer" + i);
            BaseLayer baseLayer = newLayer.AddComponent<BaseLayer>();
            MeshFilter meshFilter = newLayer.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = newLayer.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = newLayer.AddComponent<MeshCollider>();

            baseLayer.positionMap = new Dictionary<Vector2Int, int>();
            baseLayer.meshCollider = meshCollider;
            baseLayer.meshRenderer = meshRenderer;
            baseLayer.meshFilter = meshFilter;

            outerPositions.ForEach((genPos) =>
            {
                baseLayer.positionMap[genPos] = 0;
            });

            meshFilter.mesh = Hexagons.GenerateThickMesh(outerPositions);
            baseLayer.meshCollider.sharedMesh = Hexagons.GenerateMesh(generatePositions);
            meshRenderer.material = layerMaterial;

            newLayer.transform.SetParent(transform);
            newLayer.layer = 0;
            newLayer.transform.localPosition = new Vector3(0, (float)i * -layerSize, 0);

            layers.Add(baseLayer);
            if (i == 0)
            {
                ClearPosition(i, Vector2Int.zero);
            }
            else
            {
                layers[i].UpdateMesh(i != 0);
            }
        }
    }

    public void ClearPosition(int layer, Vector2Int position)
    {
        layers[layer].positionMap[position] = -2;
        layers[layer].UpdateMesh(layer != 0);
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
