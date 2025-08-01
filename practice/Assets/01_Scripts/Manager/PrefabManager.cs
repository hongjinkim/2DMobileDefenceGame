using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// ���� ��巹���� ������ �Ŵ���
public class PrefabManager : MonoBehaviour
{
    // Address(���� string) �� ĳ�̵� ����
    private readonly Dictionary<string, UnityEngine.Object> prefabCache = new();

    /// <summary>
    /// Addressable ������ �񵿱�� �ε�(ĳ�� ����)
    /// </summary>
    public IEnumerator LoadPrefabAsync<T>(string address, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if (prefabCache.TryGetValue(address, out var cached) && cached is T cachedT)
        {
            onLoaded?.Invoke(cachedT);
            yield break;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            prefabCache[address] = handle.Result;
            onLoaded?.Invoke(handle.Result);
        }
        else
        {
            Debug.LogError($"PrefabManager: Failed to load prefab at address: {address}");
            onLoaded?.Invoke(null);
        }
    }

    /// <summary>
    /// ��� ��ȯ(�̹� �ε�� ��쿡��)
    /// </summary>
    public T GetCachedPrefab<T>(string address) where T : UnityEngine.Object
    {
        if (prefabCache.TryGetValue(address, out var cached) && cached is T cachedT)
            return cachedT;
        return null;
    }

    /// <summary>
    /// Address�� �ٷ� Instantiate (�񵿱�)
    /// </summary>
    public void InstantiateAsync(string address, Action<GameObject> onInstantiated)
    {
        Addressables.InstantiateAsync(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                onInstantiated?.Invoke(handle.Result);
            else
            {
                Debug.LogError($"PrefabManager: Instantiate ����: {address}");
                onInstantiated?.Invoke(null);
            }
        };
    }

    /// <summary>
    /// �̸� �ε�� �������� ���� Instantiate
    /// </summary>
    public GameObject InstantiateFromCache(string address, Vector3 pos, Quaternion rot)
    {
        var prefab = GetCachedPrefab<GameObject>(address);
        if (prefab == null)
        {
            Debug.LogError($"PrefabManager: ĳ�̵� �������� �����ϴ�: {address}");
            return null;
        }
        return Instantiate(prefab, pos, rot);
    }

    /// <summary>
    /// Address�� ĳ�� ���� (�޸� ��ε�)
    /// </summary>
    public void ReleasePrefab(string address)
    {
        if (prefabCache.TryGetValue(address, out var cached))
        {
            Addressables.Release(cached);
            prefabCache.Remove(address);
        }
    }

    /// <summary>
    /// ��ü ĳ�� ��ε�
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var kv in prefabCache)
            Addressables.Release(kv.Value);
        prefabCache.Clear();
    }
}
