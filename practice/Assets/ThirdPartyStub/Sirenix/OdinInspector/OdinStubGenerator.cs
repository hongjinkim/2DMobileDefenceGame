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
    // �ڵ� ó�� ��� ���ӽ����̽�
    static readonly string[] TargetNamespaces = {
        "Sirenix.OdinInspector",
        "Sirenix.OdinSerializer"
    };

    [MenuItem("Tools/Generate Odin Attribute Stubs (Inspector + Serializer)")]
    public static void Generate()
    {
        var sb = new StringBuilder();
        var stubTypes = new Dictionary<string, HashSet<Type>>();
        var processedTypes = new HashSet<string>();

        sb.AppendLine("#if ODIN_INSPECTOR_OFF");
        sb.AppendLine("using System;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using Object = UnityEngine.Object;");

        // ����� ��� ��ȸ�ؼ� Ÿ�� ���ӽ����̽� Ÿ�Ը� ����
        foreach (var ns in TargetNamespaces)
        {
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}");
            sb.AppendLine("{");

            stubTypes[ns] = new HashSet<Type>();

            // ���� ����� ���� (���ӽ����̽� ����)
            var odinTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => t.IsPublic && t.Namespace == ns);

            // Attribute, Enum, Class/Struct ��� Stub ���
            foreach (var t in odinTypes)
            {
                if (!processedTypes.Add(ns + "." + t.Name))
                    continue;

                if (typeof(Attribute).IsAssignableFrom(t))
                {
                    sb.AppendLine($"    [AttributeUsage(AttributeTargets.All)]");
                    sb.AppendLine($"    public class {t.Name} : Attribute");
                    sb.AppendLine("    {");
                    // ������
                    bool hasParameterless = false;
                    foreach (var ctor in t.GetConstructors())
                    {
                        var parameters = ctor.GetParameters();
                        if (parameters.Length == 0) hasParameterless = true;
                        var paramList = string.Join(", ", parameters
                            .Select(p => $"{GetTypeKeyword(p.ParameterType)} {p.Name}"));
                        sb.AppendLine($"        public {t.Name}({paramList}) {{ }}");
                        // �Ķ���� Ÿ�� Enum/Class�� ���� Stub�� �߰�
                        foreach (var p in parameters)
                        {
                            var paramType = p.ParameterType;
                            if (paramType.Namespace == ns && (paramType.IsEnum || paramType.IsClass || (paramType.IsValueType && !paramType.IsPrimitive && !paramType.IsEnum)))
                                stubTypes[ns].Add(paramType);
                        }
                    }
                    if (!hasParameterless)
                        sb.AppendLine($"        public {t.Name}() {{ }}");
                    sb.AppendLine("    }");
                }
                else if (t.IsEnum)
                {
                    sb.AppendLine($"    public enum {t.Name}");
                    sb.AppendLine("    {");
                    foreach (var v in Enum.GetNames(t))
                    {
                        sb.AppendLine($"        {v},");
                    }
                    sb.AppendLine("    }");
                }
                else if (t.IsClass)
                {
                    sb.AppendLine($"    public class {t.Name} {{ }}");
                }
                else if (t.IsValueType && !t.IsPrimitive)
                {
                    sb.AppendLine($"    public struct {t.Name} {{ }}");
                }
            }

            // Stub�� ������ Enum/Class �߰�
            foreach (var dep in stubTypes[ns])
            {
                if (!processedTypes.Add(ns + "." + dep.Name))
                    continue;

                if (dep.IsEnum)
                {
                    sb.AppendLine($"    public enum {dep.Name}");
                    sb.AppendLine("    {");
                    foreach (var v in Enum.GetNames(dep))
                    {
                        sb.AppendLine($"        {v},");
                    }
                    sb.AppendLine("    }");
                }
                else if (dep.IsClass)
                {
                    sb.AppendLine($"    public class {dep.Name} {{ }}");
                }
                else if (dep.IsValueType && !dep.IsPrimitive)
                {
                    sb.AppendLine($"    public struct {dep.Name} {{ }}");
                }
            }

            sb.AppendLine("}");
        }

        sb.AppendLine("#endif");

        // ���� ���
        var outputPath = "Assets/ThirdPartyStub/Sirenix/OdinInspector/OdinInspectorStubs.cs";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("�Ϸ�!", $"Odin Inspector/Serializer Stub ���� �Ϸ�!\n{outputPath}", "OK");
    }

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
