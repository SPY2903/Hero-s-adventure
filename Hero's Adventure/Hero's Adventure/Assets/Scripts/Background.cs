using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private Vector3 startPos;
    private float repeatWidth;
    private BoxCollider2D coll;
    private RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
        coll = GetComponent<BoxCollider2D>();
        repeatWidth = coll.size.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        rect.Translate(Vector3.left * speed * Time.deltaTime);
        if(rect.anchoredPosition.x < startPos.x - repeatWidth)
        {
            rect.anchoredPosition = startPos;
        }
    }
}
