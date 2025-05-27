using UnityEngine;
using System.Collections.Generic;

// ������ ������ �� �� �ִ� ���� ���� (����)
public enum StatModifierType
{
    MoveSpeedPercentage,    // �̵� �ӵ� % ����
    AttackSpeedPercentage,  // ���� �ӵ� % ���� (��ٿ� ����)
    DamagePercentage,       // ���ݷ� % ����
    MaxHealthFlat,          // �ִ� ü�� ������ ����
    XPGainPercentage        // ����ġ ȹ�淮 % ����
}

[System.Serializable]
public struct StatModifier
{
    public StatModifierType type;
    public float value; // ������ ���� ��
}

[CreateAssetMenu(fileName = "New Artifact Data", menuName = "Divine Archer/Artifact Data")]
public class ArtifactData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string artifactId; // ���� ID (��: "SWIFT_BOOTS", "STORM_FEATHER")
    public string artifactName; // ���� �� ǥ�õ� �̸�
    [TextArea(3, 5)]
    public string descriptionTemplate; // ������ ������ ���� ���ø� (��: "�̵� �ӵ��� {0}% �����մϴ�.")
    public Sprite icon; // UI�� ǥ�õ� ������

    [Header("���� ����")]
    public int maxLevel = 5;

    [Header("ȿ�� ����")]
    public List<StatModifier> statModifiersPerLevel; // �� ������ ������ �����ϴ� ���� ���� ���

    // ������ ���� ������ �����ϴ� �Լ�
    public string GetLevelDescription(int level)
    {
        if (string.IsNullOrEmpty(descriptionTemplate)) return "";
        // ����: statModifiersPerLevel�� ù ��° �׸� ���� ����Ͽ� ���� ����
        if (statModifiersPerLevel.Count > 0)
        {
            return string.Format(descriptionTemplate, statModifiersPerLevel[0].value * level);
        }
        return "ȿ�� ���� ����";
    }
}
