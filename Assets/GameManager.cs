using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Reference to the currently active map (currently only a container object for buildings)
    public HexMap buildingMap;

    // current data for map positions
    public HexData tileMap;
    
    public AElevator elevator;

    public int currentLevelView = 0;

    public int excavatedTiles = 0;
    public float defaultTimeScale = 1f;
    public float hexSize = 10f;

    private void Awake()
    {
        // Ensure only one instance of the GameManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (!elevator) {
            elevator = FindObjectOfType<AElevator>();
        }

        if (!buildingMap) {
            buildingMap = FindObjectOfType<HexMap>();
        }

        if (tileMap == null) {
            tileMap = new HexData(10, 10, 10);
        }

        Hexagons.setHexSize(hexSize);
        Time.timeScale = defaultTimeScale;
    }

}