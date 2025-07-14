using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    // ���ýð� �������� '��' ���� �ٲ� ������ �����
    public static event Action<DateTimeOffset> OnChangedSecondLocal;

    // ���ýð� �������� '��' ���� �ٲ� ������ �����
    public static event Action<DateTimeOffset> OnChangedMinuteLocal;

    // ���ýð� �������� '��' ���� �ٲ� ������ �����
    public static event Action<DateTimeOffset> OnChangedHourLocal;

    // ���ýð� �������� ��¥�� ����� ������ �����
    public static event Action<DateTimeOffset> OnChangedDayLocal;

    // UTC �������� ��¥�� ����� ������ �����
    public static event Action<DateTimeOffset> OnChangedDayUTC;

    // UTC ���� �� ������ ������
    public static event Action<DateTimeOffset> OnChangedFrameUTC;


    // ���� �ð����� ��ȯ
    public static DateTimeOffset NowLocal => NowUTC.ToLocalTime();

    // UTC �ð����� ��ȯ
    public static DateTimeOffset NowUTC => ServerUTC + FlowTimeSpan;



    // ------ �ð� �帧 ������ ------ //

    private static TimeSpan FlowTimeSpan => DateTimeOffset.UtcNow - TimeStamp;

    private static DateTimeOffset ServerUTC = DateTimeOffset.UtcNow;
    private static DateTimeOffset TimeStamp = DateTimeOffset.UtcNow;

    private DateTimeOffset nowLocal;
    private DateTimeOffset nowUTC;
    private DateTimeOffset prevLocal;
    private DateTimeOffset prevUTC;
    private bool isInitialized = false;


    private void Update()
    {
        if (isInitialized == false) return;

        nowLocal = NowLocal;
        nowUTC = NowUTC;

        OnChangedFrameUTC?.Invoke(nowUTC);

        if (nowUTC.Day != prevUTC.Day ||
            nowUTC.Month != prevUTC.Month ||
            nowUTC.Year != prevUTC.Year)
        {
            OnChangedDayUTC?.Invoke(nowUTC);
        }

        if (nowLocal.Day != prevLocal.Day ||
            nowLocal.Month != prevLocal.Month ||
            nowLocal.Year != prevLocal.Year)
        {
            OnChangedSecondLocal?.Invoke(nowLocal);
            OnChangedMinuteLocal?.Invoke(nowLocal);
            OnChangedHourLocal?.Invoke(nowLocal);
            OnChangedDayLocal?.Invoke(nowLocal);
        }
        else if (nowLocal.Hour != prevLocal.Hour)
        {
            OnChangedSecondLocal?.Invoke(nowLocal);
            OnChangedMinuteLocal?.Invoke(nowLocal);
            OnChangedHourLocal?.Invoke(nowLocal);
        }
        else if (nowLocal.Minute != prevLocal.Minute)
        {
            OnChangedSecondLocal?.Invoke(nowLocal);
            OnChangedMinuteLocal?.Invoke(nowLocal);
        }
        else if (nowLocal.Second != prevLocal.Second)
        {
            OnChangedSecondLocal?.Invoke(nowLocal);
        }

        prevLocal = nowLocal;
        prevUTC = nowUTC;
    }


    public void SetUTC(DateTimeOffset utc)
    {
        ServerUTC = utc;
        TimeStamp = DateTimeOffset.UtcNow;
        isInitialized = true;
    }

    public void Stop()
    {
        isInitialized = false;
    }

}
