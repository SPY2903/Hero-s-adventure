using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PebbleScript : MonoBehaviour
{
	[SerializeField] private GameObject target;
	[SerializeField] private GameObject startPoint, endPoint, defaultPoint;
	[SerializeField] private float speedFollow = 4f;
	[SerializeField] private float ideTime = 2f;
	//[SerializeField] private float distanceAttack = 4f;
	//[SerializeField] private float distancePushBack = 5f;
	[SerializeField] private float timeToDestroy = 1.5f;
	[SerializeField] private float timeDelay = 1f;
	[SerializeField] private float timeReturnToDefaultPos = 2f;
	[SerializeField] private int maxHealth = 60;
	[SerializeField] private HealthBarComplete healthBar;
	[SerializeField] private GameObject healBarGameObject;
	private Rigidbody2D rb;
	private BoxCollider2D coll;
	private Animator anim;
	private float currentTime = 0f;
	private int currentHealth;
	private bool isIde = true, facingRight = true, isTakeDame = false, canFollow = true;
	Vector2 targetPos;
	private PlayerController playerController;

	private void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<BoxCollider2D>();
		anim = GetComponent<Animator>();
		playerController = GameObject.Find("Warrior").GetComponent<PlayerController>();
		currentHealth = maxHealth;
		targetPos = startPoint.transform.position;
		healthBar.SetMaxHeath(maxHealth);
	}
    void Update()
	{
		if(currentHealth != 0)
        {
			currentTime += Time.deltaTime;
			if (currentTime > ideTime && isIde && !isTakeDame)
			{
				currentTime = 0;
				Flip();
				anim.SetBool("Run", false);
			}
			if(target.transform.position.x > startPoint.transform.position.x &&
				target.transform.position.x < endPoint.transform.position.x && canFollow)
			{
				anim.SetBool("Run", true);
				if (Vector2.Distance(transform.position, endPoint.transform.position) < 0.1f)
                {
                    if (facingRight)
                    {
                        Flip();
                    }
                }
                else
                {
                    if (!facingRight)
                    {
                        Flip();
                    }
                }
                if (transform.position.x > target.transform.position.x)
                {
					if (Vector2.Distance(transform.position, endPoint.transform.position) < 0.1f)
					{
						targetPos = startPoint.transform.position;
						canFollow = false;
						StartCoroutine(DelayAttack());
					}
					//if (!facingRight)
     //               {
     //                   Flip();
     //               }
                }
                else
				{
					if (Vector2.Distance(transform.position, startPoint.transform.position) < 0.1f)
					{
						targetPos = endPoint.transform.position;
						canFollow = false;
						StartCoroutine(DelayAttack());
					}
					//targetPos = endPoint.transform.position;
					//if (facingRight)
     //               {
     //                   Flip();
     //               }
                }
                isIde = false;
				Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, speedFollow * Time.fixedDeltaTime);
				rb.MovePosition(newPos);
			}
            else
            {
				isIde = true;
				//if (isIde && Vector2.Distance(transform.position, defaultPoint.transform.position) < 0.1f)
				//{
				//	StartCoroutine(BackToDefaultPos());
				//}
			}
			//if (canFollow && ((Vector2.Distance(transform.position, target.transform.position) < distanceAttack) || isTakeDame))
			//{
			//	anim.SetBool("Run", true);
			//	if (transform.position.x > target.transform.position.x)
			//	{
			//		if (!facingRight)
			//		{
			//			Flip();
			//		}
			//	}
			//	else
			//	{
			//		if (facingRight)
			//		{
			//			Flip();
			//		}
			//	}
			//	isIde = false;
   //             //Vector3 targetPos = new Vector3(target.transform.position.x, 0,0);
   //             //transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
   //             //Vector2 lookDirection = (target.transform.position - transform.position).normalized;
   //             //rb.AddForce(lookDirection * speed);
   //             targetPos = new Vector2(target.transform.position.x, rb.position.y);
   //             Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, speedFollow * Time.fixedDeltaTime);
   //             rb.MovePosition(newPos);
   //         }
			//else
			//{
			//	isIde = true;
			//}
		}
		else
        {
			StartCoroutine(delay());
			StartCoroutine(DisplayHealBar());
			anim.SetBool("Run", false);
			anim.SetTrigger("Death");
			Destroy(gameObject, timeToDestroy);
		}
	}

	void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
		healBarGameObject.transform.Rotate(0, 180, 0);
	}
	//public void TakeDame(int dame)
	//   {
	//	isTakeDame = true;
	//	canFollow = false;
	//	StartCoroutine(DelayAttack());
	//	anim.SetBool("Run", false);
	//	if(target.transform.position.x > transform.position.x)
	//       {
	//		rb.velocity = new Vector2(-distancePushBack, 0);
	//       }
	//       else
	//       {
	//		rb.velocity = new Vector2(distancePushBack, 0);
	//	}
	//	anim.SetTrigger("Hit");
	//	currentHealth -= dame;
	//   }

	public void TakeDame(int dame)
	{
		isTakeDame = true;
		canFollow = false;
		StartCoroutine(DelayAttack());
		anim.SetBool("Run", false);
		if (currentHealth == maxHealth)
		{
			healBarGameObject.SetActive(true);
		}
		//if (target.transform.position.x > transform.position.x)
		//{
		//	rb.velocity = new Vector2(-distancePushBack, 0);
		//}
		//else
		//{
		//	rb.velocity = new Vector2(distancePushBack, 0);
		//}
		anim.SetTrigger("Hit");
		currentHealth -= dame;
		healthBar.SetHealth(currentHealth);
	}

	IEnumerator delay()
    {
		yield return new WaitForSeconds(0.5f);
		rb.bodyType = RigidbodyType2D.Static;
		coll.isTrigger = true;
	}

	IEnumerator DelayAttack()
	{
		yield return new WaitForSeconds(timeDelay);
		canFollow = true;
	}

	IEnumerator BackToDefaultPos()
    {
		yield return new WaitForSeconds(timeReturnToDefaultPos);
		if(transform.position.x > defaultPoint.transform.position.x)
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
		Vector2 newPos = Vector2.MoveTowards(rb.position, defaultPoint.transform.position, speedFollow * Time.fixedDeltaTime);
		rb.MovePosition(newPos);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.CompareTag("Player"))
		{
			playerController.TakeDame(20);
		}
	}
	IEnumerator DisplayHealBar()
	{
		yield return new WaitForSeconds(0.1f);
		healBarGameObject.SetActive(false);
	}
}
