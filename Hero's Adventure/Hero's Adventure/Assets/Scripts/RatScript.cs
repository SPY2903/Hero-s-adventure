using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatScript : MonoBehaviour
{
    [SerializeField] private GameObject[] wayPoints;
    [SerializeField] private GameObject target;
    [SerializeField] private int maxHealth = 60;
    [SerializeField] private float ideTime = 2f;
    [SerializeField] private float abilityTime = 0.5f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float followSpeed = 4f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float timeDelayAttack = 1f;
    [SerializeField] private float timeToDestroy = 1f;
    [SerializeField] private HealthBarComplete healthBar;
    [SerializeField] private GameObject healBarGameObject;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;
    private int currentHealth, currentWayPointIndex = 0, count = 0;
    private float currentSpeed, currentTime = 0;
    private bool facingRight = true, isIde = false, canIde = true, isAttack = false;
    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        playerController = GameObject.Find("Warrior").GetComponent<PlayerController>();
        currentTime = speed;
        currentHealth = maxHealth;
        healthBar.SetMaxHeath(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth > 0)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= ideTime)
            {
                isIde = true;
                anim.SetBool("Run", false);
                if (canIde)
                {
                    if (currentTime >= ideTime + 1)
                    {
                        StartCoroutine(Ability());
                    }
                }
                else
                {
                    StartCoroutine(Ability());
                }
            }
            if (Vector2.Distance(rb.position, target.transform.position) <= attackRange)
            {
                isAttack = true;
                count++;
                if (count == 1)
                {
                    anim.SetBool("Run", false);
                    anim.SetTrigger("Attack");
                    if (playerController.GetIsLive())
                    {
                        playerController.TakeDame(20);
                    }
                }
                else
                {
                    anim.SetBool("Run", true);
                }
            }
            else
            {
                //anim.SetBool("Run", true);
                count = 0;
                isAttack = false;
                anim.ResetTrigger("Attack");
            }
            if (!isIde)
            {
                if (target.transform.position.x > wayPoints[0].transform.position.x &&
                    target.transform.position.x < wayPoints[1].transform.position.x)
                {
                    currentSpeed = followSpeed;
                    canIde = false;
                }
                else
                {
                    currentSpeed = speed;
                    canIde = true;
                }
                if (!isAttack)
                {
                    anim.SetBool("Run", true);
                }
                if (Vector2.Distance(transform.position, wayPoints[currentWayPointIndex].transform.position) < 0.1f)
                {
                    currentWayPointIndex++;
                    Flip();
                    if (currentWayPointIndex >= wayPoints.Length)
                    {
                        currentWayPointIndex = 0;
                    }
                }
                Vector2 newPos = Vector2.MoveTowards(transform.position, wayPoints[currentWayPointIndex].transform.position, currentSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
            }
        }
        else
        {
            StartCoroutine(DisplayHealBar());
            anim.SetBool("Run", false);
            anim.SetTrigger("Death");
            Destroy(gameObject, timeToDestroy);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, -180, 0);
        healBarGameObject.transform.Rotate(0, 180, 0);
    }

    IEnumerator Ability()
    {
        anim.SetBool("Ability", true);
        yield return new WaitForSeconds(abilityTime);
        anim.SetBool("Ability", false);
        currentTime = 0;
        isIde = false;
    }
    IEnumerator DelayAttack()
    {
        isIde = true;
        yield return new WaitForSeconds(timeDelayAttack);
        isIde = false;
    }

    public void TakeDame(int dame)
    {
        if (currentHealth == maxHealth)
        {
            healBarGameObject.SetActive(true);
        }
        currentHealth -= dame;
        anim.SetBool("Run", false);
        anim.SetTrigger("Hit");
        StartCoroutine(DelayAttack());
        healthBar.SetHealth(currentHealth);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        anim.SetBool("Run", false);
    //        anim.SetTrigger("Attack");
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        anim.ResetTrigger("Attack");
    //    }
    //}
    IEnumerator DisplayHealBar()
    {
        yield return new WaitForSeconds(0.1f);
        healBarGameObject.SetActive(false);
    }
}
