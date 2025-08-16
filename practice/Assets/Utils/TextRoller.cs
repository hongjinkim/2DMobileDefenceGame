using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class TextRoller
{
    public enum Mode
    {
        Original,       // ���� �״��: ���� ������ �� ���� ���ڿ��� ����
        Progressive,    // ���� �ڸ����� ���� ���� (���ϸ� ����)
        CountUp         // ���� ���� (�ɼ�: formatter �ʿ�)
    }

    private static readonly Dictionary<TMP_Text, Coroutine> running = new();

    public static void Roll(
        MonoBehaviour host,
        TMP_Text text,
        string finalText,
        Mode mode = Mode.Original,
        float duration = 0.3f,
        float minInterval = 0.02f,
        float maxInterval = 0.06f,
        System.Func<long, string> formatterForCountUp = null // CountUp��忡���� ���
    )
    {
        if (host == null || text == null) return;

        if (running.TryGetValue(text, out var prev))
            host.StopCoroutine(prev);

        Coroutine co = mode switch
        {
            Mode.Progressive => host.StartCoroutine(RollProgressive(text, finalText, duration, minInterval, maxInterval)),
            Mode.CountUp => host.StartCoroutine(RollCountUp(text, finalText, duration, formatterForCountUp)),
            _ => host.StartCoroutine(RollOriginal(text, finalText, duration, minInterval, maxInterval)),
        };

        running[text] = co;
    }

    // === ó�� ����: ���� ������ �� ���� ���� ===
    private static IEnumerator RollOriginal(TMP_Text text, string finalText, float duration, float minInterval, float maxInterval)
    {
        char[] finalChars = finalText.ToCharArray();
        var digitIdx = new List<int>();
        for (int i = 0; i < finalChars.Length; i++)
            if (char.IsDigit(finalChars[i])) digitIdx.Add(i);

        // ���ڰ� �ϳ��� ������ �ٷ� ǥ��
        if (digitIdx.Count == 0)
        {
            text.text = finalText;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float p = Mathf.Clamp01(elapsed / duration);
            var frame = new char[finalChars.Length];

            for (int i = 0; i < finalChars.Length; i++)
            {
                if (!char.IsDigit(finalChars[i]))
                {
                    frame[i] = finalChars[i];
                }
                else
                {
                    frame[i] = (char)('0' + Random.Range(0, 10)); // ���� ���� (ó�� ���� �״��)
                }
            }

            text.text = new string(frame);

            float interval = Mathf.Lerp(minInterval, maxInterval, p);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        text.text = finalText;
        running.Remove(text);
    }

    // === ����: ���ʺ��� ���� (���ϸ� �� ����) ===
    private static IEnumerator RollProgressive(TMP_Text text, string finalText, float duration, float minInterval, float maxInterval)
    {
        char[] finalChars = finalText.ToCharArray();
        var digitIdx = new List<int>();
        for (int i = 0; i < finalChars.Length; i++)
            if (char.IsDigit(finalChars[i])) digitIdx.Add(i);

        if (digitIdx.Count == 0)
        {
            text.text = finalText;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float p = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - p, 3f);
            int locked = Mathf.FloorToInt(eased * digitIdx.Count);

            var frame = new char[finalChars.Length];
            for (int i = 0; i < finalChars.Length; i++)
            {
                if (!char.IsDigit(finalChars[i]))
                {
                    frame[i] = finalChars[i];
                    continue;
                }
                int rank = digitIdx.IndexOf(i);
                frame[i] = (rank < locked) ? finalChars[i] : (char)('0' + Random.Range(0, 10));
            }

            text.text = new string(frame);

            float interval = Mathf.Lerp(minInterval, maxInterval, p);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        text.text = finalText;
        running.Remove(text);
    }

    // === ����: ���� ī��Ʈ�� (���ڿ� ���� �����Ϸ��� formatter ���� ����) ===
    private static IEnumerator RollCountUp(TMP_Text text, string finalText, float duration, System.Func<long, string> formatter)
    {
        if (!TryParseLongFromString(finalText, out var target))
        {
            // �Ľ� �Ұ��ϸ� �׳� ��� ǥ��
            text.text = finalText;
            running.Remove(text);
            yield break;
        }

        long start = TryParseLongFromString(text.text, out var cur) ? cur : 0;
        if (start == target)
        {
            text.text = formatter != null ? formatter(target) : finalText;
            running.Remove(text);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            float p = Mathf.Clamp01(t / duration);
            float eased = 1f - Mathf.Pow(1f - p, 3f);
            long val = LerpLong(start, target, eased);

            text.text = formatter != null ? formatter(val) : val.ToString(); // formatter ������ ���� ǥ��
            yield return null;
            t += Time.deltaTime;
        }

        text.text = formatter != null ? formatter(target) : finalText;
        running.Remove(text);
    }

    private static long LerpLong(long a, long b, float t)
    {
        double v = a + (b - a) * t;
        return (long)System.Math.Round(v);
    }

    private static bool TryParseLongFromString(string s, out long value)
    {
        value = 0;
        if (string.IsNullOrEmpty(s)) return false;
        System.Text.StringBuilder sb = new(s.Length);
        bool neg = false;
        foreach (var ch in s)
        {
            if (ch == '-' && sb.Length == 0) { neg = true; continue; }
            if (char.IsDigit(ch)) sb.Append(ch);
        }
        if (sb.Length == 0) return false;
        if (!long.TryParse(sb.ToString(), out var v)) return false;
        value = neg ? -v : v;
        return true;
    }
}
