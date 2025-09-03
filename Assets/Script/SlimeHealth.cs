using UnityEngine;
using System.Collections;

public class SlimeHealth : MonoBehaviour
{
    public int maxHealth = 2;
    private int currentHealth;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isAlive = true;
    private bool isTakingHit = false;

    public float knockbackForce = 7f;
    public float hitDelay = 0.2f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (!isAlive || isTakingHit) return;
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            animator.SetTrigger("isHit");
            StartCoroutine(HitRoutine(hitDirection));
        }
    }

    private IEnumerator HitRoutine(Vector2 hitDirection)
    {
        isTakingHit = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(hitDelay);
        isTakingHit = false;
    }

    private void Die()
    {
        isAlive = false;
        animator.SetBool("isDead", true); // Any State -> Slime_Death (isDead)
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        // Gọi DestroySelf bằng Animation Event ở cuối Slime_Death
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public bool IsAlive() => isAlive;
}
