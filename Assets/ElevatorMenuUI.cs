using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorMenuUI : MonoBehaviour
{

    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void AddLevel() {
        AElevator elevator = FindObjectOfType<AElevator>();
        elevator.ExpandUnderground();
    }

    // Update is called once per frame
    void Update()
    {
        if (WorldSelectionController.selectedElevator != null) {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        } else {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}
