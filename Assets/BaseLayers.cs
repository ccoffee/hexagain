using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseLayers : MonoBehaviour
{

    // public int currentLayer = 0;

    List<Transform> layerTransforms = new List<Transform>();
    public List<BaseLayer> layers = new List<BaseLayer>();

    public List<float> layerSizes = new List<float>();
    public float[] layerOffsets;

    public int maxDepth {
        get {
            return layers.Count - 1;
        }
    }

    public Material layerMaterial;

    public Transform cameraBoomTransform;

    public static float layerSize = 10f;

    public static BaseLayers current;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
        CreateLayers();
    }

    void CreateLayers() {

        layerOffsets = new float[layerSizes.Count];

        int numberOfLayers = GameManager.Instance.tileMap.depth;
        Debug.Log("NUMBER OF LEVELS: " + numberOfLayers);

        Hexagons.setHexSize(10f);

        List<Vector2Int> generatePositions = new List<Vector2Int>();
        generatePositions = Hexagons.GetNeighborsInRange(new Vector2Int(0, 0), 5);

        List<Vector2Int> outerPositions = new List<Vector2Int>();
        outerPositions = Hexagons.GetNeighborsInRange(new Vector2Int(0, 0), 6);

        float layerOffset = 0;

        for (int i = 0; i < numberOfLayers; i++) {
            GameObject newLayer = new GameObject("Layer" + i);
            BaseLayer baseLayer = newLayer.AddComponent<BaseLayer>();

            GameObject newLayerVisual = new GameObject("LayerVisual");
            newLayerVisual.transform.SetParent(newLayer.transform);
            newLayerVisual.transform.localPosition = new Vector3(0, (BaseLayers.layerSize / 2), 0);

            MeshFilter meshFilter = newLayerVisual.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = newLayerVisual.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = newLayerVisual.AddComponent<MeshCollider>();

            baseLayer.positionMap = new Dictionary<Vector2Int, int>();
            baseLayer.meshCollider = meshCollider;
            baseLayer.meshRenderer = meshRenderer;
            baseLayer.meshFilter = meshFilter;

            outerPositions.ForEach((genPos) =>
            {
                baseLayer.positionMap[genPos] = 0;
            });

            meshFilter.mesh = Hexagons.GenerateThickMesh(outerPositions, null, layerSize);
            baseLayer.meshCollider.sharedMesh = Hexagons.GenerateMesh(generatePositions);
            meshRenderer.material = layerMaterial;
            
            newLayer.transform.SetParent(transform);
            newLayer.layer = i;

            layerOffsets[i] = layerOffset;

            newLayer.transform.localPosition = new Vector3(0, layerOffsets[i], 0);

            layerOffset += layerSizes[i];

            layers.Add(baseLayer);
            if (i == 0)
            {
                ClearPosition(i, Vector2Int.zero);
            } else {
                layers[i].UpdateMesh(i != 0);
            }
        }
    }

    public void ClearPosition(int layer, Vector2Int position) {
        layers[layer].positionMap[position] = -2;
        layers[layer].UpdateMesh(layer != 0);
    }

    // Update is called once per frame
    void Update()
    {

        /*
        if (Input.mouseScrollDelta.y != 0) {
            float mouseScroll = Input.mouseScrollDelta.y;

            currentLayer = (int) Mathf.Clamp(currentLayer + (mouseScroll > 0 ? 1 : -1), 0, GameManager.Instance.elevator.depth);

            cameraBoomTransform.transform.position = layers[currentLayer].transform.position;

            for (int i = 0; i < currentLayer; i++) {
                layers[i].gameObject.SetActive(false);
            }
            for (int i = currentLayer; i <= maxDepth; i++)
            {
                layers[i].UpdateMesh(i == currentLayer && i != 0); // i == currentLayer && i != 0, i != currentLayer);
                layers[i].gameObject.SetActive(true);
                if (i != currentLayer) {
                    layers[i].meshCollider.enabled = false;
                } else {
                    layers[i].meshCollider.enabled = true;
                }
            }
        }
        */

        
    }
}
