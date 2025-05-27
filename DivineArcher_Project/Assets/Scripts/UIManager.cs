using UnityEngine;
using UnityEngine.UI; // Unity UI 시스템 사용
using System.Collections.Generic; // List 사용
using TMPro; // TextMeshPro 사용 (선택 사항, 더 나은 텍스트 렌더링)

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("플레이어 상태 UI")]
    public Slider healthSlider; // 체력 바 (Slider 컴포넌트 사용)
    public TextMeshProUGUI healthText; // 체력 텍스트 (예: "100/100")
    public Slider xpSlider;     // 경험치 바
    public TextMeshProUGUI xpText;       // 경험치 텍스트 (예: "50/100")
    public TextMeshProUGUI levelText;    // 레벨 텍스트 (예: "Lv. 1")
    public TextMeshProUGUI timerText;    // 생존 시간 텍스트

    [Header("레벨업 UI")]
    public GameObject levelUpPanel; // 레벨업 선택 창 전체 패널
    public Button[] optionButtons;  // 선택지 버튼 배열 (3~4개)
    public TextMeshProUGUI[] optionButtonTexts; // 각 버튼의 텍스트
    public Image[] optionButtonIcons; // 각 버튼의 아이콘 (선택 사항)
    private List<UpgradeOption> currentUpgradeOptions; // 현재 제시된 업그레이드 선택지들

    [Header("게임 오버 UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalTimeSurvivedText;
    public Button restartButton;

    private PlayerController playerController;
    private float gameTimer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 플레이어 컨트롤러 참조 (더 좋은 방법은 GameManager를 통해 설정)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerController>();
        }

        // UI 초기화
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(GameManager.Instance.RestartGame); // GameManager에 RestartGame 함수 필요
        }

        // 버튼 리스너 설정
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int buttonIndex = i; // 클로저 문제 방지를 위해 로컬 변수 사용
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(buttonIndex));
        }
        UpdatePlayerStatsUI(); // 초기 UI 업데이트
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameOver && !GameManager.Instance.IsPaused)
        {
            gameTimer += Time.deltaTime;
            UpdateTimerUI();
        }
        // 플레이어 스탯 변경 시마다 UI 업데이트 (PlayerController에서 직접 호출하는 것이 더 효율적일 수 있음)
        // UpdatePlayerStatsUI();
    }

    public void UpdatePlayerStatsUI()
    {
        if (playerController == null) return;

        // 체력 UI
        if (healthSlider != null)
        {
            healthSlider.maxValue = playerController.maxHealth;
            healthSlider.value = playerController.currentHealth;
        }
        if (healthText != null)
        {
            healthText.text = $"{playerController.currentHealth} / {playerController.maxHealth}";
        }

        // 경험치 UI
        if (xpSlider != null)
        {
            xpSlider.maxValue = playerController.xpToNextLevel;
            xpSlider.value = playerController.experience;
        }
        if (xpText != null)
        {
            xpText.text = $"{Mathf.FloorToInt(playerController.experience)} / {Mathf.FloorToInt(playerController.xpToNextLevel)}";
        }

        // 레벨 UI
        if (levelText != null)
        {
            levelText.text = $"Lv. {playerController.level}";
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTimer / 60f);
            int seconds = Mathf.FloorToInt(gameTimer % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void ShowLevelUpPanel(List<UpgradeOption> options)
    {
        GameManager.Instance.PauseGame(true);
        currentUpgradeOptions = options;
        if (levelUpPanel == null)
        {
            Debug.LogError("LevelUpPanel이 UIManager에 할당되지 않았습니다!");
            GameManager.Instance.HideLevelUpOptions(); // 패널 없으면 바로 게임 재개
            return;
        }
        levelUpPanel.SetActive(true);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < options.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                // optionButtonTexts[i].text = options[i].description; // UpgradeOption에 description 필드 필요
                // if (optionButtonIcons[i] != null && options[i].icon != null) // UpgradeOption에 icon 필드 필요
                // {
                //     optionButtonIcons[i].sprite = options[i].icon;
                //     optionButtonIcons[i].gameObject.SetActive(true);
                // }
                // else if (optionButtonIcons[i] != null)
                // {
                //     optionButtonIcons[i].gameObject.SetActive(false);
                // }

                // 임시 텍스트 설정 (UpgradeOption 구조체 정의 후 수정)
                optionButtonTexts[i].text = $"선택지 {i + 1}: {options[i].optionName}";
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnOptionSelected(int optionIndex)
    {
        if (optionIndex < currentUpgradeOptions.Count)
        {
            currentUpgradeOptions[optionIndex].ApplyUpgrade(playerController); // UpgradeOption에 ApplyUpgrade 메소드 필요
            Debug.Log(currentUpgradeOptions[optionIndex].optionName + " 선택됨");
        }
        levelUpPanel.SetActive(false);
        GameManager.Instance.HideLevelUpOptions(); // GameManager를 통해 게임 재개
        UpdatePlayerStatsUI(); // 업그레이드 적용 후 UI 즉시 갱신
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel == null) return;
        gameOverPanel.SetActive(true);
        if (finalTimeSurvivedText != null)
        {
            finalTimeSurvivedText.text = "생존 시간: " + timerText.text;
        }
    }
}

// 레벨업 선택지를 위한 구조체 (별도 파일로 분리하거나 UIManager 내부에 둘 수 있음)
public struct UpgradeOption
{
    public string optionName; // UI에 표시될 이름 또는 설명
    public System.Action<PlayerController> ApplyUpgrade; // 선택 시 실행될 액션
    // public Sprite icon; // 아이콘 (선택 사항)
    // public string description; // 상세 설명 (선택 사항)

    public UpgradeOption(string name, System.Action<PlayerController> action)
    {
        optionName = name;
        ApplyUpgrade = action;
    }
}