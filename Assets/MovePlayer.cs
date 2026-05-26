using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovePlayer : MonoBehaviour
{
    [Header("Hareket ve Zıplama")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;

    [SerializeField] private float yanaItmeGucu = 15f;
    [SerializeField] private float yukariItmeGucu = 4f;

    [Header("Dash Ayarları")]
    public float dashForce = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public float dashManaCost = 20f;
    private bool canDash = true;
    public bool isDashing;
    private bool isKnockback;

    [Header("Can ve Mana")]
    public float playerHealth = 100f;
    public float playerMana = 100f;
    public float manaRegenSpeed = 10f;
    public Slider healthSlider;
    public Slider manaSlider;

    [Header("Hasar Sistemi")]
    public float damageCooldown = 1f;
    private bool canTakeDamage = true;

    [Header("Lazer ve Nişan Ayarları")]
    public Transform firePoint;
    public LayerMask canavarLayerMask;
    public float laserRange = 20f;
    public float laserDamage = 25f;

    private Rigidbody2D rb;

    void Start() { rb = GetComponent<Rigidbody2D>(); }

    void Update()
    {
        if (isDashing) return;

        if (!isKnockback)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        if (Input.GetButtonDown("Jump") && isGrounded && !isKnockback)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && playerMana >= dashManaCost && !isKnockback)
            StartCoroutine(DashAction());

        if (playerMana < 100)
        {
            playerMana += manaRegenSpeed * Time.deltaTime;
            if (playerMana > 100) playerMana = 100;
        }

        if (healthSlider) healthSlider.value = playerHealth;
        if (manaSlider) manaSlider.value = playerMana;

        if (!isKnockback && !isDashing && firePoint != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookDir = mousePos - firePoint.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

            if (transform.localScale.x < 0)
            {
                angle += 180f;
            }

            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (Input.GetMouseButtonDown(1) && !isKnockback && !isDashing)
        {
            ShootLaser360();
        }
    }

    private IEnumerator DashAction()
    {
        canDash = false;
        isDashing = true;
        playerMana -= dashManaCost;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashForce, 0f);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void TakeDamage(float amount, Vector2 knockbackDirection)
    {
        if (canTakeDamage)
        {
            playerHealth -= amount;

            StartCoroutine(KnockbackRoutine());
            StartCoroutine(Camera.main.GetComponent<CameraFollow>().Shake());

            Debug.Log("OYUNCU HASAR ALDI! Kalan: " + playerHealth);

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(knockbackDirection.x * yanaItmeGucu, yukariItmeGucu), ForceMode2D.Impulse);

            if (playerHealth <= 0)
            {
                playerHealth = 0;
                Debug.Log("OYUNCU ÖLDÜ!");
                gameObject.SetActive(false);
            }
            StartCoroutine(DamageProtection());
        }
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnockback = true;
        yield return new WaitForSeconds(0.2f);
        isKnockback = false;
    }

    private IEnumerator DamageProtection()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    public void UseMana(float amount)
    {
        playerMana -= amount;
        if (playerMana < 0) playerMana = 0;
    }

    private void ShootLaser360()
    {
        if (firePoint == null) return;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        Vector2 shootDirection = ((Vector2)mouseWorldPosition - (Vector2)firePoint.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, shootDirection, laserRange, canavarLayerMask);

        if (hit.collider != null)
        {
            Debug.DrawLine(firePoint.position, hit.point, Color.green, 0.5f);
            Debug.Log(hit.collider.name + " canavarına 360 dereceyle vuruldu!");
        }
        else
        {
            Vector2 endPoint = (Vector2)firePoint.position + (shootDirection * laserRange);
            Debug.DrawLine(firePoint.position, endPoint, Color.red, 0.5f);
        }
    }
}