using UnityEngine;
using System.Collections; // 코루틴 사용

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 스폰할 적 프리팹
    public float spawnInterval = 3f; // 초기 스폰 간격
    public float minSpawnInterval = 0.5f; // 최소 스폰 간격
    public float spawnIntervalReductionRate = 0.05f; // 시간 경과에 따른 스폰 간격 감소율 (초당)
    public float spawnRadius = 10f; // 플레이어로부터 이 반경 바깥에 스폰

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
        while (!GameManager.Instance.IsGameOver) // 게임 오버가 아닐 동안 계속 스폰
        {
            yield return new WaitForSeconds(spawnInterval);

            if (playerTransform != null && enemyPrefab != null && !GameManager.Instance.IsPaused) // 일시정지 아닐 때만 스폰
            {
                SpawnSingleEnemy();
            }

            // 시간 경과에 따라 스폰 간격 점차 감소
            if (spawnInterval > minSpawnInterval)
            {
                spawnInterval -= spawnIntervalReductionRate * Time.deltaTime; // 실제 시간 기반 감소
                spawnInterval = Mathf.Max(spawnInterval, minSpawnInterval);
            }
        }
    }

    void SpawnSingleEnemy()
    {
        // 화면 가장자리 또는 플레이어 시야 밖 특정 위치에 스폰
        // 간단하게 플레이어 주변 원형으로 스폰 위치 결정
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 spawnDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        Vector3 spawnPosition = playerTransform.position + spawnDirection * spawnRadius;

        // 스폰 위치가 카메라 시야 밖에 있도록 보정 (간단한 예시)
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(spawnPosition);
        if (viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1)
        {
            // 아직 시야 안이면 더 멀리 보냄 (더 정교한 로직 필요 가능)
            spawnPosition = playerTransform.position + spawnDirection * (spawnRadius + 5f);
        }

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
