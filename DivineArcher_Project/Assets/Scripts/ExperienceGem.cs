using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    public int experienceValue = 5; // �⺻ ����ġ ��
    public float collectionRadius = 0.5f; // �÷��̾�� �� �Ÿ���ŭ ��������� ������
    public float magnetRadius = 3f; // �� �ݰ� �� �÷��̾�� �������� ����
    public float magnetSpeed = 5f; // �������� �ӵ�

    private Transform playerTransform;
    private bool isMagnetized = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        // ���� �ð� �� �ڵ� �ı� (���� ����, �ʹ� ���� ���̴� �� ����)
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
            isMagnetized = true; // �ѹ� ������ �����ϸ� ��� ����
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, magnetSpeed * Time.deltaTime);
        }
    }

    void Collect()
    {
        PlayerController player = playerTransform.GetComponent<PlayerController>();
        if (player != null)
        {
            player.GainXP(experienceValue);
            // Debug.Log(experienceValue + " ����ġ ȹ�� (����)");
        }
        Destroy(gameObject); // ���� �� �ı�
    }

    // �÷��̾���� �浹�ε� �����ǵ��� (���� ����, Update���� �̹� ó�� ��)
    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         Collect();
    //     }
    // }
}
