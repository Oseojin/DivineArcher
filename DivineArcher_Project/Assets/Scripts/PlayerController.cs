using UnityEngine;
using System.Collections.Generic;
using System.Linq; // LINQ 사용을 위해 추가

// AcquiredItem 클래스는 이전 단계와 동일하게 유지
[System.Serializable]
public class AcquiredItem
{
    public string itemId;
    public int currentLevel;
    public float lastFiredTime;

    public AcquiredItem(string id, int level)
    {
        itemId = id;
        currentLevel = level;
        lastFiredTime = 0f;
    }
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 lastLookDirection = Vector2.right;

    [Header("Player Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int level = 1;
    public float experience = 0f;
    public float xpToNextLevel = 10f;

    [Header("Combat")]
    public List<AcquiredItem> acquiredArrows = new List<AcquiredItem>();
    public List<AcquiredItem> acquiredArtifacts = new List<AcquiredItem>();
    public string initialArrowId = "BASIC_ARROW";
    public Transform firePoint;
    public float autoAimRange = 15f; // 자동 조준 범위

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (firePoint == null)
        {
            firePoint = transform;
        }

        if (!string.IsNullOrEmpty(initialArrowId) && ItemDatabase.Instance != null)
        {
            ArrowData initialArrowData = ItemDatabase.Instance.GetArrowData(initialArrowId);
            if (initialArrowData != null)
            {
                acquiredArrows.Add(new AcquiredItem(initialArrowId, 1));
                Debug.Log(initialArrowData.itemName + " 획득! (ID: " + initialArrowData.itemId + ")");
            }
            else
            {
                Debug.LogError(initialArrowId + " ID를 가진 화살 데이터를 ItemDatabase에서 찾을 수 없습니다.");
            }
        }
        else if (ItemDatabase.Instance == null)
        {
            Debug.LogError("ItemDatabase 인스턴스를 찾을 수 없습니다. 씬에 ItemDatabase 오브젝트가 있고 스크립트가 연결되었는지 확인하세요.");
        }
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        if (movementInput.sqrMagnitude > 0.01f)
        {
            lastLookDirection = movementInput.normalized;
        }

        HandleAttacks();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused) return;

        if (movementInput.sqrMagnitude > 0)
        {
            rb.MovePosition(rb.position + movementInput.normalized * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    Transform FindClosestEnemy()
    {
        Transform closestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        // "Enemy" 태그를 가진 모든 게임 오브젝트를 찾습니다.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObject in enemies)
        {
            float distanceToEnemy = Vector3.Distance(position, enemyObject.transform.position);
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= autoAimRange)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemyObject.transform;
            }
        }
        return closestEnemy;
    }

    void HandleAttacks()
    {
        for (int i = 0; i < acquiredArrows.Count; i++)
        {
            AcquiredItem arrowInstance = acquiredArrows[i];
            ArrowData arrowData = ItemDatabase.Instance.GetArrowData(arrowInstance.itemId);
            if (arrowData == null) continue;

            float currentCooldown = CalculateArrowCooldown(arrowData, arrowInstance);

            if (Time.time >= arrowInstance.lastFiredTime + currentCooldown)
            {
                FireArrow(arrowInstance, arrowData);
                arrowInstance.lastFiredTime = Time.time;
            }
        }
    }

    void FireArrow(AcquiredItem instance, ArrowData data)
    {
        if (data.projectilePrefab == null)
        {
            Debug.LogWarning(data.itemName + "의 projectilePrefab이 할당되지 않았습니다.");
            return;
        }

        Vector3 spawnPosition = firePoint.position;
        Vector2 fireDirection = lastLookDirection; // 기본 발사 방향
        Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, lastLookDirection);

        Transform closestEnemyTransform = FindClosestEnemy();
        if (closestEnemyTransform != null)
        {
            fireDirection = (closestEnemyTransform.position - spawnPosition).normalized;
            spawnRotation = Quaternion.LookRotation(Vector3.forward, fireDirection); // 2D에서는 up 벡터를 사용
        }
        // 2D에서 스프라이트가 오른쪽을 보도록 디자인되었다면, 각도를 직접 설정하는 것이 더 직관적일 수 있습니다.
        // float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        // spawnRotation = Quaternion.Euler(0, 0, angle);


        GameObject projectileGO = Instantiate(data.projectilePrefab, spawnPosition, spawnRotation);
        Projectile projectileScript = projectileGO.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            float currentDamage = data.baseDamage + (data.damagePerLevel * (instance.currentLevel - 1));
            float currentSpeed = data.baseProjectileSpeed + (data.projectileSpeedPerLevel * (instance.currentLevel - 1));
            int currentPierce = data.basePierceCount + (data.pierceCountPerLevel * (instance.currentLevel - 1));

            projectileScript.Initialize(fireDirection, currentSpeed, (int)currentDamage, currentPierce);
        }
        else
        {
            Debug.LogError(data.projectilePrefab.name + "에 Projectile 스크립트가 없습니다.");
        }
    }

    float CalculateArrowCooldown(ArrowData arrowData, AcquiredItem arrowInstance)
    {
        float finalCooldown = arrowData.baseCooldown - (arrowData.cooldownReductionPerLevel * (arrowInstance.currentLevel - 1));
        foreach (AcquiredItem artifact in acquiredArtifacts)
        {
            ArtifactData artifactData = ItemDatabase.Instance.GetArtifactData(artifact.itemId);
            if (artifactData != null)
            {
                foreach (StatModifier mod in artifactData.statModifiersPerLevel)
                {
                    if (mod.type == StatModifierType.AttackSpeedPercentage)
                    {
                        finalCooldown *= (1f - (mod.value * artifact.currentLevel / 100f));
                    }
                }
            }
        }
        return Mathf.Max(0.1f, finalCooldown);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("플레이어 체력: " + currentHealth + "/" + maxHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.GameOver();
        }
    }

    public void GainXP(float amount)
    {
        if (GameManager.Instance.IsGameOver) return;
        experience += amount;
        Debug.Log("경험치 획득: " + amount + " / 현재 경험치: " + experience + " / 다음 레벨까지: " + (xpToNextLevel - experience));
        if (experience >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        experience -= xpToNextLevel; // 현재 레벨 초과 경험치 이월
        xpToNextLevel = Mathf.Floor(xpToNextLevel * 1.5f);
        Debug.Log("레벨 업! 현재 레벨: " + level + ". 다음 레벨 필요 경험치: " + xpToNextLevel);
        GameManager.Instance.ShowLevelUpOptions();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (UIManager.Instance != null) UIManager.Instance.UpdatePlayerStatsUI(); // UI 즉시 업데이트
        Debug.Log(amount + " 체력 회복. 현재 체력: " + currentHealth);
    }
}