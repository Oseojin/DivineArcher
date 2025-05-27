using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("����")]
    public float moveSpeed = 2f;
    public int maxHealth = 30;
    public int currentHealth;
    public int damageOnCollision = 10; // �÷��̾�� �浹 �� ���� ������
    public int experienceValue = 10;   // óġ �� ������ ����ġ

    [Header("����")]
    private Transform playerTransform;
    private Rigidbody2D rb;
    // public GameObject deathEffectPrefab; // ��� ȿ�� ������ (���� ����)
    public GameObject experienceGemPrefab; // ����ġ ���� ������ (���� �ܰ迡�� ����)

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        // �÷��̾ ã�Ƽ� ���� ���� (�� ���� ����� GameManager ���� ���� ���޹޴� ��)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("������ 'Player' �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�.");
            // �÷��̾ ã�� ���ϸ� ��Ȱ��ȭ�ϰų� �ٸ� �ൿ�� �ϵ��� ó�� ����
            // gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver || playerTransform == null)
        {
            rb.linearVelocity = Vector2.zero; // ���� ���� �� �Ǵ� �÷��̾ ������ ������ ����
            return;
        }

        // �÷��̾ ���� �̵�
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        // �÷��̾ �ٶ󺸵��� ȸ�� (���� ����, ��������Ʈ ���⿡ ���� �ʿ� ���� �� ����)
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        // rb.rotation = angle;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        // Debug.Log(gameObject.name + " ü��: " + currentHealth);

        // ������ �ǰ� ȿ�� (��: ���� ����, ���߿� ���� ����)
        // StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Debug.Log(gameObject.name + " ���!");

        // ��� ȿ�� ���� (���� ����)
        // if (deathEffectPrefab != null)
        // {
        //     Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        // }

        // ����ġ ���� ����
        if (experienceGemPrefab != null)
        {
            Instantiate(experienceGemPrefab, transform.position, Quaternion.identity);
            // ExperienceGem ��ũ��Ʈ�� ����ġ �� ���� ���� �߰� ����
            // GameObject gem = Instantiate(experienceGemPrefab, transform.position, Quaternion.identity);
            // gem.GetComponent<ExperienceGem>().Initialize(experienceValue);
        }
        else
        {
            // ����ġ ���� �������� ���ٸ� �÷��̾�� ���� ����ġ ���� (�׽�Ʈ��)
            if (playerTransform != null)
            {
                PlayerController player = playerTransform.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.GainXP(experienceValue);
                }
            }
        }

        Destroy(gameObject); // �� ������Ʈ �ı�
    }

    // �÷��̾�� �浹 �� ������ ó��
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageOnCollision);
                // ƨ�ܳ����� ȿ����, ��� ���� �ð� ���� ���⿡ �߰��� �� ����
                // Die(); // �÷��̾�� �浹�ϸ� �ٷ� �״� �� (��: ������)
            }
        }
    }

    // IEnumerator FlashRed() // ������ �ǰ� �� ������ ȿ��
    // {
    //     SpriteRenderer sr = GetComponent<SpriteRenderer>();
    //     if (sr != null)
    //     {
    //         Color originalColor = sr.color;
    //         sr.color = Color.red;
    //         yield return new WaitForSeconds(0.1f);
    //         sr.color = originalColor;
    //     }
    // }
}