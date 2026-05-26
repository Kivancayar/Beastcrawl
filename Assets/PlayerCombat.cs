using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Gerekli Atamalar")]
    public MovePlayer movePlayer;
    public LineRenderer lineRenderer;
    public Transform firePoint;

    [Header("Işın Ayarları")]
    public float range = 12f;
    public float damagePerSecond = 50f;
    public float manaCostPerSecond = 25f;

    // --- YENİ DEĞİŞKENLER: Dash sonrası lazer kilit kontrolü ---
    private bool dashSonrasiKilit = false;

    void Start()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    void Update()
    {
        if (movePlayer == null) return;

        // 1. ADIM: Karakter şu an dash atıyorsa, lazeri kapat ve kilidi aktif et
        if (movePlayer.isDashing)
        {
            if (lineRenderer != null) lineRenderer.enabled = false;
            dashSonrasiKilit = true; // "Kıvanç dash attı, lazeri kilitle" dedik
            return;
        }

        // 2. ADIM: Dash bitti ama Kıvanç hala parmağını F'den çekmediyse, lazeri kapalı tut
        if (dashSonrasiKilit)
        {
            // Oyuncu parmağını F tuşundan ÇEKTİĞİ ANDA kilit açılır
            if (!Input.GetKey(KeyCode.F))
            {
                dashSonrasiKilit = false;
            }

            // Parmağını çekene kadar aşağıdaki ateşleme kodlarına asla geçme:
            if (lineRenderer != null) lineRenderer.enabled = false;
            return;
        }

        // 3. ADIM: Normal Ateşleme Kontrolü
        if (Input.GetKey(KeyCode.F) && movePlayer.playerMana > 0)
        {
            ShootBeam();
            movePlayer.UseMana(manaCostPerSecond * Time.deltaTime);
        }
        else
        {
            if (lineRenderer != null) lineRenderer.enabled = false;
        }
    }

    void ShootBeam()
    {
        if (lineRenderer == null || movePlayer == null) return;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, Vector3.zero);

        Vector2 direction = movePlayer.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        int layerMask = ~(1 << LayerMask.NameToLayer("Player"));
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, range, layerMask);

        if (hit.collider != null)
        {
            float hitDistance = Vector2.Distance(firePoint.position, hit.point);
            lineRenderer.SetPosition(1, new Vector3(hitDistance, 0, 0));

            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }
        else
        {
            lineRenderer.SetPosition(1, new Vector3(range, 0, 0));
        }
    }
}