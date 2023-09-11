using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemPhase1Script : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distanceAppear = 2f;
    [SerializeField] private float dashForce = 2f;
    [SerializeField] private float transformTime = 8f;
    [SerializeField] private float fadeTime = 4f;
    [SerializeField] private float timeDelayAttack = 1f;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] private float takeDameTime = 2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDame = 20;
    [SerializeField] private Transform attackPoint;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private PlayerController playerController;
    private int currentHealth = 0, count = 0;
    private float currentSpeed, currentTime = 0;
    private bool facingRight = true, isFading = false, isDashing = false, canTakeDame = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        playerController = GameObject.Find("Warrior").GetComponent<PlayerController>();
        currentHealth = maxHealth;
        currentSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth != 0)
        {
            if (transform.position.x > target.transform.position.x)
            {
                if (!facingRight)
                {
                    Flip();
                }
            }
            else
            {
                if (facingRight)
                {
                    Flip();
                }
            }
            currentTime += Time.deltaTime;
            if (currentTime >= transformTime && !isDashing)
            {
                currentTime = 0;
                StartCoroutine(Fade());
                //if (transform.position.x > target.transform.position.x)
                //{

                //}
            }
            if (isDashing)
            {
                if (isDashing)
                {
                    if (playerController.GetFacingRight())
                    {
                        Vector2 newPos = Vector2.MoveTowards(transform.position, new Vector2(target.transform.position.x + dashForce, transform.position.y), speed * Time.fixedDeltaTime);
                        rb.MovePosition(newPos);
                    }
                    else
                    {
                        Vector2 newPos = Vector2.MoveTowards(transform.position, new Vector2(target.transform.position.x - dashForce, transform.position.y), speed * Time.fixedDeltaTime);
                        rb.MovePosition(newPos);
                    }
                }
            }
        }
        else
        {
            count++;
            if(count == 1)
            {
                anim.SetTrigger("Death");
            }
        }
    }
    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, -180, 0);
    }
    IEnumerator Fade()
    {
        isFading = true;
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(fadeTime);
        StartCoroutine(FadeIn());
        isFading = false;
        yield return new WaitForSeconds(timeDelayAttack);
        StartCoroutine(Attack());
    }

    IEnumerator FadeOut()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            Color c = sprite.material.color;
            c.a = f;
            sprite.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }
    IEnumerator FadeIn()
    {
        if (playerController.GetFacingRight())
        {
            Flip();
            rb.position = new Vector2(target.transform.position.x - distanceAppear, transform.position.y);
        }
        else
        {
            Flip();
            rb.position = new Vector2(target.transform.position.x + distanceAppear, transform.position.y);
        }
        for (float f = 0.05f; f <= 1f; f += 0.05f)
        {
            Color c = sprite.material.color;
            c.a = f;
            sprite.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }
    IEnumerator Attack()
    {
        anim.SetTrigger("Attack 3");
        StartCoroutine(Dash());
        yield return new WaitForSeconds(timeDelayAttack);
        anim.ResetTrigger("Attack 3");
    }
    IEnumerator Dash()
    {
        isDashing = true;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<PlayerController>().TakeDame(attackDame);
        }
        StartCoroutine(CanTakeDame());
    }
    IEnumerator CanTakeDame()
    {
        canTakeDame = true;
        yield return new WaitForSeconds(takeDameTime);
        canTakeDame = false;
        anim.ResetTrigger("Hit");
        anim.ResetTrigger("Attack 2");
    }

    public void TakeDame(int dame)
    {
        if (canTakeDame)
        {
            currentHealth -= dame;
            anim.SetTrigger("Hit");
            Debug.Log(currentHealth);
        }
        else
        {
            anim.SetTrigger("Attack 2");
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
