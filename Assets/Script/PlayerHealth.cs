using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private Animator animator;
    private GameManager gameManager;
    private Rigidbody2D rb;
    private PlayerController playerController;

    public float knockbackForce = 7f;
    public float hitDelay = 0.2f;
    private bool isAlive = true;
    private bool isInvulnerable = false;

    // Thêm biến này để gắn HealthBar trên Inspector
    public HealthBar healthBar;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindAnyObjectByType<GameManager>();
        playerController = GetComponent<PlayerController>();
        currentHealth = maxHealth;
        animator.SetBool("isAlive", true);

        // Cập nhật thanh máu lúc bắt đầu game
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (!isAlive || isInvulnerable) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // Cập nhật thanh máu
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitRoutine(hitDirection));
        }
    }

    private IEnumerator HitRoutine(Vector2 hitDirection)
    {
        isInvulnerable = true;
        animator.SetTrigger("isHit");
        if (playerController != null)
            playerController.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(hitDelay);

        if (playerController != null && isAlive)
            playerController.enabled = true;
        isInvulnerable = false;
    }

    private void Die()
    {
        isAlive = false;
        animator.SetBool("isAlive", false);
        if (playerController != null)
            playerController.enabled = false;
        rb.linearVelocity = Vector2.zero;
    }

    // Hàm này sẽ được gọi từ Animation Event (chỉ cần public, không static)
    public void ShowGameOver()
    {
        if (gameManager != null)
            gameManager.GameOver();
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    // Hàm này dùng để hồi máu (nếu cần)
    public void Heal(int amount)
    {
        if (!isAlive) return;
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }
}
