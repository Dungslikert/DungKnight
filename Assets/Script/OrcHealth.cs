using UnityEngine;
using System.Collections;

public class OrcHealth : MonoBehaviour
{
    public int maxHealth = 2;
    public float knockbackForce = 7f;
    public float hitDelay = 0.2f;

    private int currentHealth;
    private bool isAlive = true;
    private bool isInvulnerable = false;
    private Animator animator;
    private Rigidbody2D rb;
    private OrcController orcController;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        orcController = GetComponent<OrcController>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (!isAlive || isInvulnerable) return;

        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
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
        if (orcController != null) orcController.enabled = false; // tuỳ nếu bạn muốn dừng AI
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(hitDelay);
        if (orcController != null && isAlive) orcController.enabled = true;
        isInvulnerable = false;
    }

    private void Die()
    {
        isAlive = false;
        animator.SetBool("isDead", true);
        if (orcController != null) orcController.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        // Gọi DestroySelf bằng Animation Event
    }

    public bool IsAlive() => isAlive;
    public void DestroySelf() => Destroy(gameObject);
}
