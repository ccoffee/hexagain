using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;

	HexCell[] cells;

	public TMPro.TextMeshProUGUI cellLabelPrefab;

	Canvas gridCanvas;

	HexMesh hexMesh;

	public Transform cellContainer;

	public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;

	public bool previewGrid = false;
	public List<HexCoordinates> previewCoordinates = new List<HexCoordinates>();

	public bool showLabels;

	void Awake()
	{
		
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		if (previewGrid)
		{

			HexCoordinates centerCoord = new HexCoordinates(2, 2);
			previewCoordinates = new List<HexCoordinates>();
			previewCoordinates.Add(centerCoord);
			
			// centerCoord.GetNeighborsInRange(1);

			cells = new HexCell[previewCoordinates.Count];

			int i = 0;
			previewCoordinates.ForEach((previewCoordinate) =>
			{

				CreateCell(previewCoordinate.X, previewCoordinate.Z, i++);

			});
			
		}
		else
		{
			cells = new HexCell[height * width];

			for (int z = 0, i = 0; z < height; z++)
			{
				for (int x = 0; x < width; x++)
				{
					CreateCell(x, z, i++);
				}
			}
		}
	}

    private void Start()
    {
		hexMesh.Triangulate(cells, previewGrid);
    }

	public void DrawPreview() {
		hexMesh.Triangulate(cells);
    }

	public void DrawPreview(List<HexCoordinates> hexCoordinates) {
		// cells = CellsFromCoordinates(previewCoordinates);
		hexMesh.Triangulate(cells);
    }


    void CreateCell(int x, int z, int i)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);
		
		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(cellContainer, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.color = defaultColor;

		if (showLabels)
		{
			TMPro.TextMeshProUGUI label = Instantiate<TMPro.TextMeshProUGUI>(cellLabelPrefab);

			label.rectTransform.SetParent(gridCanvas.transform, false);
			label.rectTransform.anchoredPosition =
				new Vector2(position.x, position.z);
			label.text = cell.coordinates.ToStringOnSeparateLines();
		}
	}

	HexCell[] CellsFromCoordinates(List<HexCoordinates> coordinates)
	{

		List<HexCell> selectionCells = new List<HexCell>();

		coordinates.ForEach((coordinate) =>
		{
			Vector3 position;
			position.x = (coordinate.X + coordinate.X * 0.5f - coordinate.Z / 2) * (HexMetrics.innerRadius * 2f);
			position.y = 0f;
			position.z = coordinate.Z * (HexMetrics.outerRadius * 1.5f);

			HexCell cell = Instantiate<HexCell>(cellPrefab);
			cell.transform.SetParent(cellContainer, false);
			cell.transform.localPosition = position;
			cell.coordinates = coordinate;
			cell.color = defaultColor;


			selectionCells.Add(cell);
		});


		return selectionCells.ToArray();


	}

	public void ColorCell(Vector3 position, Color color)
	{
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		HexCell cell = cells[index];
		cell.color = color;
		hexMesh.Triangulate(cells);
	}

}
