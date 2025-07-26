#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#endif
using UGS;
using UnityEngine;


public class DataBase : SerializedMonoBehaviour
{
    public VoidEventChannelSO DataLoadedEvent;

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
        DataLoadedEvent.RaiseEvent();
    }

    private void LoadData()
    {
        // �� ������ Ŭ������ �����͸� �ε�
        playerData.LoadData();
        initialData.LoadData();
        heroData.LoadData();
        stageData.LoadData();
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
