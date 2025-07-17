
using UGS;
using UnityEngine;


public class DataReader : MonoBehaviour
{
    public static DataReader Instance { get; private set; } = null;
    public VoidEventChannelSO DataLoadedEvent;

    // data Ŭ�������� ���⿡ ����
    public PlayerData PlayerData;
    public InitialData InitialData;
    public HeroData HeroData;
    public StageData StageData;


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
        DataLoadedEvent.RaiseEvent();
    }

    private void LoadData()
    {
        // �� ������ Ŭ������ �����͸� �ε�
        //PlayerData.LoadData();
        InitialData = new InitialData();
        HeroData = new HeroData();
        StageData = new StageData();
    }

}
