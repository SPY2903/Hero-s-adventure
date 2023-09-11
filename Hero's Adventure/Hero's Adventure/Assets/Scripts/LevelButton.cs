using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int index;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectedMaterial;
    private Button btn;
    private ManagerSelection manager;
    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        manager = GameObject.Find("Manager selection").GetComponent<ManagerSelection>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public int GetIndex()
    {
        return index;
    }
    public void Selected()
    {
        btn.image.material = selectedMaterial;
    }
    public void UnSelect()
    {
        btn.image.material = defaultMaterial;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.SetCurrentSelection(index);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //UnSelect();
    }
}
