#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEditor;
using System.Collections.Generic;

public static class OdinStubGenerator
{
    [MenuItem("Tools/Generate Odin Attribute Stubs")]
    public static void Generate()
    {
        const string odinNamespace = "Sirenix.OdinInspector";
        var sb = new StringBuilder();
        var enumSet = new HashSet<Type>();
        var processedTypes = new HashSet<string>();

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

        // ��� Attribute Ÿ�� + ������ �Ķ���� Ÿ�Կ��� enum, class, struct ����
        var attrTypes = odinAsm.GetTypes()
            .Where(t => t.IsClass && t.IsPublic && t.Namespace == odinNamespace && typeof(Attribute).IsAssignableFrom(t))
            .OrderBy(t => t.Name);

        foreach (var attr in attrTypes)
        {
            if (!processedTypes.Add(attr.FullName))
                continue;

            // Attribute ����
            sb.AppendLine($"    [AttributeUsage(AttributeTargets.All)]");
            sb.AppendLine($"    public class {attr.Name} : Attribute");
            sb.AppendLine("    {");

            // ������(�����ε�) ����
            var ctors = attr.GetConstructors();
            bool hasParameterless = false;
            foreach (var ctor in ctors)
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length == 0) hasParameterless = true;
                var paramList = string.Join(", ", parameters
                    .Select(p => $"{GetTypeKeyword(p.ParameterType)} {p.Name}"));
                sb.AppendLine($"        public {attr.Name}({paramList}) {{ }}");

                // �Ķ���� Ÿ�Կ� Enum/class�� ���
                foreach (var p in parameters)
                {
                    var type = p.ParameterType;
                    if (type.Namespace == odinNamespace)
                    {
                        if (type.IsEnum || (type.IsClass && type != attr && type.IsPublic) || (type.IsValueType && !type.IsPrimitive && !type.IsEnum))
                        {
                            enumSet.Add(type);
                        }
                    }
                }
            }
            if (!hasParameterless)
                sb.AppendLine($"        public {attr.Name}() {{ }}");
            sb.AppendLine("    }");
        }

        // Enum/class stub
        foreach (var type in enumSet)
        {
            if (!processedTypes.Add(type.FullName))
                continue;

            if (type.IsEnum)
            {
                sb.AppendLine($"    public enum {type.Name}");
                sb.AppendLine("    {");
                var values = Enum.GetNames(type);
                foreach (var v in values)
                {
                    sb.AppendLine($"        {v},");
                }
                sb.AppendLine("    }");
            }
            else if (type.IsClass)
            {
                sb.AppendLine($"    public class {type.Name} {{ }}");
            }
            else if (type.IsValueType && !type.IsPrimitive)
            {
                sb.AppendLine($"    public struct {type.Name} {{ }}");
            }
        }

        sb.AppendLine("}");
        sb.AppendLine("#endif");

        // ���� ���
        var outputPath = "Assets/ThirdPartyStub/Sirenix/OdinInspector/OdinInspectorStubs.cs";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("�Ϸ�!", $"Odin Inspector Stub ���� �Ϸ�!\n{outputPath}", "OK");
    }

    // �⺻�� �̸� ������
    static string GetTypeKeyword(Type t)
    {
        if (t == typeof(int)) return "int";
        if (t == typeof(float)) return "float";
        if (t == typeof(bool)) return "bool";
        if (t == typeof(string)) return "string";
        if (t.IsEnum) return t.Name;
        if (t.IsByRef) return GetTypeKeyword(t.GetElementType());
        if (t.IsArray) return GetTypeKeyword(t.GetElementType()) + "[]";
        return t.Name;
    }
}
#endif
