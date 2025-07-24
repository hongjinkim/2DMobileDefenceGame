#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEditor;

public static class OdinStubGenerator
{
    [MenuItem("Tools/Generate Odin Attribute Stubs")]
    public static void Generate()
    {
        // Odin Inspector Assembly ���ӽ����̽�
        const string odinNamespace = "Sirenix.OdinInspector";
        var sb = new StringBuilder();
        sb.AppendLine("#if !ODIN_INSPECTOR");
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("namespace Sirenix.OdinInspector");
        sb.AppendLine("{");

        // Odin Inspector ����� ã��
        var odinAsm = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name.StartsWith("Sirenix.OdinInspector"));

        if (odinAsm == null)
        {
            EditorUtility.DisplayDialog("Odin Not Found", "Odin Inspector Assembly�� ã�� �� �����ϴ�.", "OK");
            return;
        }

        // Attribute Ÿ�Ը� �̱�
        var attrs = odinAsm.GetTypes()
            .Where(t => t.IsClass && t.IsPublic && t.Namespace == odinNamespace && typeof(Attribute).IsAssignableFrom(t))
            .OrderBy(t => t.Name);

        foreach (var attr in attrs)
        {
            sb.AppendLine($"    [AttributeUsage(AttributeTargets.All)]");
            sb.AppendLine($"    public class {attr.Name} : Attribute");
            sb.AppendLine("    {");
            // �����ε�� ������ ����
            foreach (var ctor in attr.GetConstructors())
            {
                var paramList = string.Join(", ", ctor.GetParameters()
                    .Select(p => $"{p.ParameterType.Name} {p.Name}"));
                sb.AppendLine($"        public {attr.Name}({paramList}) {{ }}");
            }
            // �⺻ �����ڰ� ���� ���� �� ������ �ϳ� �߰�
            if (!attr.GetConstructors().Any(c => c.GetParameters().Length == 0))
                sb.AppendLine($"        public {attr.Name}() {{ }}");

            sb.AppendLine("    }");
        }

        sb.AppendLine("}");
        sb.AppendLine("#endif");

        // ���� ���
        var outputPath = "Assets/ThirdPartyStub/OdinInspector/Stubs/Scripts/OdinInspectorStubs.cs";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("�Ϸ�!", $"Odin Inspector Stub ���� �Ϸ�!\n{outputPath}", "OK");
    }
}
#endif
