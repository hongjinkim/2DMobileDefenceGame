#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class OdinInspectorDefineCleaner
{
    static OdinInspectorDefineCleaner()
    {
        // ������Ʈ�� Odin Inspector ������ �����ϴ��� üũ
        string[] possiblePaths = {
            "Assets/Sirenix/",
            "Assets/Plugins/Sirenix/"
        };

        bool odinExists = false;
        foreach (var path in possiblePaths)
        {
            if (Directory.Exists(path))
            {
                odinExists = true;
                break;
            }
        }

        // ODIN_INSPECTOR define ���� Ȯ��
        var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        if (!odinExists && defines.Contains("ODIN_INSPECTOR"))
        {
            // �ڵ����� define ����
            defines = string.Join(";", defines.Split(';').Where(d => d != "ODIN_INSPECTOR"));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
            Debug.Log("<color=orange>Odin Inspector�� ������Ʈ�� ��� ODIN_INSPECTOR define�� �ڵ� �����߽��ϴ�.</color>");
        }
    }
}
#endif