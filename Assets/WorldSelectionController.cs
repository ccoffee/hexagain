using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldSelectionController : MonoBehaviour
{

    public bool selected = false;
    public GameObject selectedObject;

    public Building selectedBuilding;
    public TestUnit selectedUnit;

    public List<TestUnit> selectedUnits = new List<TestUnit>();

    public LineRenderer outlineRenderer;

    public List<LineRenderer> outlineRenderers = new List<LineRenderer>();

    public Texture2D guiTexture;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        HandleSelectionBox();

        if (selectedUnits.Count > 0 && Input.GetMouseButtonDown(1)) {
            UnitCommandTest uct = FindObjectOfType<UnitCommandTest>();
            uct.CommandUnitMoveGroup(selectedUnits.ToArray(), GetWorldPointOnYAxis(Input.mousePosition));
        }

        /*
        if (Input.GetMouseButtonDown(0))
        {

            if (FindObjectOfType<BuildingPlacement>().placing == true)
            {
                outlineRenderer.enabled = false;
                return;
            }


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                Building hitBuilding = hitObject.GetComponent<Building>();
                TestUnit hitUnit = hitObject.GetComponent<TestUnit>();

                Debug.Log(string.Format("HIT UNIT: {0}", hitUnit));

                if (hitBuilding != null && hitBuilding == selectedBuilding)
                {
                    Deselect();

                }
                else if (hitBuilding != null)
                {
                    selectedUnit = null;
                    selectedBuilding = hitBuilding;
                    DrawOutline(selectedBuilding.placedBuilding.allCoordinates.ToList());

                }
                else if (hitUnit != null)
                {

                    if (hitUnit == selectedUnit)
                    {
                        Deselect();
                    }
                    else
                    {
                        selectedUnit = hitUnit;
                    }
                } 
                else
                {
                    Deselect();
                }
            } else {
                Deselect();
            }
        }

        if (selectedUnit != null) {
            DrawUnitOutline();
        }

        */
    }

    public void Deselect() {
        selected = false;
        selectedBuilding = null;
        selectedUnit = null;
        selectedUnits.Clear();
        outlineRenderer.enabled = false;
        outlineRenderers.ForEach((outlineRenderer) =>
        {
            outlineRenderer.enabled = false;
        });
    }

    Vector2 mousePosition1;
    Vector3 mousePosition1World;
    Vector2 mousePosition2;
    bool isSelecting;
    Rect selectionRect;

    void HandleSelectionBox()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Start selecting
            isSelecting = true;
            mousePosition1 = Input.mousePosition;
            mousePosition1World = GetWorldPointOnYAxis(mousePosition1);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Stop selecting
            isSelecting = false;

            // Perform selection logic
            PerformSelection();
        }

        // Update the second mouse position while dragging
        if (isSelecting)
        {
            mousePosition2 = Input.mousePosition;
        }

        if (selectedUnits.Count > 0) {
            DrawUnitOutlines();
        }
    }


    Vector3 GetWorldPointOnYAxis(Vector3 screenPoint)
    {
        // Create a ray from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);

        // To find the intersection with the Y-axis, we need to find the scalar parameter (t) at which
        // the ray crosses the Y-axis. The equation for the ray is: ray.origin + t * ray.direction.
        // For the Y-axis, the X and Z components are both 0.

        // Calculate the scalar parameter (t) for the Y-axis intersection
        float t = -ray.origin.y / ray.direction.y;

        // Use the parameter to find the intersection point in world space
        Vector3 intersectionPoint = ray.origin + t * ray.direction;

        return intersectionPoint;
    }

    void PerformSelection()
    {
        // Perform your selection logic here
        // For example, you can check if objects are within the dynamically calculated selectionRect



        Vector3 mouseWorld1 = mousePosition1World;

        // Debug.DrawLine(mouseWorld1, mouseWorld1 + new Vector3(0, 10, 0), Color.green, 100f);

        Vector3 mouseWorld2 = GetWorldPointOnYAxis(mousePosition2);

        // Debug.DrawLine(mouseWorld2, mouseWorld2 + new Vector3(0, 10, 0), Color.green, 100f);


        Vector3 center = ((Vector2) Camera.main.WorldToScreenPoint(mousePosition1World) + mousePosition2) * 0.5f;
        Vector3 halfExtents = new Vector3(Mathf.Abs(mouseWorld2.x - mouseWorld1.x) * 0.5f, 10f, Mathf.Abs(mouseWorld2.z - mouseWorld1.z) * 5f);

        Collider[] colliders = Physics.OverlapBox((mouseWorld1 + mouseWorld2) * 0.5f, halfExtents, Quaternion.identity, ~0);

        Vector3[] corners = new Vector3[8];

        // debug draw the overlap box
        /*
        for (int i = 0; i < 4; i++)
        {
            Vector3 cornerScreen = center + Vector3.Scale(halfExtents, new Vector3(Mathf.Pow(-1, i % 2), Mathf.Pow(-1, i / 2), 0.0f));
            Vector3 cornerWorld = Camera.main.ScreenToWorldPoint(cornerScreen);
            corners[i] = cornerWorld;

            int nextIndex = (i + 1) % 2 + 2;
            corners[nextIndex] = cornerWorld;

            Debug.DrawLine(corners[i], corners[nextIndex], Color.red, 100f);
            Debug.Log(corners[i]);
        }
        */

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {

        } else if (Input.GetKey(KeyCode.LeftControl)) {

        } else {
            selectedUnits.Clear();
        }

        foreach (var collider in colliders)
        {
            GameObject obj = collider.gameObject;

            if (obj.CompareTag("Selectable"))
            {

                TestUnit hitUnit = obj.GetComponent<TestUnit>();
                if (hitUnit != null ) {
                    if (Input.GetKey(KeyCode.LeftControl)) {
                        if (selectedUnits.Contains(hitUnit)) {
                            selectedUnits.Remove(hitUnit);
                        } else {
                            selectedUnits.Add(hitUnit);
                        }
                    } else if (!selectedUnits.Contains(hitUnit)) {
                        selectedUnits.Add(hitUnit);
                    }
                }

            }
        }
        Debug.Log(string.Format("Selected {0} units", selectedUnits.Count));

        if (selectedUnits.Count > 0) {
            DrawUnitOutlines();
        } else {
            Deselect();
        }

    }

    Vector3 ProjectToXZPlane(Vector3 screenPosition)
    {
        // Use the camera's projection matrix and view matrix to calculate the intersection point with the X-Z plane
        Matrix4x4 projectionMatrix = Camera.main.projectionMatrix;
        Matrix4x4 viewMatrix = Camera.main.worldToCameraMatrix;

        // Invert the matrices
        Matrix4x4 inverseProjectionMatrix = projectionMatrix.inverse;
        Matrix4x4 inverseViewMatrix = viewMatrix.inverse;

        // Combine the inverse matrices
        Matrix4x4 combinedMatrix = inverseProjectionMatrix * inverseViewMatrix;

        // Apply the combined matrix to the normalized screen position to get the intersection point in world space
        Vector4 normalizedScreenPosition = new Vector4((screenPosition.x / Screen.width) * 2 - 1, (screenPosition.y / Screen.height) * 2 - 1, 0, 1);
        Vector4 intersectionPointHomogeneous = combinedMatrix * normalizedScreenPosition;
        Vector3 intersectionPoint = new Vector3(intersectionPointHomogeneous.x, intersectionPointHomogeneous.y, intersectionPointHomogeneous.z) / intersectionPointHomogeneous.w;

        return intersectionPoint;
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            // Ensure mousePosition1 is the top-left corner and mousePosition2 is the bottom-right corner
            Vector2 mousePos1 = Camera.main.WorldToScreenPoint(mousePosition1World);
            Vector2 topLeft = new Vector2(Mathf.Floor(Mathf.Min(mousePos1.x, mousePosition2.x)), Mathf.Floor(Mathf.Min(mousePos1.y, mousePosition2.y)));
            Vector2 size = new Vector2(Mathf.Ceil(Mathf.Abs(mousePosition2.x - mousePos1.x)), Mathf.Ceil(Mathf.Abs(mousePosition2.y - mousePos1.y)));

            // Draw the selection box in screen space
            Rect selectionRect = new Rect(topLeft.x, Screen.height - topLeft.y - size.y, size.x, size.y);

            if (selectionRect.width > 5 && selectionRect.height > 5)
            {
                GUIStyle currentStyle = new GUIStyle(GUI.skin.box);
                currentStyle.normal.background = guiTexture;

                GUI.backgroundColor = Color.green;

                GUI.Box(selectionRect, "", currentStyle);
            }
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public void DrawUnitOutline(TestUnit unit, LineRenderer lineRenderer) {

        Vector3 unitPosition = unit.transform.position;
        unitPosition += new Vector3(0, -unitPosition.y, 0) + new Vector3(0, 0.5f, 0);

        int pointCount = 12;

        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < pointCount; i++) {
            float angleDeg = (360f / (float) pointCount) * (float) i;
            float angleRad = Mathf.PI / 180 * angleDeg;
            points.Add( unitPosition  + (new Vector3(Mathf.Cos(-angleRad), 0f, Mathf.Sin(-angleRad)) * unit.size)) ;
        }

        points.Add(points[0]);

        lineRenderer.enabled = true;
        lineRenderer.positionCount = pointCount + 1;
        lineRenderer.SetPositions(points.ToArray());

    }

    public void DrawOutline(List<Vector2Int> positions)
    {

        List<Vector3> outlinePositions = Hexagons.GetOuterVertices(positions);

        outlineRenderer.positionCount = outlinePositions.Count;
        outlineRenderer.SetPositions(outlinePositions.ToArray());
        outlineRenderer.enabled = true;

    }

    public void DrawUnitOutlines() {

        while (outlineRenderers.Count < selectedUnits.Count) {
            Debug.Log("ADDING LINE RENDERER");
            GameObject newLineRenderer = new GameObject("OutlineRenderer");
            outlineRenderers.Add(newLineRenderer.AddComponent<LineRenderer>());
            newLineRenderer.transform.parent = transform;
        }

        for (int i = 0; i < selectedUnits.Count; i++) {
            DrawUnitOutline(selectedUnits[i], outlineRenderers[i]);
        }

        if (outlineRenderers.Count > selectedUnits.Count) {
            for (int i = selectedUnits.Count; i < outlineRenderers.Count; i++) {
                outlineRenderers[i].enabled = false;
            }
        }

        

    }
}
