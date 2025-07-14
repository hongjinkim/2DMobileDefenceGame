using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public static class Extension
{
    public static string ToABC(this BigNum bigNum) => bigNum.ToRegularFormat();
    public static string ToABC(this double bigNum) => new BigNum(bigNum).ToABC();
    public static string ToA_BC(this double bigNum) => new BigNum(bigNum).ToA_BC();

    public static string ToBCD(this BigNum bigNum) => bigNum.ToWideFormat();
    public static string ToBCD(this double bigNum) => new BigNum(bigNum).ToBCD();
    public static string ToBCD(this float bigNum) => new BigNum(bigNum).ToBCD();
    public static string ToBCD(this int bigNum) => new BigNum(bigNum).ToBCD();
    public static string ToBCD(this long bigNum) => new BigNum(bigNum).ToBCD();

    //�Ҽ��� ���ϵ� ǥ���ϵ��� �ϴ� �޼��� (float��ȯ Ȱ��, �ִ� 2�ڸ����� ǥ��, �Ҽ��� �ǵ��ڸ� 0�� ����)
    public static string ToA_BC(this BigNum bigNum)
    {
        if (bigNum >= 100 || bigNum < -100)
            return bigNum.ToRegularFormat();

        else if (bigNum >= 10 || bigNum <= -10)
            return String.Format("{0:0.#}", (float)bigNum);

        else
            return String.Format("{0:0.##}", (float)bigNum);
    }

    // ǥ�� Extension Method
    //public static string ToABC(this PoomNum poomNum) => IsKorean ? poomNum.ToKorean() : poomNum.ToRegularFormat();

    public static string ToRound(this float Num) => (Mathf.Round(Num * 100) / 100f).ToString();

    public static string ToRound(this double Num) => (Mathf.Round((float)Num * 100) / 100f).ToString();
    public static void InitializeArray<T>(this T[] array) where T : class, new()
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new T();
        }
    }

    public static T[] InitializeArray<T>(int length) where T : class, new()
    {
        T[] array = new T[length];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new T();
        }

        return array;
    }



    // string�� enum ��Ͽ��� ã�� �ش� enum������ index�� ��ȯ
    public static int ConvertStringToEnum(string str, Array arr)
    {
        foreach (var e in arr)
        {
            if (e.ToString() == str)
                return (int)e;
        }

        return -1;
    }

    public static int FloatToIntByRatio(this float value)
    {
        int result = (int)value;
        if (UnityEngine.Random.value < value - result) { return result + 1; }
        else { return result; }
    }


    //�Ϲݽð� - TimeScale�� ����o : Keyword�� delay�� float ->  WaitForSeconds�� Pooling
    private static readonly Dictionary<float, WaitForSeconds> t_Pool = new Dictionary<float, WaitForSeconds>(new FloatComparer());

    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y) { return x == y; }
        int IEqualityComparer<float>.GetHashCode(float obj) { return obj.GetHashCode(); }
    }

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();


    //�Ϲݽð� �ܺ�ȣ�� �Լ�
    public static void Invoke(this MonoBehaviour mb, Action f, float delay) { mb.StartCoroutine(InvokeRoutine(f, delay)); }

    private static IEnumerator InvokeRoutine(Action function, float delay)
    {
        WaitForSeconds wfs;
        if (!t_Pool.TryGetValue(delay, out wfs)) { t_Pool.Add(delay, wfs = new WaitForSeconds(delay)); }
        yield return wfs; //time���
        function(); //������ �޼ҵ� ����
    }
    public static WaitForSeconds Yield(this MonoBehaviour mb, float tick)
    {
        if (!t_Pool.TryGetValue(tick, out WaitForSeconds result)) { t_Pool.Add(tick, new WaitForSeconds(tick)); }

        return result;
    }

    // �ߺ����� ��������Ʈ �޼���
    public static List<int> List_Pick_noDup(int Start_Num, int End_Num, int Pick_Num)
    {
        if (Start_Num >= End_Num) { Debug.LogWarning("������������ ���� ���� �Է� �ʿ�"); return null; }

        List<int> target_list = new List<int>();
        List<int> picked_list = new List<int>();

        int picked_Index;

        for (int i = Start_Num; i <= End_Num; i++) { target_list.Add(i); } //Ÿ�ٸ���Ʈ int������ ����

        for (int i = 0; i < Pick_Num; i++) //�����ϰ� ����
        {
            picked_Index = (int)(target_list.Count * UnityEngine.Random.value);
            picked_list.Add(target_list[picked_Index]);
            target_list.RemoveAt(picked_Index);
        }

        return picked_list;
    }

    //float �˰������ؼ� int�� ��ȯ�ϴ� �޼���
    public static int floatToInt(this float f)
    {
        return (int)(f + 0.0001f);
    }


    /* *** �Ʒ��� ������ ���ӿ��� ������ �ҷ��� �� �ְ� ����© �� *** */

    // ������� ����
    // ���� : private static SystemLanguage currentLanguage => Blackboard.Settings.CurrentLanguage;
    private static SystemLanguage CurrentLanguage => SystemLanguage.Korean;     // �ӽ�

    // �ѱ� ��� ���� �Ǻ�
    // SystemLanguage ������� �ʴ´ٸ� ���⸦ �ٲ����
    private static bool IsKorean => CurrentLanguage == SystemLanguage.Korean;   // �ӽ�

}

