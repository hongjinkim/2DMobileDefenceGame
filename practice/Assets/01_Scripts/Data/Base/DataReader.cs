
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
        // �� �����ڿ��� ������ �ε�
        PlayerData = new PlayerData();
        InitialData = new InitialData();
        HeroData = new HeroData();

        DataLoadedEvent.RaiseEvent();
    }

}
