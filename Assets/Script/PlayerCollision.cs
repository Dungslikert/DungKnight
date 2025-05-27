using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;
    private NextLevel nextLevel;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManager>();
        nextLevel = FindAnyObjectByType<NextLevel>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("This collider name: " + gameObject.name + " | collision: " + collision.name);
        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            audioManager.PlayCoinSound();
            gameManager.AddScore(1);
        }
        else if (collision.CompareTag("Trap"))
        {
            if (gameObject.CompareTag("Player") && gameObject.name == "Player")
            {
                PlayerHealth playerHealth = GetComponent<PlayerHealth>();
                if (playerHealth != null && playerHealth.IsAlive())
                {
                    Vector2 hitDir = (transform.position - collision.transform.position).normalized;
                    playerHealth.TakeDamage(1, hitDir);
                }
            }
        }

        else if (collision.CompareTag("Heal"))
        {
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.IsAlive())
            {
                playerHealth.Heal(2); 
            }
            Destroy(collision.gameObject);
            audioManager.PlayCoinSound();
        }

        else if (collision.CompareTag("HealS"))
        {
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.IsAlive())
            {
                playerHealth.Heal(5); 
            }
            Destroy(collision.gameObject);
            audioManager.PlayCoinSound();
        }

        else if (collision.CompareTag("Mana"))
        {
            PlayerController playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.AddMana(3); 
            }
            Destroy(collision.gameObject);
            audioManager.PlayCoinSound();
        }

        else if (collision.CompareTag("Key"))
        {
            Destroy(collision.gameObject) ;
            gameManager.GameWin();
        }
        else if (collision.CompareTag("Sign"))
        {
            nextLevel.LoadManChoiMoi();
        }
    }
}
