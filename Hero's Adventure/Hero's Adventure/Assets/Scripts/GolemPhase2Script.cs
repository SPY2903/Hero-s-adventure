using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GolemPhase2Script : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float ideTime = 2f;
    [SerializeField] private float distanceDisappear = 1f;
    [SerializeField] private float distanceAppearBack = 1.5f;
    [SerializeField] private float distanceAppearFront = 6f;
    [SerializeField] private float fadeTime = 4;
    [SerializeField] private float delayTransformTime = 2f;
    [SerializeField] private float attackTime = 1f;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private bool facingRight = true, isFading = false, canTramsform = true, isAttack = false, isWaiting = false, canTakeDame = false;
    private float currentTime = 0;
    private int currentHealth = 0, count = 0;
    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerController = GameObject.Find("Warrior").GetComponent<PlayerController>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentHealth > 0)
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
            if (!isFading && canTramsform)
            {
                if (Vector2.Distance(transform.position, target.transform.position) <= distanceDisappear)
                {
                    anim.SetBool("Run", false);
                    StartCoroutine(Fade());
                }
                else
                {
                    if (!isWaiting)
                    {
                        anim.SetBool("Run", true);
                        Vector2 targetPos = new Vector2(target.transform.position.x, transform.position.y);
                        Vector2 newPos = Vector2.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
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
        if (isAttack)
        {
            AppearFront();
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FadeIn());
            StartCoroutine(Waiting());
        }
        else
        {
            AppearBack();
            StartCoroutine(FadeIn());
            StartCoroutine(Attack());
        }
        isFading = false;
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
        isAttack = true;
        canTramsform = false;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackTime);
        anim.ResetTrigger("Attack");
        StartCoroutine(CanTramsform());

    }
    IEnumerator CanTramsform()
    {
        canTakeDame = true;
        yield return new WaitForSeconds(delayTransformTime);
        canTramsform = true;
        canTakeDame = false;
        anim.ResetTrigger("Hit");
    }
    IEnumerator Waiting()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1f);
        isWaiting = false;
    }

    void AppearBack()
    {
        if (playerController.GetFacingRight())
        {
            rb.position = new Vector2(target.transform.position.x - distanceAppearBack, transform.position.y);
        }
        else
        {
            rb.position = new Vector2(target.transform.position.x + distanceAppearBack, transform.position.y);
        }
    }
    void AppearFront()
    {
        if (playerController.GetFacingRight())
        {
            rb.position = new Vector2(target.transform.position.x + distanceAppearFront, transform.position.y);
        }
        else
        {
            rb.position = new Vector2(target.transform.position.x - distanceAppearFront, transform.position.y);
        }
        isAttack = false;
    }

    public void TakeDame(int dame)
    {
        if (canTakeDame)
        {
            currentHealth -= dame;
            anim.SetTrigger("Hit");
        }
    }
}
