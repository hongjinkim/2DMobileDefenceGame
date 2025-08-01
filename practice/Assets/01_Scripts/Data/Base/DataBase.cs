
using Sirenix.OdinInspector;

using UGS;
using UnityEngine;


public class DataBase : MonoBehaviour
{
    public static DataBase Instance { get; private set; } = null;
    private bool isDataLoaded = false;



    // data Ŭ�������� ���⿡ ����

    [TabGroup("Tabs", "Player"), HideLabel][InlineProperty][SerializeField] private PlayerData playerData = new PlayerData();
    [TabGroup("Tabs", "Initial"), HideLabel][InlineProperty][SerializeField] private InitialData initialData = new InitialData();
    [TabGroup("Tabs", "Hero"), HideLabel][InlineProperty][SerializeField] private HeroData heroData = new HeroData();
    [TabGroup("Tabs", "Stage"), HideLabel][InlineProperty][SerializeField] private StageData stageData = new StageData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            UnityGoogleSheet.LoadAllData();
            Initialize();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // ���� �� ������ �ʱ�ȭ
    private void Initialize()
    {
        LoadData();
        isDataLoaded = true;
        EventManager.Raise(EEventType.DataLoaded);
    }

    private void LoadData()
    {
        // �� ������ Ŭ������ �����͸� �ε�
        playerData.LoadData();
        initialData.LoadData();
        heroData.LoadData();
        stageData.LoadData();
    }


    public static bool TryGetHeroValue(string id, out HeroValue value)
    {
        return Instance.heroData.HeroDict.TryGetValue(id, out value);
    }
    public static bool TryGetStageValue(int id, out StageValue value)
    {
        return Instance.stageData.StageDict.TryGetValue(id.ToString(), out value);
    }
}
