using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTutorial : MonoBehaviour
{
    [SerializeField] int maxHealth = 60;
    [SerializeField] private HealthBarComplete healthBar;
    [SerializeField] private GameObject healBarGameObject;
    [SerializeField] private float destroyTime = 1f;
    [SerializeField] private float gravityScale = 4f;
    [SerializeField] private LayerMask jumpAbleGround;
    private int currentHealth = 0;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        healthBar.SetMaxHeath(maxHealth);
    }

    // Update is called once per frame
    void FixUpdate()
    {
        if (IsGrounded())
        {
            coll.isTrigger = true;
            rb.gravityScale = 0;
        }
    }
    public void TakeDame(int dame)
    {
        if(currentHealth == maxHealth)
        {
            healBarGameObject.SetActive(true);
        }
        currentHealth -= dame;
        if(currentHealth != 0)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            anim.SetTrigger("Death");
            StartCoroutine(DisplayHealBar());
            coll.isTrigger = false;
            rb.gravityScale = gravityScale;
            Destroy(gameObject, destroyTime);
        }
        healthBar.SetHealth(currentHealth);
    }

    IEnumerator DisplayHealBar()
    {
        yield return new WaitForSeconds(0.1f);
        healBarGameObject.SetActive(false);
    }
    bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpAbleGround);
    }
}
