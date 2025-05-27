using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �߰�

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public UIManager uiManagerInstance; // Inspector���� UIManager ������Ʈ �Ҵ�
    public PlayerController playerController;

    // ���� ���� ���� ���� (����)
    public bool IsGameOver { get; private set; } = false;
    public bool IsPaused { get; private set; } = false; // ������ â ��� ���

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� ����Ǿ GameManager�� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �ִٸ� �ߺ� ���� ����
        }
    }

    void Start()
    {
        // �ʱ�ȭ ����
        IsGameOver = false;
        Time.timeScale = 1f; // ���� ���� �� ���� �ӵ�
        if (uiManagerInstance == null)
        {
            uiManagerInstance = FindFirstObjectByType<UIManager>(); // ������ UIManager�� ã�� �Ҵ�
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
        Debug.Log("���� ����!");
        if (uiManagerInstance != null) uiManagerInstance.ShowGameOverPanel(); // ���� ���� UI ǥ��
    }

    public void ReturnToMainMenu()
    {
        // SceneManager.LoadScene("MainMenuScene"); // ���� �޴� �� �̸����� ����
    }

    // ������ UI ǥ�� �Լ� (PlayerController���� ȣ��� ����)
    public void ShowLevelUpOptions() // PlayerController���� ȣ��
    {
        // ���� ������ ���� ������ PlayerController �Ǵ� UpgradeManager ��� ó�� �� UIManager�� ����
        List<UpgradeOption> options = GenerateUpgradeOptions(); // �� �Լ��� �Ʒ��� ���÷� �߰�
        if (uiManagerInstance != null) uiManagerInstance.ShowLevelUpPanel(options);
        else PauseGame(true); // UIManager ������ �ϴ� ���Ӹ� ����
    }

    // �ӽ� ������ ���� �Լ� (�����δ� PlayerController�� ���� �Ŵ������� ����)
    private List<UpgradeOption> GenerateUpgradeOptions()
    {
        List<UpgradeOption> generatedOptions = new List<UpgradeOption>();
        // ����: ���� ���� ȭ�� ��ȭ �Ǵ� �� ���� ȹ��
        // �����δ� ItemDatabase�� PlayerController�� ���� ���¸� ������� �����ؾ� ��

        // ���� 1: �⺻ ȭ�� ��ȭ (PlayerController�� �ش� ȭ���� �ִ��� Ȯ�� �ʿ�)
        string basicArrowId = "BASIC_ARROW"; // PlayerController�� initialArrowId�� �����ؾ� ��
        AcquiredItem basicArrow = playerController.acquiredArrows.Find(a => a.itemId == basicArrowId);
        if (basicArrow != null && ItemDatabase.Instance.GetArrowData(basicArrowId) != null && basicArrow.currentLevel < ItemDatabase.Instance.GetArrowData(basicArrowId).maxLevel)
        {
            generatedOptions.Add(new UpgradeOption(
                ItemDatabase.Instance.GetArrowData(basicArrowId).itemName + " ��ȭ (Lv." + (basicArrow.currentLevel + 1) + ")",
                (pc) => {
                    AcquiredItem itemToUpgrade = pc.acquiredArrows.Find(a => a.itemId == basicArrowId);
                    if (itemToUpgrade != null) itemToUpgrade.currentLevel++;
                }
            ));
        }

        // ���� 2: �� ���� ȹ�� (�÷��̾ ���� ������ ���� ���� �� ���� ����)
        // string newArtifactId = "SWIFT_BOOTS"; // ���� ���� ID
        // if (!playerController.acquiredArtifacts.Exists(a => a.itemId == newArtifactId) && ItemDatabase.Instance.GetArtifactData(newArtifactId) != null)
        // {
        //     generatedOptions.Add(new UpgradeOption(
        //         ItemDatabase.Instance.GetArtifactData(newArtifactId).artifactName + " ȹ��",
        //         (pc) => pc.acquiredArtifacts.Add(new AcquiredItem(newArtifactId, 1))
        //     ));
        // }

        // �������� �����ϸ� �⺻ �ɼ� �߰� (��: ü�� ȸ��)
        if (generatedOptions.Count < 3)
        {
            generatedOptions.Add(new UpgradeOption("�ִ� ü���� 20% ȸ��", (pc) => pc.Heal(Mathf.FloorToInt(pc.maxHealth * 0.2f))));
        }
        // �����δ� 3������ �پ��� �ɼ��� �����ϵ��� ���� ���� �ʿ�
        return generatedOptions.Take(3).ToList(); // �ִ� 3���� ��ȯ
    }


    // ������ ���� �Ϸ� �� ȣ��� �Լ� (UI���� ȣ��)
    public void HideLevelUpOptions()
    {
        PauseGame(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // �ð� �帧 ����ȭ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���� �� �ٽ� �ε�
    }
}