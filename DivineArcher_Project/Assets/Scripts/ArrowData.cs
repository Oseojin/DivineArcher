using UnityEngine;

// ȭ���� �߻� ���� ���� �����ϱ� ���� Enum (����)
public enum ArrowType
{
    SingleProjectile, // ���� ����ü
    Orbiting,         // �ֺ� ȸ����
    MultiShot,        // �ٹ� �߻���
    Beam              // �� ���� (���� Ȯ���)
}

[CreateAssetMenu(fileName = "New Arrow Data", menuName = "Divine Archer/Arrow Data")]
public class ArrowData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string itemId; // ���� ID (��: "BASIC_ARROW", "BLADE_ORBIT_ARROW")
    public string itemName; // ���� �� ǥ�õ� �̸� (��: "�⺻ ȭ��")
    [TextArea(3, 5)]
    public string descriptionTemplate; // ������ ������ ���� ���ø� (��: "�������� ȭ���� �߻��մϴ�. ���ط�: {0}, �ӵ�: {1}")
    public Sprite icon; // UI�� ǥ�õ� ������

    [Header("���� ����")]
    public int maxLevel = 5;

    [Header("���� ����")]
    public ArrowType arrowType = ArrowType.SingleProjectile;
    public GameObject projectilePrefab; // �߻�� ����ü ������
    public float baseDamage = 10f;
    public float baseCooldown = 1.0f; // �⺻ �߻� ���� (��)
    public float baseProjectileSpeed = 10f;
    public int basePierceCount = 0; // �⺻ ���� Ƚ�� (0�̸� ���� ����)

    // ������ ���� ������ (����, �� ������ ������ ����)
    [Header("������ ���� ������")]
    public float damagePerLevel = 2f;
    public float cooldownReductionPerLevel = 0.05f; // ������ ��ٿ� ���ҷ�
    public float projectileSpeedPerLevel = 0.5f;
    public int pierceCountPerLevel = 0; // ������ ���� ���� (�ʿ��)

    // Ư�� ������ �������� �� ����ü ���� ���� ���� Ư�� ȿ���� ���� �迭 (���� ����)
    // public int[] projectilesAtLevel = {1, 1, 2, 2, 3}; // ���� 1~5�� �� �߻�Ǵ� ����ü ��

    // ������ ���� ������ �����ϴ� �Լ�
    public string GetLevelDescription(int level)
    {
        if (string.IsNullOrEmpty(descriptionTemplate)) return "";
        // ����: ���ط�, ��ٿ�, �ӵ� ���� ����Ͽ� descriptionTemplate�� ����
        float currentDamage = baseDamage + (damagePerLevel * (level - 1));
        float currentCooldown = Mathf.Max(0.1f, baseCooldown - (cooldownReductionPerLevel * (level - 1))); // �ּ� ��ٿ� ����
        return string.Format(descriptionTemplate, currentDamage, currentCooldown.ToString("F2"));
    }
}
