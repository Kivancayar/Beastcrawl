using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public MovePlayer player;

    public void UpgradeSpeed()
    {
        player.moveSpeed += 1f;
        Debug.Log("Speed arttı! Yeni Speed: " + player.moveSpeed);
    }

    public void UpgradeDamage()
    {
        player.laserDamage += 1f;
        Debug.Log("Damage arttı! Yeni Damage: " + player.laserDamage);
    }
}