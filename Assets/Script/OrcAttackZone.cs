using UnityEngine;

public class OrcAttackZone : MonoBehaviour
{
    private OrcController orcController;

    void Start()
    {
        orcController = GetComponentInParent<OrcController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            orcController.isPlayerInAttackZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            orcController.isPlayerInAttackZone = false;
        }
    }
}
