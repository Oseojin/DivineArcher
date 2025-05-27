using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    public int experienceValue = 5; // 기본 경험치 값
    public float collectionRadius = 0.5f; // 플레이어와 이 거리만큼 가까워지면 수집됨
    public float magnetRadius = 3f; // 이 반경 내 플레이어에게 끌려가기 시작
    public float magnetSpeed = 5f; // 끌려가는 속도

    private Transform playerTransform;
    private bool isMagnetized = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        // 일정 시간 후 자동 파괴 (선택 사항, 너무 많이 쌓이는 것 방지)
        // Destroy(gameObject, 20f);
    }

    public void Initialize(int value)
    {
        experienceValue = value;
    }

    void Update()
    {
        if (playerTransform == null || GameManager.Instance.IsGameOver) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= collectionRadius)
        {
            Collect();
        }
        else if (distanceToPlayer <= magnetRadius || isMagnetized)
        {
            isMagnetized = true; // 한번 끌리기 시작하면 계속 끌림
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, magnetSpeed * Time.deltaTime);
        }
    }

    void Collect()
    {
        PlayerController player = playerTransform.GetComponent<PlayerController>();
        if (player != null)
        {
            player.GainXP(experienceValue);
            // Debug.Log(experienceValue + " 경험치 획득 (보석)");
        }
        Destroy(gameObject); // 수집 후 파괴
    }

    // 플레이어와의 충돌로도 수집되도록 (선택 사항, Update에서 이미 처리 중)
    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         Collect();
    //     }
    // }
}
