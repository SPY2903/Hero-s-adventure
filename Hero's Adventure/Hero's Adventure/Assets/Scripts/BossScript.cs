using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
public class BossScript : MonoBehaviour
{
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distanceAttack = 2f;
    [SerializeField] private float delayFollowTime = 5f;
    [SerializeField] private float delayAttackTime = 1f;
    [SerializeField] private float timeDelayAttack = 1f;
    [SerializeField] private float distanceAppear = 2f;
    [SerializeField] private float fadeTime = 2f;
    [SerializeField] private GameObject target;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDame = 20;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private HealthBarComplete healthBar;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerController playerController;
    private bool facingRight = true, isAttacking = false, isFading = false, canMove = false;
    private float countTimeDelayFollow = 0;
    private int currentHealth = 0, countSkill = 0;
    private enum state { ide, run, death};
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerController = GameObject.Find("Warrior").GetComponent<PlayerController>();
        currentHealth = maxHealth;
        healthBar.SetMaxHeath(currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth != 0 && canMove)
        {
            countTimeDelayFollow += Time.deltaTime;
            if (!isAttacking)
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
            }
            if(countTimeDelayFollow >= delayFollowTime && playerController.GetCurrentHealth() != 0)
            {
                if (Vector2.Distance(transform.position, target.transform.position) > distanceAttack && !isAttacking)
                {
                    anim.SetInteger("state", (int)state.run);
                    Vector2 newPos = Vector2.MoveTowards(transform.position, new Vector2(target.transform.position.x, transform.position.y), speed * Time.fixedDeltaTime);
                    rb.MovePosition(newPos);
                }
                else if (Vector2.Distance(transform.position, target.transform.position) <= distanceAttack)
                {
                    if(currentHealth > maxHealth / 2)
                    {
                        StartCoroutine(Attack());
                    }
                    else
                    {
                        attackDame = 40;
                        countSkill++;
                        if(countSkill == 1)
                        {
                            StartCoroutine(Fade());
                        }
                        if(countSkill == 2)
                        {
                            StartCoroutine(Attack());
                            countSkill = 0;
                        }
                    }
                    countTimeDelayFollow = 0;
                }
            }
            else
            {
                anim.ResetTrigger("hurt");
                anim.SetInteger("state", (int)state.ide);
            }
        }
        if(!canMove)
        {
            anim.SetInteger("state", (int)state.ide);
        }
    }
    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }
    IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(delayAttackTime);
        anim.ResetTrigger("attack");
        isAttacking = false;

    }
    public void Inflict()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D enemy in hitPlayer)
        {
            if (enemy.CompareTag("Player"))
            {
                playerController.TakeDame(attackDame);
            }
        }
    }
    public void TakeDame(int dame)
    {
        if (!isFading)
        {
            currentHealth -= dame;
            healthBar.SetHealth(currentHealth);
            if (currentHealth != 0)
            {
                anim.SetTrigger("hurt");
            }
            else
            {
                anim.SetTrigger("death");
            }
        }
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
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    public void setCanMove(bool canMove)
    {
        this.canMove = canMove; 
    }
    public int GetCurrentHealth() {
        return currentHealth;
    }
}
