using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDropScript : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Math.Round(target.transform.position.x,3) >= Math.Round(transform.position.x, 3))
        {
            rb.gravityScale = 6;
        }
    }
}
