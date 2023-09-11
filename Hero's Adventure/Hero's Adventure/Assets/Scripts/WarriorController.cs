using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float dashForce = 4f;
    [SerializeField] private float dashingTime = 0.2f, crouchTime = 0.2f;
    [SerializeField] private LayerMask jumpAbleGround;
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D coll;
    private int currentHealth;
    private float dirX, dirY;
    private bool isAlive = true, facingRight = true, canDash = true, isDashing = false, isCrouching = false;
    private enum movementState { ide, run, fall, crouch};
    public bool ClimbingAllowed { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            if (isDashing)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    anim.SetTrigger("isDashAttack");
                    isDashing = false;
                }
                return;
            }
            dirX = Input.GetAxis("Horizontal");
            dirY = Input.GetAxis("Vertical");
            if (dirX > 0)
            {
                if (!facingRight)
                {
                    Flip();
                }
            }
            else if (dirX < 0)
            {
                if (facingRight)
                {
                    Flip();
                }
            }
            if (dirX != 0 && IsGrounded())
            {
                anim.SetInteger("state", (int)movementState.run);
            }
            if (!isCrouching && dirX !=0)
            {
                rb.velocity = new Vector2(dirX * speed, transform.position.y);
            }
            if (IsGrounded() &&  Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger("isJump");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            if (rb.velocity.y < 0f && !IsGrounded() && !ClimbingAllowed)
            {
                anim.SetInteger("state", (int)movementState.fall);
            }
            if (Input.GetKeyDown(KeyCode.Z) && canDash && !isCrouching)
            {
                anim.SetTrigger("isDash");
                if (!facingRight)
                {
                    rb.velocity = new Vector2(-dashForce * transform.localScale.x, transform.position.y);
                }
                else
                {
                    rb.velocity = new Vector2(dashForce * transform.localScale.x, transform.position.y);
                }
                StartCoroutine(Dash());
            }
            if (IsGrounded() && dirY < 0f)
            {
                //anim.SetInteger("state", (int)movementState.croush);
                isCrouching = true;
                anim.SetInteger("state",10);
                anim.SetTrigger("isCrouch");
                StartCoroutine(Crouch());
            }
            if (ClimbingAllowed && !isCrouching && dirY != 0f)
            {
                rb.isKinematic = true;
                anim.SetTrigger("isClimbingLadder");
                rb.velocity = new Vector2(rb.velocity.x, dirY * speed);
            }else if(ClimbingAllowed && !isCrouching && !IsGrounded())
            {
                rb.isKinematic = true;
                anim.SetTrigger("isStopClimb");
                //rb.velocity = new Vector2(rb.velocity.x,transform.position.y);
            }
            else
            {
                rb.isKinematic = false;
            }
            if (Input.GetKeyDown(KeyCode.C) && IsGrounded())
            {
                anim.SetTrigger("isAttack");
            }

            if (IsGrounded() && dirY == 0 && dirX == 0)
            {
                anim.SetInteger("state", (int)movementState.ide);
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpAbleGround);
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        canDash = true;
    }

    IEnumerator Crouch()
    {
        yield return new WaitForSeconds(crouchTime);
        if (dirY == 0) isCrouching = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            TakeDame(20);
            if(currentHealth == 0)
            {
                isAlive = false;
                anim.SetTrigger("isDeath");
            }
            else
            {
                anim.SetTrigger("isHurt");
            }
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void TakeDame(int dame)
    {
        currentHealth -= dame;
    }

    public void Revival()
    {
        isAlive = true;
        currentHealth = maxHealth;
        anim.SetInteger("state", (int)movementState.ide);
    }
}
