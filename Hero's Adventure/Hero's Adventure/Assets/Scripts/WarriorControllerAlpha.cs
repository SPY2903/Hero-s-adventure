using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorControllerAlpha : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float dashForce = 4f;
    [SerializeField] private float anchorJumpForce = 10f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float slideForce = 4f;
    [SerializeField] private float slidingTime = 0.2f;
    [SerializeField] private LayerMask jumpAbleGround, pointAbleAnchor;
    [SerializeField] private GameObject anchorArm;
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D coll, anchorArmColl;
    private int currentHealth;
    private float dirX, dirY, gravityScale, originalBoxcolliderSizeY, crouchBoxcolliderSizeY = 1.14f,
    originalBoxColliderOffsetY, crouchBoxColliderOffsetY = -0.26f, slideBoxColliderSizeY = 0.9f,
    slideBoxColliderOffsetY = -0.37f;
    private bool isAlive = true, facingRight = true, canDash = true, isDashing = false,
    isCrouching = false, isWall = false;
    private enum movementState { ide, run, fall, crouch, climbingLadder, stopClimbLadder };
    public bool ClimbingAllowed { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        anchorArmColl = anchorArm.GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        gravityScale = rb.gravityScale;
        originalBoxcolliderSizeY = coll.size.y;
        originalBoxColliderOffsetY = coll.offset.y;
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
            if (!isCrouching)
            {
                rb.velocity = new Vector2(dirX * speed, rb.velocity.y);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                anim.SetTrigger("isAttack");
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
                StartCoroutine(Dash(dashingTime));
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded() && canDash)
            {
                anim.SetTrigger("isSlide");
                coll.size = new Vector2(coll.size.x, slideBoxColliderSizeY);
                coll.offset = new Vector2(coll.offset.x, slideBoxColliderOffsetY);
                if (!facingRight)
                {
                    rb.velocity = new Vector2(-slideForce * transform.localScale.x, transform.position.y);
                }
                else
                {
                    rb.velocity = new Vector2(slideForce * transform.localScale.x, transform.position.y);
                }
                StartCoroutine(Dash(slidingTime));
            }
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !ClimbingAllowed)
            {
                anim.ResetTrigger("isWallSlide");
                anim.ResetTrigger("isEdgeGrab");
                anim.SetTrigger("isJump");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            if (Input.GetKeyDown(KeyCode.Space) && !ClimbingAllowed && IsAnchor() && !IsGrounded())
            {
                anim.SetTrigger("isJump");
                rb.velocity = new Vector2(rb.velocity.x, anchorJumpForce);
            }
            if (Input.GetKeyDown(KeyCode.Space) && isWall && !IsAnchor() && !IsGrounded())
            {
                anim.SetTrigger("isEdgeGrab");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            }
            if (!isCrouching && IsGrounded() && dirX != 0)
            {
                anim.ResetTrigger("isCrouch");
                anim.SetInteger("state", (int)movementState.run);
            }
            else if (dirY < 0 && IsGrounded())
            {
                isCrouching = true;
                coll.size = new Vector2(coll.size.x, crouchBoxcolliderSizeY);
                coll.offset = new Vector2(coll.offset.x, crouchBoxColliderOffsetY);
                anim.SetTrigger("isCrouch");
            }
            else if (rb.velocity.y < 0f && !IsGrounded() && !ClimbingAllowed)
            {
                anim.ResetTrigger("isEdgeGrab");
                anim.SetInteger("state", (int)movementState.fall);
            }
            else if (ClimbingAllowed && dirY != 0)
            {
                anim.ResetTrigger("isCrouch");
                anim.SetInteger("state", (int)movementState.climbingLadder);
            }
            else if (ClimbingAllowed && dirY == 0 && !IsGrounded())
            {
                anim.SetInteger("state", (int)movementState.stopClimbLadder);
            }
            else if (isWall && !IsGrounded() && !IsAnchor())
            {
                //anim.SetTrigger("isWallSlide");
                anim.SetInteger("state", 20);
            }
            else if (IsAnchor() && !Input.GetKeyDown(KeyCode.Space) && !IsGrounded())
            {
                anim.SetTrigger("isEdgeIde");
            }
            else
            {
                //anim.ResetTrigger("isWallSlide");
                anim.ResetTrigger("isCrouch");
                isCrouching = false;
                if (!isDashing)
                {
                    coll.size = new Vector2(coll.size.x, originalBoxcolliderSizeY);
                    coll.offset = new Vector2(coll.offset.x, originalBoxColliderOffsetY);
                }
                anim.SetInteger("state", (int)movementState.ide);
            }
            Debug.Log("is wall : " + isWall + "; isGrounded : " + IsGrounded() + "; isAnchor : " + IsAnchor());
        }
    }

    private void FixedUpdate()
    {
        if (ClimbingAllowed)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, dirY * speed);
        }
        if (!ClimbingAllowed)
        {
            rb.gravityScale = gravityScale;
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

    bool IsAnchor()
    {
        return Physics2D.BoxCast(anchorArmColl.bounds.center, anchorArmColl.bounds.size, 0f, Vector2.down, .1f, pointAbleAnchor);
    }

    IEnumerator Dash(float time)
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(time);
        rb.gravityScale = originalGravity;
        isDashing = false;
        canDash = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            TakeDame(20);
            if (currentHealth == 0)
            {
                isAlive = false;
                anim.SetTrigger("isDeath");
            }
            else
            {
                anim.SetTrigger("isHurt");
            }
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWall = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWall = false;
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

