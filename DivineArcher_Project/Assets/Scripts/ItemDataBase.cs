using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Linq ����� ���� �߰� (������)

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("������ ��� (�����Ϳ��� �Ҵ�)")]
    public List<ArrowData> allArrowDataList;
    public List<ArtifactData> allArtifactDataList;

    private Dictionary<string, ArrowData> arrowDataDict = new Dictionary<string, ArrowData>();
    private Dictionary<string, ArtifactData> artifactDataDict = new Dictionary<string, ArtifactData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDatabase()
    {
        // �����Ϳ��� �Ҵ�� ����Ʈ�� ����Ͽ� ��ųʸ� �ʱ�ȭ
        foreach (ArrowData data in allArrowDataList)
        {
            if (data != null && !string.IsNullOrEmpty(data.itemId) && !arrowDataDict.ContainsKey(data.itemId))
            {
                arrowDataDict.Add(data.itemId, data);
            }
            else if (data != null && !string.IsNullOrEmpty(data.itemId))
            {
                Debug.LogWarning($"�ߺ��� ȭ�� ID �Ǵ� ��ȿ���� ���� ID: {data.itemId} ({data.name})");
            }
        }

        foreach (ArtifactData data in allArtifactDataList)
        {
            if (data != null && !string.IsNullOrEmpty(data.artifactId) && !artifactDataDict.ContainsKey(data.artifactId))
            {
                artifactDataDict.Add(data.artifactId, data);
            }
            else if (data != null && !string.IsNullOrEmpty(data.artifactId))
            {
                Debug.LogWarning($"�ߺ��� ���� ID �Ǵ� ��ȿ���� ���� ID: {data.artifactId} ({data.name})");
            }
        }
        Debug.Log($"������ �����ͺ��̽� �ʱ�ȭ �Ϸ�: ȭ�� {arrowDataDict.Count}��, ���� {artifactDataDict.Count}�� �ε��.");
    }

    public ArrowData GetArrowData(string itemId)
    {
        arrowDataDict.TryGetValue(itemId, out ArrowData data);
        if (data == null)
        {
            Debug.LogWarning($"ID�� �ش��ϴ� ȭ�� �����͸� ã�� �� �����ϴ�: {itemId}");
        }
        return data;
    }

    public ArtifactData GetArtifactData(string artifactId)
    {
        artifactDataDict.TryGetValue(artifactId, out ArtifactData data);
        if (data == null)
        {
            Debug.LogWarning($"ID�� �ش��ϴ� ���� �����͸� ã�� �� �����ϴ�: {artifactId}");
        }
        return data;
    }

    // (���� ����) ������ �� ������ ���� ������ ���� ���� �� �߰� ����
}
