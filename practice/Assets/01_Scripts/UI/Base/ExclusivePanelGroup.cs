using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ExclusivePanelGroup : MonoBehaviour
{
    [Serializable]
    public struct PanelEntry
    {
        public string Id;       // "Lobby", "Shop", "Settings" �� ���� ID
        public GameObject Panel;
    }

    [Header("�� ���� �ϳ��� Ȱ��ȭ�� �г� �׷�")]
    [SerializeField] private List<PanelEntry> panels = new List<PanelEntry>();

    [Tooltip("�� ���� �� �� �⺻ �г� (Id ����, ���� ��� ��Ȱ��ȭ)")]
    [SerializeField] private string defaultPanelId = "";

    private readonly Dictionary<string, GameObject> _map = new();
    public GameObject Current { get; private set; }

    void Awake()
    {
        BuildMap();
        if (!string.IsNullOrEmpty(defaultPanelId) && _map.TryGetValue(defaultPanelId, out var def))
        {
            ShowOnly(def);
        }
        else
        {
            // �⺻���� ������ ��� ��
            SetAllActive(false);
            Current = null;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // �ߺ�/���� ���� ���: null ����, �ߺ� ID ���
        var seen = new HashSet<string>();
        for (int i = panels.Count - 1; i >= 0; i--)
        {
            var p = panels[i];
            if (p.Panel == null)
            {
                panels.RemoveAt(i);
                continue;
            }
            if (!string.IsNullOrEmpty(p.Id))
            {
                if (!seen.Add(p.Id))
                {
                    Debug.LogWarning($"ExclusivePanelGroup: �ߺ� ID '{p.Id}'", this);
                }
            }
        }
    }
#endif

    private void BuildMap()
    {
        _map.Clear();
        foreach (var p in panels)
        {
            if (p.Panel == null) continue;
            if (!string.IsNullOrEmpty(p.Id))
            {
                _map[p.Id] = p.Panel; // ���� �׸��� ���� ���� (�ǵ���)
            }
        }
    }

    private void SetAllActive(bool active)
    {
        foreach (var p in panels)
        {
            if (p.Panel != null)
                p.Panel.SetActive(active);
        }
    }

    /// <summary>�ش� �гθ� On</summary>
    public void ShowOnly(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("ShowOnly: target�� null�Դϴ�.", this);
            return;
        }

        foreach (var p in panels)
        {
            if (p.Panel == null) continue;
            p.Panel.SetActive(p.Panel == target);
        }
        Current = target;
    }

    /// <summary>Index�� ��ȯ</summary>
    public void ShowOnly(int index)
    {
        if (index < 0 || index >= panels.Count)
        {
            Debug.LogError($"ShowOnly: index {index} ���� �ʰ�", this);
            return;
        }
        ShowOnly(panels[index].Panel);
    }

    /// <summary>Id�� ��ȯ</summary>
    public void ShowOnly(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("ShowOnly: id�� ����ֽ��ϴ�.", this);
            return;
        }
        if (!_map.TryGetValue(id, out var go) || go == null)
        {
            Debug.LogError($"ShowOnly: id '{id}'�� �ش��ϴ� �г��� �����ϴ�.", this);
            return;
        }
        ShowOnly(go);
    }

    /// <summary>��� ����</summary>
    public void HideAll()
    {
        SetAllActive(false);
        Current = null;
    }

    /// <summary>���� ���� �г��� Id �������� (������ �� ���ڿ�)</summary>
    public string CurrentId
    {
        get
        {
            if (Current == null) return "";
            foreach (var p in panels)
            {
                if (p.Panel == Current) return p.Id;
            }
            return "";
        }
    }
}
