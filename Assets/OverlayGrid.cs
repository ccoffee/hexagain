using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OverlayGrid : MonoBehaviour
{

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    public Material overlayMaterial;

    public Camera worldCamera;

    public int mapSize = 50;

    public float fadeStartDistance = 140f;
    public float fadeEndDistance = 160f;

    bool gridEnabled = true;

    public Transform cameraTransform;

    float easeOutBounce(float x) {
        float n1 = 7.5625f;
            float d1 = 2.75f;

        if (x < 1 / d1) {
            return n1* x * x;
        } else if (x < 2f / d1)
        {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        }
        else if (x < 2.5 / d1)
        {
            return n1 * (x -= 2.25f / d1) * x + 0.8375f;
        }
        else
        {
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }

    IEnumerator ShowGrid()
    {

        float elapsed = 0f;
        float duration = 0.25f;

        while (elapsed < duration) {

            elapsed += Time.deltaTime;

            // Set the _DistanceFadeStart and _DistanceFadeEnd properties in the material
            meshRenderer.material.SetFloat("_DistanceFadeStart", Mathf.Lerp(0, fadeStartDistance, easeOutBounce(elapsed / duration)));
            meshRenderer.material.SetFloat("_DistanceFadeEnd", Mathf.Lerp(0, fadeStartDistance + 20f, easeOutBounce(elapsed / duration)));

            yield return null;

        }

    }

    IEnumerator HideGrid()
    {

        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {

            elapsed += Time.deltaTime;

            // Set the _DistanceFadeStart and _DistanceFadeEnd properties in the material
            meshRenderer.material.SetFloat("_DistanceFadeStart", Mathf.Lerp(fadeStartDistance, 0, elapsed / duration));
            meshRenderer.material.SetFloat("_DistanceFadeEnd", Mathf.Lerp(fadeStartDistance + 20f, 0, elapsed / duration));

            yield return new WaitForEndOfFrame();

        }

    }

    // Start is called before the first frame update
    void Start()
    {

        if (!cameraTransform)
        {
            cameraTransform = FindObjectOfType<CameraMovement>().transform;
        }

        Hexagons.setHexSize(10f);

        if (!worldCamera) {
            worldCamera = Camera.main;
        }

        meshFilter = meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = overlayMaterial ? overlayMaterial : new Material(Shader.Find("Standard"));

        RedrawMesh();
        // meshFilter.mesh = Hexagons.GenerateMesh(Hexagons.GenerateHexGrid(mapSize, mapSize, true));
        //meshCollider.sharedMesh = meshFilter.mesh;

        

    }

    public void RedrawMesh() {
        List<Vector2Int> positions = GameManager.Instance.currentMap.buildingLocations.Keys.ToList();

        meshFilter.mesh = Hexagons.GenerateMesh(Hexagons.GenerateHexGrid(mapSize, mapSize, true), positions);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            gridEnabled = !gridEnabled;
            if (gridEnabled)
            {
                StartCoroutine(ShowGrid());
            } else {
                StartCoroutine(HideGrid());
            }
        }

        meshRenderer.material.SetVector("_WorldPosition", cameraTransform.position);
    }
}
