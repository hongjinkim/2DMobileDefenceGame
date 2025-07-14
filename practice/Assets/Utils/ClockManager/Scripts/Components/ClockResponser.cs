using UnityEngine;


public class ClockResponser : MonoBehaviour, IClockResponser
{
    [SerializeField]
    private string DateTimeFormat = "o";

    private bool isInitialized = false;


    private void Awake()
    {
        var clock = FindObjectOfType<Clock>();

        if (clock != null)
        {
            isInitialized = true;
        }
        else
        {
            Debug.LogError($"[!] <{GetType()}> �� �ȿ� PoomClockManager�� �����ؾ� �մϴ�.");
        }
    }

    public string NowLocalString()
    {
        return isInitialized == true 
            ? Clock.NowLocal.ToString(DateTimeFormat)
            : string.Empty;
    }

    public string NowUTCString()
    {
        return isInitialized == true
            ? Clock.NowUTC.ToString(DateTimeFormat)
            : string.Empty;
    }

}

