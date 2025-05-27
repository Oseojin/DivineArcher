using UnityEngine;
using System.Collections.Generic;

// 유물이 영향을 줄 수 있는 스탯 종류 (예시)
public enum StatModifierType
{
    MoveSpeedPercentage,    // 이동 속도 % 증가
    AttackSpeedPercentage,  // 공격 속도 % 증가 (쿨다운 감소)
    DamagePercentage,       // 공격력 % 증가
    MaxHealthFlat,          // 최대 체력 고정값 증가
    XPGainPercentage        // 경험치 획득량 % 증가
}

[System.Serializable]
public struct StatModifier
{
    public StatModifierType type;
    public float value; // 레벨당 증가 값
}

[CreateAssetMenu(fileName = "New Artifact Data", menuName = "Divine Archer/Artifact Data")]
public class ArtifactData : ScriptableObject
{
    [Header("기본 정보")]
    public string artifactId; // 고유 ID (예: "SWIFT_BOOTS", "STORM_FEATHER")
    public string artifactName; // 게임 내 표시될 이름
    [TextArea(3, 5)]
    public string descriptionTemplate; // 레벨별 설명을 위한 템플릿 (예: "이동 속도가 {0}% 증가합니다.")
    public Sprite icon; // UI에 표시될 아이콘

    [Header("레벨 정보")]
    public int maxLevel = 5;

    [Header("효과 정보")]
    public List<StatModifier> statModifiersPerLevel; // 이 유물이 레벨당 제공하는 스탯 변경 목록

    // 레벨에 따른 설명을 생성하는 함수
    public string GetLevelDescription(int level)
    {
        if (string.IsNullOrEmpty(descriptionTemplate)) return "";
        // 예시: statModifiersPerLevel의 첫 번째 항목 값을 사용하여 설명 생성
        if (statModifiersPerLevel.Count > 0)
        {
            return string.Format(descriptionTemplate, statModifiersPerLevel[0].value * level);
        }
        return "효과 설명 없음";
    }
}
