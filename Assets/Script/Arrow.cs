using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;
    public float lifeTime = 2f;

    private Vector2 moveDir;

    public void Init(Vector2 dir)
    {
        moveDir = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var orcHealth = collision.GetComponent<OrcHealth>();
            if (orcHealth != null && orcHealth.IsAlive())
            {
                Vector2 hitDir = (collision.transform.position - transform.position).normalized;
                orcHealth.TakeDamage(damage, hitDir);
                Destroy(gameObject);
                return;
            }
            var slimeHealth = collision.GetComponent<SlimeHealth>();
            if (slimeHealth != null && slimeHealth.IsAlive())
            {
                Vector2 hitDir = (collision.transform.position - transform.position).normalized;
                slimeHealth.TakeDamage(damage, hitDir);
                Destroy(gameObject);
                return;
            }

        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
