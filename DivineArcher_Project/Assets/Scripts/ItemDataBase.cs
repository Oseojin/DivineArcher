using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Linq 사용을 위해 추가 (선택적)

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("데이터 목록 (에디터에서 할당)")]
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
        // 에디터에서 할당된 리스트를 사용하여 딕셔너리 초기화
        foreach (ArrowData data in allArrowDataList)
        {
            if (data != null && !string.IsNullOrEmpty(data.itemId) && !arrowDataDict.ContainsKey(data.itemId))
            {
                arrowDataDict.Add(data.itemId, data);
            }
            else if (data != null && !string.IsNullOrEmpty(data.itemId))
            {
                Debug.LogWarning($"중복된 화살 ID 또는 유효하지 않은 ID: {data.itemId} ({data.name})");
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
                Debug.LogWarning($"중복된 유물 ID 또는 유효하지 않은 ID: {data.artifactId} ({data.name})");
            }
        }
        Debug.Log($"아이템 데이터베이스 초기화 완료: 화살 {arrowDataDict.Count}개, 유물 {artifactDataDict.Count}개 로드됨.");
    }

    public ArrowData GetArrowData(string itemId)
    {
        arrowDataDict.TryGetValue(itemId, out ArrowData data);
        if (data == null)
        {
            Debug.LogWarning($"ID에 해당하는 화살 데이터를 찾을 수 없습니다: {itemId}");
        }
        return data;
    }

    public ArtifactData GetArtifactData(string artifactId)
    {
        artifactDataDict.TryGetValue(artifactId, out ArtifactData data);
        if (data == null)
        {
            Debug.LogWarning($"ID에 해당하는 유물 데이터를 찾을 수 없습니다: {artifactId}");
        }
        return data;
    }

    // (선택 사항) 레벨업 시 보여줄 랜덤 아이템 선택 로직 등 추가 가능
}
