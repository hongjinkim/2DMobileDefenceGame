using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableKeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;

    public SerializableKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
public abstract class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    // �ν����Ϳ� ����� ����Ʈ
    [SerializeField]
    private List<SerializableKeyValuePair<TKey, TValue>> keyValuePairs = new List<SerializableKeyValuePair<TKey, TValue>>();

    // ���� ���� �������� ����� ��ųʸ�
    private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

    // Public���� �����Ͽ� ���� ��ųʸ�ó�� ����� �� �ְ� ��
    public Dictionary<TKey, TValue> ToDictionary()
    {
        return _dictionary;
    }

    // Unity�� ����ȭ�� �����͸� �ҷ��� �� ȣ��
    public void OnAfterDeserialize()
    {
        _dictionary.Clear();
        foreach (var pair in keyValuePairs)
        {
            // Ű �ߺ� üũ
            if (pair.Key != null && !_dictionary.ContainsKey(pair.Key))
            {
                _dictionary.Add(pair.Key, pair.Value);
            }
        }
    }

    // Unity�� �����͸� ����ȭ�ϱ� �� ȣ�� (���⼭�� Ư���� �� ���� ����)
    public void OnBeforeSerialize()
    {
    }
}


// Dictionary ����ȭ�� Ŭ����
//[Serializable]
//public class StringIntDictionary : SerializableDictionary<string, int> { }

