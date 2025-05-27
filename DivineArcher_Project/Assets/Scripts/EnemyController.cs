using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("스탯")]
    public float moveSpeed = 2f;
    public int maxHealth = 30;
    public int currentHealth;
    public int damageOnCollision = 10; // 플레이어와 충돌 시 입힐 데미지
    public int experienceValue = 10;   // 처치 시 제공할 경험치

    [Header("참조")]
    private Transform playerTransform;
    private Rigidbody2D rb;
    // public GameObject deathEffectPrefab; // 사망 효과 프리팹 (선택 사항)
    public GameObject experienceGemPrefab; // 경험치 보석 프리팹 (다음 단계에서 생성)

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        // 플레이어를 찾아서 참조 저장 (더 좋은 방법은 GameManager 등을 통해 전달받는 것)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("씬에서 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.");
            // 플레이어를 찾지 못하면 비활성화하거나 다른 행동을 하도록 처리 가능
            // gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver || playerTransform == null)
        {
            rb.linearVelocity = Vector2.zero; // 게임 오버 시 또는 플레이어가 없으면 움직임 정지
            return;
        }

        // 플레이어를 향해 이동
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        // 플레이어를 바라보도록 회전 (선택 사항, 스프라이트 방향에 따라 필요 없을 수 있음)
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        // rb.rotation = angle;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        // Debug.Log(gameObject.name + " 체력: " + currentHealth);

        // 간단한 피격 효과 (예: 색상 변경, 나중에 개선 가능)
        // StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Debug.Log(gameObject.name + " 사망!");

        // 사망 효과 생성 (선택 사항)
        // if (deathEffectPrefab != null)
        // {
        //     Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        // }

        // 경험치 보석 생성
        if (experienceGemPrefab != null)
        {
            Instantiate(experienceGemPrefab, transform.position, Quaternion.identity);
            // ExperienceGem 스크립트에 경험치 값 전달 로직 추가 가능
            // GameObject gem = Instantiate(experienceGemPrefab, transform.position, Quaternion.identity);
            // gem.GetComponent<ExperienceGem>().Initialize(experienceValue);
        }
        else
        {
            // 경험치 보석 프리팹이 없다면 플레이어에게 직접 경험치 지급 (테스트용)
            if (playerTransform != null)
            {
                PlayerController player = playerTransform.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.GainXP(experienceValue);
                }
            }
        }

        Destroy(gameObject); // 적 오브젝트 파괴
    }

    // 플레이어와 충돌 시 데미지 처리
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageOnCollision);
                // 튕겨나가는 효과나, 잠시 무적 시간 등을 여기에 추가할 수 있음
                // Die(); // 플레이어와 충돌하면 바로 죽는 적 (예: 자폭형)
            }
        }
    }

    // IEnumerator FlashRed() // 간단한 피격 시 깜빡임 효과
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