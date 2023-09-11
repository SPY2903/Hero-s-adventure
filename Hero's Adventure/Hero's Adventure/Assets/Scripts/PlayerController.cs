using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float ladderJumpForce = 10f;
    [SerializeField] private float anchorJumpForce = 10f;
    [SerializeField] private float dashForce = 4f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float slideForce = 4f;
    [SerializeField] private float slidingTime = 0.2f;
    [SerializeField] private float timeDelayAttack = .25f;
    [SerializeField] private float timeDelayAttackEnemy = .5f;
    [SerializeField] private LayerMask jumpAbleGround, pointAbleAnchor, enemyLayers;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private GameObject anchorArm;
    [SerializeField] private AudioSource runningSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource crouchSound;
    [SerializeField] private AudioSource dashSound;
    [SerializeField] private AudioSource hurtSound;
    [SerializeField] private HealthBarComplete healthBar;
    [SerializeField] private GameObject losePanel;
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D coll, anchorArmColl;
    private Material material;
    private int currentHealth;
    private float dirX, dirY, gravityScale, originalBoxcolliderSizeY, crouchBoxcolliderSizeY = 1.14f,
    originalBoxColliderOffsetY, crouchBoxColliderOffsetY = -0.26f, slideBoxColliderSizeY = 0.9f,
    slideBoxColliderOffsetY = -0.37f, timeSinceAttack = 0f, fade = 1f, deathBoxcolliderSizeY = 0.4f, deathBoxColliderOffsetY = -0.62f,
        countInSpikeTime = 0;
    private bool isAlive = true, isRunning = false, facingRight = true, canDash = true, isDashing = false,
    isCrouching = false, isWall = false, isLadderPointJump = false, canAttack = true, canMove = true, isInSpike = false, canTakeDame = true;
    private enum movementState { ide, run, fall, crouch, climbingLadder, stopClimbLadder };
    public bool ClimbingAllowed { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        material = GetComponent<SpriteRenderer>().material;
        anchorArmColl = anchorArm.GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        gravityScale = rb.gravityScale;
        originalBoxcolliderSizeY = coll.size.y;
        originalBoxColliderOffsetY = coll.offset.y;
        healthBar.SetMaxHeath(currentHealth);
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive && canMove)
        {
            if (isDashing)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    anim.SetTrigger("isDashAttack");
                    Attack();
                    attackSound.loop = false;
                    attackSound.Play();
                    //StartCoroutine(DelayAttack());
                    isDashing = false;
                }
                return;
            }
            timeSinceAttack += Time.deltaTime;
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
            if (!isCrouching && canAttack)
            {
                rb.velocity = new Vector2(dirX * speed, rb.velocity.y);
                isRunning = true;
            }
            if (Input.GetKeyDown(KeyCode.C) && canAttack)
            {
                anim.SetTrigger("isAttack");
                Attack();
                StartCoroutine(DelayAttack());
            }
            if (Input.GetKeyDown(KeyCode.Z) && canDash && !isCrouching)
            {
                anim.SetTrigger("isDash");
                dashSound.Play();
                if (!facingRight)
                {
                    rb.velocity = new Vector2(-dashForce * transform.localScale.x, 0);
                }
                else
                {
                    rb.velocity = new Vector2(dashForce * transform.localScale.x, 0);
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
            if (Input.GetKeyDown(KeyCode.X) && IsGrounded() && !ClimbingAllowed)
            {
                anim.ResetTrigger("isWallSlide");
                anim.ResetTrigger("isEdgeGrab");
                anim.SetTrigger("isJump");
                jumpSound.Play();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            if(Input.GetKeyDown(KeyCode.X) && !ClimbingAllowed && IsAnchor() && !IsGrounded())
            {
                anim.SetTrigger("isJump");
                rb.velocity = new Vector2(rb.velocity.x, anchorJumpForce);
            }
            if(Input.GetKeyDown(KeyCode.X) && isWall && !IsAnchor() && !IsGrounded())
            {
                anim.SetTrigger("isEdgeGrab");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            }
            if (!isCrouching && IsGrounded() && dirX != 0)
            {
                anim.ResetTrigger("isCrouch");
                if (!runningSound.isPlaying)
                {
                    runningSound.Play();
                }
                anim.SetInteger("state", (int)movementState.run);
            }
            else if (dirY < 0 && IsGrounded())
            {
                coll.size = new Vector2(coll.size.x, crouchBoxcolliderSizeY);
                coll.offset = new Vector2(coll.offset.x, crouchBoxColliderOffsetY);
                if (!isCrouching)
                {
                    crouchSound.Play();
                    isCrouching = true;
                }
                anim.SetTrigger("isCrouch");
            }
            else if (rb.velocity.y < 0f && !IsGrounded() && !ClimbingAllowed)
            {
                anim.ResetTrigger("isEdgeGrab");
                anim.SetInteger("state", (int)movementState.fall);
            }
            else if (ClimbingAllowed && dirY != 0)
            {
                isRunning = false;
                anim.ResetTrigger("isCrouch");
                anim.SetInteger("state", (int)movementState.climbingLadder);
            }
            else if (ClimbingAllowed && dirY == 0 && !IsGrounded())
            {
                isRunning = false;
                anim.SetInteger("state", (int)movementState.stopClimbLadder);
            }
            else if(isWall && !IsGrounded() && !IsAnchor())
            {
                //anim.SetTrigger("isWallSlide");
                anim.SetInteger("state", 20);
            }
            else if (IsAnchor() && !Input.GetKeyDown(KeyCode.X) && !IsGrounded())
            {
                anim.SetTrigger("isEdgeIde");
            }
            else
            {
                //anim.ResetTrigger("isWallSlide");
                anim.ResetTrigger("isCrouch");
                isRunning = false;
                isCrouching = false;
                if (!isDashing && !isCrouching && currentHealth !=0)
                {
                    coll.size = new Vector2(coll.size.x, originalBoxcolliderSizeY);
                    coll.offset = new Vector2(coll.offset.x, originalBoxColliderOffsetY);
                }
                if(currentHealth > 0)
                {
                    runningSound.Stop();
                    anim.SetInteger("state", (int)movementState.ide);
                }
            }
            //Debug.Log("is wall : " + isWall + "; isGrounded : " + IsGrounded() + "; isAnchor : " + IsAnchor());
            if (isInSpike)
            {
                countInSpikeTime += Time.deltaTime;
                if (countInSpikeTime >= 2)
                {
                    TakeDame(10);
                    countInSpikeTime = 0;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (ClimbingAllowed && !isLadderPointJump && !isRunning)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, dirY * speed);
        }
        if(!ClimbingAllowed){
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

    IEnumerator DelayAttack()
    {
        canAttack = false;
        attackSound.loop = true;
        attackSound.Play();
        canTakeDame = false;
        yield return new WaitForSeconds(timeDelayAttack);
        attackSound.Stop();
        canAttack = true;
        canTakeDame = true;
    }

    IEnumerator DelayAttackEnemy()
    {
        yield return new WaitForSeconds(timeDelayAttackEnemy);
        Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemys)
        {
            if (enemy.name.Equals("Pebble"))
            {
                enemy.GetComponent<PebbleScript>().TakeDame(attackDamage);
            }
            if (enemy.name.Equals("Bat"))
            {
                enemy.GetComponent<BatScript>().TakeDame(attackDamage);
            }
            if (enemy.name.Equals("Rat"))
            {
                enemy.GetComponent<RatScript>().TakeDame(attackDamage);
            }
            if(enemy.name.Equals("Spiked Slime"))
            {
                enemy.GetComponent<SpikedSlimeScript>().TakeDame(attackDamage);
            }
            if(enemy.name.Equals("Golem Phase 1"))
            {
                enemy.GetComponent<GolemPhase1Script>().TakeDame(attackDamage);
            }
            if(enemy.name.Equals("Golem Phase 2"))
            {
                enemy.GetComponent<GolemPhase2Script>().TakeDame(attackDamage);
            }
            if (enemy.name.Equals("Skull"))
            {
                enemy.GetComponent<EnemyTutorial>().TakeDame(attackDamage);
            }
            if (enemy.name.Equals("Boss"))
            {
                enemy.GetComponent<BossScript>().TakeDame(attackDamage);
            }
        }
    }

    IEnumerator DelayTakeDame()
    {
        yield return new WaitForSeconds(2f);
        TakeDame(20);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            isInSpike = true;
            TakeDame(10);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWall = true;
        }
        if (collision.gameObject.CompareTag("LadderPointJump"))
        {
            isLadderPointJump = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            isInSpike = false;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWall = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LadderPointJump"))
        {
            isLadderPointJump = true;
            if (ClimbingAllowed && dirY > 0)
            {
                rb.gravityScale = gravityScale;
                rb.velocity = new Vector2(rb.velocity.x, ladderJumpForce);
            }
        }
        if (collision.CompareTag("Heart"))
        {
            if(currentHealth != maxHealth)
            {
                currentHealth += 20;
                healthBar.SetHealth(currentHealth);
                Destroy(collision.gameObject);
            }
        }
        if (collision.CompareTag("DeathZone"))
        {
            currentHealth = 0;
            healthBar.SetHealth(currentHealth);
        }
        if (collision.CompareTag("Spike"))
        {
            TakeDame(20);
        }
        //if (collision.CompareTag("Enemy"))
        //{
        //    if (isAlive)
        //    {
        //        TakeDame(20);
        //        //StartCoroutine(DelayTakeDame());
        //        if (currentHealth == 0)
        //        {
        //            isAlive = false;
        //            anim.SetTrigger("isDeath");
        //        }
        //    }
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("LadderPointJump"))
        {
            isLadderPointJump = false;
            if (!ClimbingAllowed)
            {
                anim.SetTrigger("isJump");
            }
        }
    }

    void Attack()
    {
        StartCoroutine(DelayAttackEnemy());
    }

    public bool GetFacingRight()
    {
        return facingRight;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDame(int dame)
    {
        if (canTakeDame)
        {
            hurtSound.Play();
            currentHealth -= dame;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > 0)
            {
                anim.SetTrigger("isHurt");
            }
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isAlive = false;
                coll.size = new Vector2(coll.size.x, deathBoxcolliderSizeY);
                coll.offset = new Vector2(coll.offset.x, deathBoxColliderOffsetY);
                anim.SetTrigger("isDeath");
                anim.SetInteger("state", 0);
                StartCoroutine(DisplayLosePanel());
            }
        }
    }
    public void IncreaseHP(int hp)
    {
        currentHealth += hp;
    }

    public void Revival()
    {
        isAlive = true;
        currentHealth = maxHealth;
        anim.SetInteger("state", (int)movementState.ide);
    }
    public bool GetIsLive()
    {
        return isAlive;
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public void Fade()
    {
        fade -= Time.deltaTime;
        if(fade <= 0)
        {
            fade = 0;
        }
        material.SetFloat("_Fade", fade);
    }
    public float GetFadeValue()
    {
        return fade;
    }
    IEnumerator DisplayLosePanel()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0;
        losePanel.SetActive(true);
    }
}
