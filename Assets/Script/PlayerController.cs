using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    // 3 vùng đánh combo
    public PlayerAttackZone attackZone1;
    public PlayerAttackZone attackZone2;
    public PlayerAttackZone attackZone3;

    // BẮN CUNG
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    private bool isShooting = false;

    // MANA
    public int maxMana = 10;
    private int currentMana;
    public ManaBar manaBar;
    public float manaRegenCooldown = 2f;   
    private float manaRegenTimer = 0f;

    private bool isGrounded;
    private bool canDoubleJump;
    private Animator animator;
    private Rigidbody2D rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentMana = maxMana;
        if (manaBar != null)
            manaBar.SetMana(currentMana, maxMana);
    }

    void Update()
    {
        // Không điều khiển nếu đang bị đánh hoặc đã chết
        if (!enabled) return;

        // Nếu đang attack thì đứng yên
        bool isAttacking = animator.GetBool("isAttacking");
        if (!isAttacking && !isShooting)
        {
            HandleMovement();
            HandleJump();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("attack");
        }

        if (Input.GetKeyDown(KeyCode.C) && !isShooting && animator.GetBool("isAlive"))
        {
            if (currentMana > 0)
            {
                isShooting = true;
                currentMana--;
                if (manaBar != null)
                    manaBar.SetMana(currentMana, maxMana);
                animator.SetTrigger("rangeAttack");
            }
            else
            {
                Debug.Log("Không đủ mana để bắn cung!");
            }
        }
        manaRegenTimer += Time.deltaTime;
        if (manaRegenTimer >= manaRegenCooldown)
        {
            manaRegenTimer = 0f;
            if (currentMana < maxMana)
            {
                currentMana++;
                if (manaBar != null)
                    manaBar.SetMana(currentMana, maxMana);
            }
        }

        UpdateAnimation();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? moveSpeed : moveSpeed * 0.71f;

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // Lật mặt theo hướng di chuyển
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true;
        }
        else if (Input.GetButtonDown("Jump") && canDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = false;
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void UpdateAnimation()
    {
        float speedX = Mathf.Abs(rb.linearVelocity.x);
        bool isMoving = speedX > 0.1f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);
        bool isJumping = !isGrounded;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
    }

    public void Attack1DealDamage()
    {
        if (attackZone1 != null && attackZone1.isEnemyInZone && attackZone1.enemyTarget != null)
        {
            Vector2 dir = (attackZone1.enemyTarget.transform.position - transform.position).normalized;
            var orcHealth = attackZone1.enemyTarget.GetComponent<OrcHealth>();
            if (orcHealth != null && orcHealth.IsAlive())
            {
                orcHealth.TakeDamage(attackZone1.attackDamage, dir);
                return;
            }
            var slimeHealth = attackZone1.enemyTarget.GetComponent<SlimeHealth>();
            if (slimeHealth != null && slimeHealth.IsAlive())
            {
                slimeHealth.TakeDamage(attackZone1.attackDamage, dir);
            }
        }
    }

    public void Attack2DealDamage()
    {
        if (attackZone2 != null && attackZone2.isEnemyInZone && attackZone2.enemyTarget != null)
        {
            Vector2 dir = (attackZone2.enemyTarget.transform.position - transform.position).normalized;
            var orcHealth = attackZone2.enemyTarget.GetComponent<OrcHealth>();
            if (orcHealth != null && orcHealth.IsAlive())
            {
                orcHealth.TakeDamage(attackZone2.attackDamage, dir);
                return;
            }
            var slimeHealth = attackZone2.enemyTarget.GetComponent<SlimeHealth>();
            if (slimeHealth != null && slimeHealth.IsAlive())
            {
                slimeHealth.TakeDamage(attackZone2.attackDamage, dir);
            }
        }
    }

    public void Attack3DealDamage()
    {
        if (attackZone3 != null && attackZone3.isEnemyInZone && attackZone3.enemyTarget != null)
        {
            Vector2 dir = (attackZone3.enemyTarget.transform.position - transform.position).normalized;
            var orcHealth = attackZone3.enemyTarget.GetComponent<OrcHealth>();
            if (orcHealth != null && orcHealth.IsAlive())
            {
                orcHealth.TakeDamage(attackZone3.attackDamage, dir);
                return;
            }
            var slimeHealth = attackZone3.enemyTarget.GetComponent<SlimeHealth>();
            if (slimeHealth != null && slimeHealth.IsAlive())
            {
                slimeHealth.TakeDamage(attackZone3.attackDamage, dir);
            }
        }
    }

    public void ShootArrow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null) return;
        Vector2 shootDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        var arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        if (shootDir == Vector2.left)
            arrow.transform.localScale = new Vector3(-1, 1, 1);

        arrow.GetComponent<Arrow>().Init(shootDir);
    }

    public void EndShoot()
    {
        isShooting = false;
    }

    public void AddMana(int amount)
    {
        currentMana += amount;
        if (currentMana > maxMana) currentMana = maxMana;
        if (manaBar != null)
            manaBar.SetMana(currentMana, maxMana);
    }
}
