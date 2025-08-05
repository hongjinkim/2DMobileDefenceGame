#if UNITY_EDITOR
using UnityEditor;

public static class DefineConfig
{
    // �÷������� �ʿ��� Define ���
    private static readonly string[] REQUIRED_DEFINES = new[]
    {
        "ODIN_INSPECTOR",
        "DEBUG_ON"
    };

    /// <summary>
    /// Ư�� ���� Ÿ�� �׷쿡 define�� �����մϴ�.
    /// </summary>
    public static void ApplyDefines(BuildTargetGroup group)
    {
        string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        var defineList = new System.Collections.Generic.HashSet<string>(
            currentDefines.Split(';'),
            System.StringComparer.OrdinalIgnoreCase
        );

        bool changed = false;

        foreach (var def in REQUIRED_DEFINES)
        {
            if (!defineList.Contains(def))
            {
                defineList.Add(def);
                changed = true;
            }
        }

        if (changed)
        {
            string updatedDefines = string.Join(";", defineList);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, updatedDefines);
            UnityEngine.Debug.Log($"[DefineConfig] Updated defines for {group}: {updatedDefines}");
        }
    }

    /// <summary>
    /// ���� ��� ���� ��� �ֿ� Ÿ�� �׷쿡 define ����
    /// </summary>
    [MenuItem("Tools/Apply Define Symbols")]
    public static void ApplyToAllTargets()
    {
        ApplyDefines(BuildTargetGroup.Standalone); // Windows, Mac, Linux
        ApplyDefines(BuildTargetGroup.Android);
        ApplyDefines(BuildTargetGroup.iOS);
        // �ʿ��� Ÿ�� �߰� ����
    }
}
#endif
