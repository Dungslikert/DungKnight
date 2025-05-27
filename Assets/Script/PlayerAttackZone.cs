using UnityEngine;

public class PlayerAttackZone : MonoBehaviour
{
    public int attackDamage = 1;

    [HideInInspector] public bool isEnemyInZone = false;
    [HideInInspector] public MonoBehaviour enemyTarget = null; // Đa năng cho nhiều loại enemy

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Ưu tiên OrcHealth, nếu không có thì lấy SlimeHealth
            var orc = other.GetComponentInParent<OrcHealth>();
            if (orc != null)
            {
                enemyTarget = orc;
            }
            else
            {
                var slime = other.GetComponentInParent<SlimeHealth>();
                if (slime != null) enemyTarget = slime;
            }
            if (enemyTarget != null) isEnemyInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            isEnemyInZone = false;
            enemyTarget = null;
        }
    }
}
