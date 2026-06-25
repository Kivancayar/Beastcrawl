using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Karakterin "Player" tag'ine sahip olup olmadığını kontrol et
        if (collision.CompareTag("Player"))
        {
            // Eğer karakterse, coin'i sahneden sil
            Destroy(gameObject);
            Debug.Log("Coin toplandı!");
        }
    }
}