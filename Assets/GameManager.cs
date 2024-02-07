using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Reference to the currently active map
    public HexMap currentMap;

    public int excavatedTiles = 0;

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
        }

        Time.timeScale = 4f;
    }

}