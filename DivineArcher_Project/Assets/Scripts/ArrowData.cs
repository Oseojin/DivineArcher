using UnityEngine;

// 화살의 발사 형태 등을 구분하기 위한 Enum (예시)
public enum ArrowType
{
    SingleProjectile, // 단일 투사체
    Orbiting,         // 주변 회전형
    MultiShot,        // 다발 발사형
    Beam              // 빔 형태 (추후 확장용)
}

[CreateAssetMenu(fileName = "New Arrow Data", menuName = "Divine Archer/Arrow Data")]
public class ArrowData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemId; // 고유 ID (예: "BASIC_ARROW", "BLADE_ORBIT_ARROW")
    public string itemName; // 게임 내 표시될 이름 (예: "기본 화살")
    [TextArea(3, 5)]
    public string descriptionTemplate; // 레벨별 설명을 위한 템플릿 (예: "전방으로 화살을 발사합니다. 피해량: {0}, 속도: {1}")
    public Sprite icon; // UI에 표시될 아이콘

    [Header("레벨 정보")]
    public int maxLevel = 5;

    [Header("공격 정보")]
    public ArrowType arrowType = ArrowType.SingleProjectile;
    public GameObject projectilePrefab; // 발사될 투사체 프리팹
    public float baseDamage = 10f;
    public float baseCooldown = 1.0f; // 기본 발사 간격 (초)
    public float baseProjectileSpeed = 10f;
    public int basePierceCount = 0; // 기본 관통 횟수 (0이면 관통 없음)

    // 레벨별 스탯 증가량 (예시, 더 복잡한 구조도 가능)
    [Header("레벨당 스탯 증가량")]
    public float damagePerLevel = 2f;
    public float cooldownReductionPerLevel = 0.05f; // 레벨당 쿨다운 감소량
    public float projectileSpeedPerLevel = 0.5f;
    public int pierceCountPerLevel = 0; // 레벨당 관통 증가 (필요시)

    // 특정 레벨에 도달했을 때 투사체 개수 증가 등의 특수 효과를 위한 배열 (선택 사항)
    // public int[] projectilesAtLevel = {1, 1, 2, 2, 3}; // 레벨 1~5일 때 발사되는 투사체 수

    // 레벨에 따른 설명을 생성하는 함수
    public string GetLevelDescription(int level)
    {
        if (string.IsNullOrEmpty(descriptionTemplate)) return "";
        // 예시: 피해량, 쿨다운, 속도 등을 계산하여 descriptionTemplate에 삽입
        float currentDamage = baseDamage + (damagePerLevel * (level - 1));
        float currentCooldown = Mathf.Max(0.1f, baseCooldown - (cooldownReductionPerLevel * (level - 1))); // 최소 쿨다운 보장
        return string.Format(descriptionTemplate, currentDamage, currentCooldown.ToString("F2"));
    }
}
