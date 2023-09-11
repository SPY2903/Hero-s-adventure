using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatScript : MonoBehaviour
{
	[SerializeField] private GameObject[] wayPoints;
	[SerializeField] private GameObject defaultPos;
	[SerializeField] private GameObject target;
    [SerializeField] private LayerMask jumpAbleGround;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float speedFollow = 4f;
	[SerializeField] private float distanceAttack = 8f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float distancePushBackX = 5f, distancePushBackY = 4f;
	[SerializeField] private float timeToDestroy = 1.5f;
    [SerializeField] private float timeDelay = 1f;
    [SerializeField] private int maxHealth = 60, timeRepeat = 0;
    [SerializeField] private HealthBarComplete healthBar;
    [SerializeField] private GameObject healBarGameObject;
    private Rigidbody2D rb;
	private BoxCollider2D coll;
	private Animator anim;
	private int currentHealth, currentWayPointIndex = 0;
    private bool facingRight = true, isTakeDame = false, canFollow = true,canAttack = true;
    private PlayerController playerController;
    private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<BoxCollider2D>();
		anim = GetComponent<Animator>();
        playerController = GameObject.Find("Warrior").GetComponent<PlayerController>();
        currentHealth = maxHealth;
        healthBar.SetMaxHeath(maxHealth);
    }
	void Update()
	{
		if (currentHealth != 0)
		{
            if (canFollow && ((Vector2.Distance(transform.position, target.transform.position) < distanceAttack) || isTakeDame) &&
                (Vector2.Distance(transform.position, target.transform.position) > attackRange))
            {
                //anim.SetBool("Run", true);
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
                //Vector2 lookDirection = (target.transform.position - transform.position).normalized;
                //rb.AddForce(lookDirection * speed);
                //Vector2 targetPos = new Vector2(target.transform.position.x, target.transform.position.y);
                //transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
                Vector2 newPos = Vector2.MoveTowards(transform.position, target.transform.position, speedFollow * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
            }else if (Vector2.Distance(transform.position, target.transform.position) <= attackRange)
            {
                if (canAttack)
                {
                    StartCoroutine(DelayAttack());
                }
            }
            else
            {
                if (!isTakeDame)
                {
                    if (transform.position.y != defaultPos.transform.position.y)
                    {
                        if (transform.position.x > defaultPos.transform.position.x)
                        {
                            currentWayPointIndex = 0;
                            if (!facingRight)
                            {
                                Flip();
                            }
                        }
                        else
                        {
                            currentWayPointIndex = 1;
                            if (facingRight)
                            {
                                Flip();
                            }
                        }
                        transform.position = Vector2.MoveTowards(transform.position, defaultPos.transform.position, speed * Time.deltaTime);
                    }
                    else
                    {
                        if (Vector2.Distance(wayPoints[currentWayPointIndex].transform.position, transform.position) < .1f)
                        {
                            currentWayPointIndex++;
                            if (transform.position.x > defaultPos.transform.position.x)
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
                            if (currentWayPointIndex >= wayPoints.Length)
                            {
                                //Flip();
                                currentWayPointIndex = 0;
                            }
                        }
                        transform.position = Vector2.MoveTowards(transform.position, wayPoints[currentWayPointIndex].transform.position, speed * Time.deltaTime);
                    }
                }
            }
        }
        else
		{
            StartCoroutine(DisplayHealBar());
            coll.isTrigger = false;
            if (IsGrounded() && timeRepeat == 1)
            {
                rb.gravityScale = 0;
                coll.isTrigger = true;
                anim.SetTrigger("isDeathLie");
                timeRepeat++;
            }
            else
            {
                //coll.size = new Vector2(coll.size.x, sizeBoxColliderY);
                //coll.offset = new Vector2(coll.offset.x, offsetBoxColliderY);
                rb.gravityScale = 1.5f;
                if(timeRepeat == 0)
                {
                    anim.SetTrigger("isDeathFall");
                    timeRepeat++;
                }
                Destroy(gameObject, timeToDestroy);
            }
        }
	}

    bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpAbleGround);
    }

    void Flip()
	{
		facingRight = !facingRight;
		transform.Rotate(0, -180, 0);
        healBarGameObject.transform.Rotate(0, 180, 0);
	}
	public void TakeDame(int dame)
    {
        if (currentHealth == maxHealth)
        {
            healBarGameObject.SetActive(true);
        }
        canFollow = false;
        StartCoroutine(Delay());
		isTakeDame = true;
		//anim.SetBool("Run", false);
		if (target.transform.position.x > transform.position.x)
		{
            rb.velocity = new Vector2(-distancePushBackX, distancePushBackY);
        }
		else
		{
            rb.velocity = new Vector2(distancePushBackX, distancePushBackY);
        }
        anim.ResetTrigger("Attack");
        anim.SetTrigger("Hit");
		currentHealth -= dame;
        healthBar.SetHealth(currentHealth);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(timeDelay);
        canFollow = true;
    }
    IEnumerator DisplayHealBar()
    {
        yield return new WaitForSeconds(0.1f);
        healBarGameObject.SetActive(false);
    }
    IEnumerator DelayAttack()
    {
        canAttack = false;
        anim.SetTrigger("Attack");
        if (playerController.GetIsLive())
        {
            playerController.TakeDame(20);
        }
        yield return new WaitForSeconds(1.5f);
        canAttack = true;
    }
}
