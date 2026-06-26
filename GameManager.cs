using UnityEngine;
using UnityEngine.UI; // Text kullanıyorsan bu lazım

public class GameManager : MonoBehaviour
{
    public static int score = 0; // Her yerden ulaşmak için static yaptık
    public Text scoreText; // Unity'de Text objesini buraya sürükle

    void Update()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}