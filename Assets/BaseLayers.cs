using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseLayers : MonoBehaviour
{

    public int currentLayer = 0;

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

    public static BaseLayers current;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
        CreateLayers();
    }

    void CreateLayers() {

        int numberOfLayers = 6;

        Hexagons.setHexSize(10f);

        List<Vector2Int> generatePositions = new List<Vector2Int>();
        generatePositions = Hexagons.GetNeighborsInRange(new Vector2Int(0, 0), 5);

        List<Vector2Int> outerPositions = new List<Vector2Int>();
        outerPositions = Hexagons.GetNeighborsInRange(new Vector2Int(0, 0), 6);

        for (int i = 0; i < numberOfLayers; i++) {
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
            newLayer.transform.localPosition = new Vector3(0, (float)i * -5, 0);

            layers.Add(baseLayer);
            if (i == 0)
            {
                ClearPosition(0, Vector2Int.zero);
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

        if (Input.mouseScrollDelta.y != 0) {
            float mouseScroll = Input.mouseScrollDelta.y;

            currentLayer = (int) Mathf.Clamp(currentLayer + (mouseScroll > 0 ? 1 : -1), 0, unlockedDepth);

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

        if (Input.GetMouseButtonDown(0))
        {

            // the top layer is not excavatable
            if (currentLayer != 0)
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Debug.DrawLine(Vector3.zero, hit.point, Color.red, 10f);

                    // only allow excavation of tiles which have an excavated neighbor
                    Vector2Int hexPosition = Hexagons.WorldToHex(hit.point);
                    if (layers[currentLayer].hasExposedNeighbors(hexPosition)) {

                        // Version 1 - base prototype
                        // clear a space on the board by a click - like you would during excavation.
                        // Debug.Log(string.Format("Hit position {0}, object {1}", hexPosition, hit.collider.gameObject.name));
                        // layers[currentLayer].positionMap[hexPosition] = -2;
                        // layers[currentLayer].UpdateMesh(currentLayer != 0);


                        // Version 2 - queue excavation
                        // mark a space on the board as being ready to excavate
                        layers[currentLayer].MarkExcavation(hexPosition);

                    }


                }
            }
        }
    }
}
