using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private int index;
    [SerializeField] private bool isContinueSelection = false;
    [SerializeField] private GameObject selection;
    [SerializeField] private GameObject continueSelection;
    private SelectionControll selectionControl;
    private ContinueSelectionControl continueSelectionControl;
    // Start is called before the first frame update
    void Start()
    {
        selectionControl = selection.GetComponent<SelectionControll>();
        continueSelectionControl = continueSelection.GetComponent<ContinueSelectionControl>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isContinueSelection)
        {
            selectionControl.SetCurrentSelection(index);
        }
        else
        {
            continueSelectionControl.SetCurrentSelection(index);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isContinueSelection)
        {
            selectionControl.AccessSelection();
        }
        else
        {
            continueSelectionControl.AccessSelection();
        }
    }
}
