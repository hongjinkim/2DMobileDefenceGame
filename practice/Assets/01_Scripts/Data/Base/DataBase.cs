
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UGS;
using UnityEngine;


public class DataBase : SerializedMonoBehaviour
{
    public VoidEventChannelSO DataLoadedEvent;

    public static DataBase Instance { get; private set; } = null;
    private bool isDataLoaded = false;



    // data Ŭ�������� ���⿡ ����

    [TabGroup("Tabs", "Player"), HideLabel][InlineProperty][SerializeField] private PlayerData playerData;
    [TabGroup("Tabs", "Initial"), HideLabel][InlineProperty][SerializeField] private InitialData initialData;
    [TabGroup("Tabs", "Hero"), HideLabel][InlineProperty][SerializeField] private HeroData heroData;
    [TabGroup("Tabs", "Stage"), HideLabel][InlineProperty][SerializeField] private StageData stageData;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        UnityGoogleSheet.LoadAllData();
        Initialize();
    }

    // ���� �� ������ �ʱ�ȭ
    private void Initialize()
    {
        LoadData();
        isDataLoaded = true;
        DataLoadedEvent.RaiseEvent();
    }

    private void LoadData()
    {
        // �� ������ Ŭ������ �����͸� �ε�
        playerData = new PlayerData();
        initialData = new InitialData();
        heroData = new HeroData();
        stageData = new StageData();
    }


    public static PlayerData GetPlayerData()
    {
        if(Instance == null || Instance.playerData == null || !Instance.isDataLoaded)
        {
            Debug.LogError("DataReader instance or playerData is not initialized.");
            return null;
        }
        return Instance.playerData;
    }

    public static InitialData GetInitialData()
    {
        if (Instance == null || Instance.initialData == null || !Instance.isDataLoaded)
        {
            Debug.LogError("DataReader instance or initialData is not initialized.");
            return null;
        }
        return Instance.initialData;
    }
}
