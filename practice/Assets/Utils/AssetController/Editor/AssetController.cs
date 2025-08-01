#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[InitializeOnLoad]
public static class AssetController
{
    const string ODIN_INSPECTOR = "ODIN_INSPECTOR";
    const string ODIN_INSPECTOR_OFF = "ODIN_INSPECTOR_OFF";

    static AssetController()
    {
        // Unity�� ����(������Ʈ ����)�� �ڵ� �˻� �� ����ȭ
        CheckAndSetOdinDefine();
    }

    /// <summary>
    /// ������ �޴��� ������ ����ȭ ����
    /// </summary>
    [MenuItem("Tools/Odin/����ȭ: ���� ������ ���� Define �ڵ� ����")]
    public static void ManualCheckAndSetOdinDefine()
    {
        CheckAndSetOdinDefine();
    }

    /// <summary>
    /// ���� ���� üũ �� Scripting Define Symbols ����
    /// </summary>
    public static void CheckAndSetOdinDefine()
    {
        bool odinExists =
            Directory.Exists("Assets/Sirenix") ||
            Directory.Exists("Assets/Plugins/Sirenix");

        BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        // �����ݷ����� split, �ߺ�/���� ����, Odin ���� define�� ����
        var symbolList = new List<string>();
        foreach (var s in currentSymbols.Split(';'))
        {
            var trimmed = s.Trim();
            if (!string.IsNullOrEmpty(trimmed) &&
                trimmed != ODIN_INSPECTOR &&
                trimmed != ODIN_INSPECTOR_OFF)
            {
                symbolList.Add(trimmed);
            }
        }

        bool changed = false;

        if (odinExists)
        {
            // ���� ������ ON
            if (!symbolList.Contains(ODIN_INSPECTOR))
            {
                symbolList.Add(ODIN_INSPECTOR);
                changed = true;
                Debug.Log("<color=cyan>Odin Inspector/Serializer ������ ����: ODIN_INSPECTOR define Ȱ��ȭ</color>");
            }
        }
        else
        {
            // ���� ������ OFF
            if (!symbolList.Contains(ODIN_INSPECTOR_OFF))
            {
                symbolList.Add(ODIN_INSPECTOR_OFF);
                changed = true;
                Debug.Log("<color=magenta>Odin Inspector/Serializer ������ ����: ODIN_INSPECTOR_OFF define Ȱ��ȭ</color>");
            }
        }

        string newSymbols = string.Join(";", symbolList) + ";";
        if (changed && newSymbols != currentSymbols)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newSymbols);
            AssetDatabase.Refresh();
            Debug.Log($"<color=yellow>Odin Define ����ȭ �Ϸ�: {newSymbols}</color>");
        }
    }
}
#endif
