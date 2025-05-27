using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public UIManager uiManagerInstance; // Inspector에서 UIManager 오브젝트 할당
    public PlayerController playerController;

    // 게임 상태 관련 변수 (예시)
    public bool IsGameOver { get; private set; } = false;
    public bool IsPaused { get; private set; } = false; // 레벨업 창 등에서 사용

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 GameManager는 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있다면 중복 생성 방지
        }
    }

    void Start()
    {
        // 초기화 로직
        IsGameOver = false;
        Time.timeScale = 1f; // 게임 시작 시 정상 속도
        if (uiManagerInstance == null)
        {
            uiManagerInstance = FindFirstObjectByType<UIManager>(); // 씬에서 UIManager를 찾아 할당
        }
    }

    public void PauseGame(bool pause)
    {
        IsPaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f;
        Debug.Log("게임 오버!");
        if (uiManagerInstance != null) uiManagerInstance.ShowGameOverPanel(); // 게임 오버 UI 표시
    }

    public void ReturnToMainMenu()
    {
        // SceneManager.LoadScene("MainMenuScene"); // 메인 메뉴 씬 이름으로 변경
    }

    // 레벨업 UI 표시 함수 (PlayerController에서 호출될 예정)
    public void ShowLevelUpOptions() // PlayerController에서 호출
    {
        // 실제 선택지 생성 로직은 PlayerController 또는 UpgradeManager 등에서 처리 후 UIManager로 전달
        List<UpgradeOption> options = GenerateUpgradeOptions(); // 이 함수는 아래에 예시로 추가
        if (uiManagerInstance != null) uiManagerInstance.ShowLevelUpPanel(options);
        else PauseGame(true); // UIManager 없으면 일단 게임만 멈춤
    }

    // 임시 선택지 생성 함수 (실제로는 PlayerController나 별도 매니저에서 관리)
    private List<UpgradeOption> GenerateUpgradeOptions()
    {
        List<UpgradeOption> generatedOptions = new List<UpgradeOption>();
        // 예시: 현재 보유 화살 강화 또는 새 유물 획득
        // 실제로는 ItemDatabase와 PlayerController의 현재 상태를 기반으로 생성해야 함

        // 예시 1: 기본 화살 강화 (PlayerController에 해당 화살이 있는지 확인 필요)
        string basicArrowId = "BASIC_ARROW"; // PlayerController의 initialArrowId와 동일해야 함
        AcquiredItem basicArrow = playerController.acquiredArrows.Find(a => a.itemId == basicArrowId);
        if (basicArrow != null && ItemDatabase.Instance.GetArrowData(basicArrowId) != null && basicArrow.currentLevel < ItemDatabase.Instance.GetArrowData(basicArrowId).maxLevel)
        {
            generatedOptions.Add(new UpgradeOption(
                ItemDatabase.Instance.GetArrowData(basicArrowId).itemName + " 강화 (Lv." + (basicArrow.currentLevel + 1) + ")",
                (pc) => {
                    AcquiredItem itemToUpgrade = pc.acquiredArrows.Find(a => a.itemId == basicArrowId);
                    if (itemToUpgrade != null) itemToUpgrade.currentLevel++;
                }
            ));
        }

        // 예시 2: 새 유물 획득 (플레이어가 아직 가지지 않은 유물 중 랜덤 선택)
        // string newArtifactId = "SWIFT_BOOTS"; // 예시 유물 ID
        // if (!playerController.acquiredArtifacts.Exists(a => a.itemId == newArtifactId) && ItemDatabase.Instance.GetArtifactData(newArtifactId) != null)
        // {
        //     generatedOptions.Add(new UpgradeOption(
        //         ItemDatabase.Instance.GetArtifactData(newArtifactId).artifactName + " 획득",
        //         (pc) => pc.acquiredArtifacts.Add(new AcquiredItem(newArtifactId, 1))
        //     ));
        // }

        // 선택지가 부족하면 기본 옵션 추가 (예: 체력 회복)
        if (generatedOptions.Count < 3)
        {
            generatedOptions.Add(new UpgradeOption("최대 체력의 20% 회복", (pc) => pc.Heal(Mathf.FloorToInt(pc.maxHealth * 0.2f))));
        }
        // 실제로는 3개까지 다양한 옵션을 제공하도록 로직 구성 필요
        return generatedOptions.Take(3).ToList(); // 최대 3개만 반환
    }


    // 레벨업 선택 완료 후 호출될 함수 (UI에서 호출)
    public void HideLevelUpOptions()
    {
        PauseGame(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // 시간 흐름 정상화
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 다시 로드
    }
}