using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGroupOutline : MonoBehaviour
{

    public static HexGroupOutline current;

    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Draw(List<HexCoordinates> coordinates) {

        

    }
}
