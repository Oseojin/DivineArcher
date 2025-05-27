using UnityEngine;
using System.Collections.Generic;
using System.Linq; // LINQ ����� ���� �߰�

// AcquiredItem Ŭ������ ���� �ܰ�� �����ϰ� ����
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
    public float autoAimRange = 15f; // �ڵ� ���� ����

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
                Debug.Log(initialArrowData.itemName + " ȹ��! (ID: " + initialArrowData.itemId + ")");
            }
            else
            {
                Debug.LogError(initialArrowId + " ID�� ���� ȭ�� �����͸� ItemDatabase���� ã�� �� �����ϴ�.");
            }
        }
        else if (ItemDatabase.Instance == null)
        {
            Debug.LogError("ItemDatabase �ν��Ͻ��� ã�� �� �����ϴ�. ���� ItemDatabase ������Ʈ�� �ְ� ��ũ��Ʈ�� ����Ǿ����� Ȯ���ϼ���.");
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

        // "Enemy" �±׸� ���� ��� ���� ������Ʈ�� ã���ϴ�.
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
            Debug.LogWarning(data.itemName + "�� projectilePrefab�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        Vector3 spawnPosition = firePoint.position;
        Vector2 fireDirection = lastLookDirection; // �⺻ �߻� ����
        Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, lastLookDirection);

        Transform closestEnemyTransform = FindClosestEnemy();
        if (closestEnemyTransform != null)
        {
            fireDirection = (closestEnemyTransform.position - spawnPosition).normalized;
            spawnRotation = Quaternion.LookRotation(Vector3.forward, fireDirection); // 2D������ up ���͸� ���
        }
        // 2D���� ��������Ʈ�� �������� ������ �����εǾ��ٸ�, ������ ���� �����ϴ� ���� �� �������� �� �ֽ��ϴ�.
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
            Debug.LogError(data.projectilePrefab.name + "�� Projectile ��ũ��Ʈ�� �����ϴ�.");
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
        Debug.Log("�÷��̾� ü��: " + currentHealth + "/" + maxHealth);
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
        Debug.Log("����ġ ȹ��: " + amount + " / ���� ����ġ: " + experience + " / ���� ��������: " + (xpToNextLevel - experience));
        if (experience >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        experience -= xpToNextLevel; // ���� ���� �ʰ� ����ġ �̿�
        xpToNextLevel = Mathf.Floor(xpToNextLevel * 1.5f);
        Debug.Log("���� ��! ���� ����: " + level + ". ���� ���� �ʿ� ����ġ: " + xpToNextLevel);
        GameManager.Instance.ShowLevelUpOptions();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (UIManager.Instance != null) UIManager.Instance.UpdatePlayerStatsUI(); // UI ��� ������Ʈ
        Debug.Log(amount + " ü�� ȸ��. ���� ü��: " + currentHealth);
    }
}