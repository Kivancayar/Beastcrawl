using UnityEngine;

public class Coin : MonoBehaviour
{
    private AudioSource coinSound;

    private void Start()
    {
        coinSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Skoru 1 artır
            GameManager.score += 1;

            // Sesi çal
            if (coinSound != null && coinSound.clip != null)
            {
                // Sesi kameranın olduğu yere çaldır (çünkü coin yok olunca ses de kesilmesin)
                AudioSource.PlayClipAtPoint(coinSound.clip, transform.position);
            }

            // Coin'i sil
            Destroy(gameObject);
        }
    }
}