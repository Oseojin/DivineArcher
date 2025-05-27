using UnityEngine;
using System.Collections; // �ڷ�ƾ ���

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // ������ �� ������
    public float spawnInterval = 3f; // �ʱ� ���� ����
    public float minSpawnInterval = 0.5f; // �ּ� ���� ����
    public float spawnIntervalReductionRate = 0.05f; // �ð� ����� ���� ���� ���� ������ (�ʴ�)
    public float spawnRadius = 10f; // �÷��̾�κ��� �� �ݰ� �ٱ��� ����

    private Transform playerTransform;
    private Camera mainCamera;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        mainCamera = Camera.main;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (!GameManager.Instance.IsGameOver) // ���� ������ �ƴ� ���� ��� ����
        {
            yield return new WaitForSeconds(spawnInterval);

            if (playerTransform != null && enemyPrefab != null && !GameManager.Instance.IsPaused) // �Ͻ����� �ƴ� ���� ����
            {
                SpawnSingleEnemy();
            }

            // �ð� ����� ���� ���� ���� ���� ����
            if (spawnInterval > minSpawnInterval)
            {
                spawnInterval -= spawnIntervalReductionRate * Time.deltaTime; // ���� �ð� ��� ����
                spawnInterval = Mathf.Max(spawnInterval, minSpawnInterval);
            }
        }
    }

    void SpawnSingleEnemy()
    {
        // ȭ�� �����ڸ� �Ǵ� �÷��̾� �þ� �� Ư�� ��ġ�� ����
        // �����ϰ� �÷��̾� �ֺ� �������� ���� ��ġ ����
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 spawnDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        Vector3 spawnPosition = playerTransform.position + spawnDirection * spawnRadius;

        // ���� ��ġ�� ī�޶� �þ� �ۿ� �ֵ��� ���� (������ ����)
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(spawnPosition);
        if (viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1)
        {
            // ���� �þ� ���̸� �� �ָ� ���� (�� ������ ���� �ʿ� ����)
            spawnPosition = playerTransform.position + spawnDirection * (spawnRadius + 5f);
        }

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
