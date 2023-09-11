using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class ArrowScript : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float gravityScale = 4f;
    [SerializeField] private float destroyTime = 2f;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private bool canTransform = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canTransform)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            //rb.AddForce(Vector2.left * speed);
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        canTransform = false;
    //        rb.gravityScale = gravityScale;
    //        coll.isTrigger = true;
    //        Destroy(gameObject, destroyTime);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Ground"))
        {
            canTransform = false;
            rb.gravityScale = gravityScale;
            Destroy(gameObject, destroyTime);
        }
    }

}
