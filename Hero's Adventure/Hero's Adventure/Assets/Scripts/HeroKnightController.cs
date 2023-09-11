using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Tự động thêm rigidbody nếu chưa tồn tại
//[RequireComponent(typeof(Rigidbody))]
public class HeroKnightController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private LayerMask jumpAbleGround;
    [SerializeField] private float delayTimeJump = 1.5f, delayTimeAttack = 1f;
    private Rigidbody2D heroKnightRb;
    private Animator heroKnightamin;
    private BoxCollider2D heroKnightColl;
    private float dirX, dirY, timeSinceAttack;
    private bool facingRight = true, isDeath = false, isNotDelay = true;
    private int currentAttack = 0, currentHealth;
    private enum movementState { ide, run, fall };
    // Start is called before the first frame update
    void Start()
    {
        heroKnightRb = GetComponent<Rigidbody2D>();
        heroKnightamin = GetComponent<Animator>();
        heroKnightColl = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        healthBar.SetMaxHeath(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxis("Horizontal");
        dirY = Input.GetAxis("Vertical");
        if (!isDeath)
        {
            heroKnightRb.velocity = new Vector2(dirX * speed, heroKnightRb.velocity.y);
            if (dirY > .1f && IsGrounded() && isNotDelay)
            {
                heroKnightRb.velocity = new Vector2(heroKnightRb.velocity.x, jumpForce);
                StartCoroutine(countTimeDelay(delayTimeJump));
            }
            UpdateAnimation();
        }
        
    }

    void UpdateAnimation()
    {
        timeSinceAttack += Time.deltaTime;
        if (dirX > 0f)
        {
            if (!facingRight)
            {
                Flip();
            }
        }
        else if (dirX < 0f)
        {
            if (facingRight)
            {
                Flip();
            }
        }
        if (dirX != 0 && IsGrounded())
        {
            heroKnightamin.SetInteger("state", (int)movementState.run);
        }
        else if (heroKnightRb.velocity.y > 0f)
        {
            heroKnightamin.SetTrigger("isJump");
        }
        else if (heroKnightRb.velocity.y < -.1f && !IsGrounded())
        {
            heroKnightamin.SetInteger("state", (int)movementState.fall);
        }
        else if (Input.GetMouseButtonDown(0) && timeSinceAttack > 0.25f && isNotDelay)
        {
            currentAttack++;
            if(currentAttack > 2)
            {
                currentAttack = 1;
            }
            if(timeSinceAttack > 1.0f)
            {
                currentAttack = 1;
            }
            if(isNotDelay) heroKnightamin.SetTrigger("isAttack" + currentAttack);
            if(currentAttack == 2) StartCoroutine(countTimeDelay(delayTimeAttack));
            timeSinceAttack = 0.0f;
        }
        else if(IsGrounded()) 
        {
            heroKnightamin.SetInteger("state", (int)movementState.ide);
        }
    }
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
        healthBar.gameObject.transform.Rotate(0, 180, 0);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(heroKnightColl.bounds.center, heroKnightColl.bounds.size, 0f, Vector2.down, .1f, jumpAbleGround);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            TakeHit(20);
            heroKnightamin.SetTrigger("isTakeHit");
        }
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            isDeath = true;
            heroKnightamin.SetTrigger("isDeath");
        }
    }
    public void revival()
    {
        isDeath = false;
        currentHealth = maxHealth;
        healthBar.SetHealth(maxHealth);
        heroKnightamin.SetInteger("state", (int)movementState.ide);
    }

    IEnumerator countTimeDelay(float time)
    {
        isNotDelay = false;
        yield return new WaitForSeconds(time); 
        isNotDelay = true;
    }

    private void TakeHit(int dame)
    {
        currentHealth -= dame;
        healthBar.SetHealth(currentHealth);
    }

    // %#a = ctrl + shift + a
    // Add a new menu item with hotkey A
    //[MenuItem("Tools/Revival _a")]
    //[MenuItem("Tools/Revival %#a")]
    //public static void ShowState()
    //{
    //    Debug.Log("New item");
    //}
}
