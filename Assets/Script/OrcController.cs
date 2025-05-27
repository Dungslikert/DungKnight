using UnityEngine;

public class OrcController : MonoBehaviour
{
    // AI Movement
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public float detectionRange = 5f;
    public float patrolRange = 5f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform wallCheck;

    // AI State
    private bool isChasing = false;
    private bool isReturning = false;
    private bool facingRight = true;
    private Vector3 startPos;

    // Attack zone sync
    [HideInInspector]
    public bool isPlayerInAttackZone = false;

    // Delay/Timers
    private float edgeWaitTimer = 0f;
    private float maxEdgeWaitTime = 1f;
    private float chaseCooldownTimer = 0f;
    private float maxChaseCooldown = 1f;

    // Component cache
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private OrcHealth orcHealth; // Tham chiếu script máu mới

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        orcHealth = GetComponent<OrcHealth>(); // Lấy component máu
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPos = transform.position;
    }

    void Update()
    {
        animator.SetBool("hasTarget", isPlayerInAttackZone);

        // Đã chết thì không làm gì nữa (check qua OrcHealth)
        if (orcHealth != null && !orcHealth.IsAlive()) return;

        // Nếu đang tấn công thì không check chase/patrol
        if (isPlayerInAttackZone) return;

        // Nếu đang trở về điểm tuần tra thì không check detect/chase
        if (isReturning)
        {
            isChasing = false;
            return;
        }

        if (chaseCooldownTimer > 0)
        {
            chaseCooldownTimer -= Time.deltaTime;
            isChasing = false;
            return;
        }

        // Logic chuyển đổi trạng thái chase/patrol
        if (isChasing && !PlayerInRange())
        {
            isChasing = false;
            isReturning = true;
        }
        else if (!isChasing && PlayerInRange())
        {
            isChasing = true;
            isReturning = false;
        }
    }

    void FixedUpdate()
    {
        // Chết thì không di chuyển
        if (orcHealth != null && !orcHealth.IsAlive()) return;

        if (isPlayerInAttackZone)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        else if (isChasing)
        {
            ChasePlayer();
        }
        else if (isReturning)
        {
            ReturnToStartPos();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        bool isBlocked = Physics2D.Raycast(wallCheck.position, facingRight ? Vector2.right : Vector2.left, 0.1f, groundLayer);
        float patrolDistance = transform.position.x - startPos.x;

        if (!isGroundAhead || isBlocked || Mathf.Abs(patrolDistance) >= patrolRange)
        {
            Flip();
        }
        rb.linearVelocity = new Vector2((facingRight ? 1 : -1) * patrolSpeed, rb.linearVelocity.y);
    }

    private void ChasePlayer()
    {
        if (player == null) return;
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);

        if (!isGroundAhead)
        {
            rb.linearVelocity = Vector2.zero;
            edgeWaitTimer += Time.fixedDeltaTime;
            if (edgeWaitTimer >= maxEdgeWaitTime)
            {
                edgeWaitTimer = 0f;
                Flip();
                isChasing = false;
                chaseCooldownTimer = maxChaseCooldown;
            }
            return;
        }
        edgeWaitTimer = 0f;

        if (player.position.x > transform.position.x)
        {
            if (!facingRight) Flip();
            rb.linearVelocity = new Vector2(chaseSpeed, rb.linearVelocity.y);
        }
        else
        {
            if (facingRight) Flip();
            rb.linearVelocity = new Vector2(-chaseSpeed, rb.linearVelocity.y);
        }
    }

    private void ReturnToStartPos()
    {
        if (Vector2.Distance(transform.position, startPos) < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            isReturning = false;
            return;
        }

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        if (!isGroundAhead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (startPos.x > transform.position.x)
        {
            if (!facingRight) Flip();
            rb.linearVelocity = new Vector2(patrolSpeed, rb.linearVelocity.y);
        }
        else
        {
            if (facingRight) Flip();
            rb.linearVelocity = new Vector2(-patrolSpeed, rb.linearVelocity.y);
        }
    }

    private bool PlayerInRange()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 1f);

        if (wallCheck != null)
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (facingRight ? Vector3.right : Vector3.left) * 0.1f);
    }

    // --- Orc Attack gọi Animation Event ---
    public void DealDamage()
    {
        if (orcHealth != null && !orcHealth.IsAlive()) return;
        if (isPlayerInAttackZone)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                var playerHealth = playerObj.GetComponent<PlayerHealth>();
                if (playerHealth != null && playerHealth.IsAlive())
                {
                    Vector2 hitDir = (playerObj.transform.position - transform.position).normalized;
                    playerHealth.TakeDamage(1, hitDir);
                }
            }
        }
    }
}
