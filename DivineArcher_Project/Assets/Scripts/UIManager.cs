using UnityEngine;
using UnityEngine.UI; // Unity UI �ý��� ���
using System.Collections.Generic; // List ���
using TMPro; // TextMeshPro ��� (���� ����, �� ���� �ؽ�Ʈ ������)

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("�÷��̾� ���� UI")]
    public Slider healthSlider; // ü�� �� (Slider ������Ʈ ���)
    public TextMeshProUGUI healthText; // ü�� �ؽ�Ʈ (��: "100/100")
    public Slider xpSlider;     // ����ġ ��
    public TextMeshProUGUI xpText;       // ����ġ �ؽ�Ʈ (��: "50/100")
    public TextMeshProUGUI levelText;    // ���� �ؽ�Ʈ (��: "Lv. 1")
    public TextMeshProUGUI timerText;    // ���� �ð� �ؽ�Ʈ

    [Header("������ UI")]
    public GameObject levelUpPanel; // ������ ���� â ��ü �г�
    public Button[] optionButtons;  // ������ ��ư �迭 (3~4��)
    public TextMeshProUGUI[] optionButtonTexts; // �� ��ư�� �ؽ�Ʈ
    public Image[] optionButtonIcons; // �� ��ư�� ������ (���� ����)
    private List<UpgradeOption> currentUpgradeOptions; // ���� ���õ� ���׷��̵� ��������

    [Header("���� ���� UI")]
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
        // �÷��̾� ��Ʈ�ѷ� ���� (�� ���� ����� GameManager�� ���� ����)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerController>();
        }

        // UI �ʱ�ȭ
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(GameManager.Instance.RestartGame); // GameManager�� RestartGame �Լ� �ʿ�
        }

        // ��ư ������ ����
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int buttonIndex = i; // Ŭ���� ���� ������ ���� ���� ���� ���
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(buttonIndex));
        }
        UpdatePlayerStatsUI(); // �ʱ� UI ������Ʈ
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameOver && !GameManager.Instance.IsPaused)
        {
            gameTimer += Time.deltaTime;
            UpdateTimerUI();
        }
        // �÷��̾� ���� ���� �ø��� UI ������Ʈ (PlayerController���� ���� ȣ���ϴ� ���� �� ȿ������ �� ����)
        // UpdatePlayerStatsUI();
    }

    public void UpdatePlayerStatsUI()
    {
        if (playerController == null) return;

        // ü�� UI
        if (healthSlider != null)
        {
            healthSlider.maxValue = playerController.maxHealth;
            healthSlider.value = playerController.currentHealth;
        }
        if (healthText != null)
        {
            healthText.text = $"{playerController.currentHealth} / {playerController.maxHealth}";
        }

        // ����ġ UI
        if (xpSlider != null)
        {
            xpSlider.maxValue = playerController.xpToNextLevel;
            xpSlider.value = playerController.experience;
        }
        if (xpText != null)
        {
            xpText.text = $"{Mathf.FloorToInt(playerController.experience)} / {Mathf.FloorToInt(playerController.xpToNextLevel)}";
        }

        // ���� UI
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
            Debug.LogError("LevelUpPanel�� UIManager�� �Ҵ���� �ʾҽ��ϴ�!");
            GameManager.Instance.HideLevelUpOptions(); // �г� ������ �ٷ� ���� �簳
            return;
        }
        levelUpPanel.SetActive(true);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < options.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                // optionButtonTexts[i].text = options[i].description; // UpgradeOption�� description �ʵ� �ʿ�
                // if (optionButtonIcons[i] != null && options[i].icon != null) // UpgradeOption�� icon �ʵ� �ʿ�
                // {
                //     optionButtonIcons[i].sprite = options[i].icon;
                //     optionButtonIcons[i].gameObject.SetActive(true);
                // }
                // else if (optionButtonIcons[i] != null)
                // {
                //     optionButtonIcons[i].gameObject.SetActive(false);
                // }

                // �ӽ� �ؽ�Ʈ ���� (UpgradeOption ����ü ���� �� ����)
                optionButtonTexts[i].text = $"������ {i + 1}: {options[i].optionName}";
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
            currentUpgradeOptions[optionIndex].ApplyUpgrade(playerController); // UpgradeOption�� ApplyUpgrade �޼ҵ� �ʿ�
            Debug.Log(currentUpgradeOptions[optionIndex].optionName + " ���õ�");
        }
        levelUpPanel.SetActive(false);
        GameManager.Instance.HideLevelUpOptions(); // GameManager�� ���� ���� �簳
        UpdatePlayerStatsUI(); // ���׷��̵� ���� �� UI ��� ����
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel == null) return;
        gameOverPanel.SetActive(true);
        if (finalTimeSurvivedText != null)
        {
            finalTimeSurvivedText.text = "���� �ð�: " + timerText.text;
        }
    }
}

// ������ �������� ���� ����ü (���� ���Ϸ� �и��ϰų� UIManager ���ο� �� �� ����)
public struct UpgradeOption
{
    public string optionName; // UI�� ǥ�õ� �̸� �Ǵ� ����
    public System.Action<PlayerController> ApplyUpgrade; // ���� �� ����� �׼�
    // public Sprite icon; // ������ (���� ����)
    // public string description; // �� ���� (���� ����)

    public UpgradeOption(string name, System.Action<PlayerController> action)
    {
        optionName = name;
        ApplyUpgrade = action;
    }
}