
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ScriptDependencyViewer : EditorWindow
{
    private MonoScript selectedScript;

    private readonly List<MonoScript> dependencies =
        new List<MonoScript>();

    private Vector2 scrollPosition;

    [MenuItem("Tools/Script Dependency Viewer")]
    public static void OpenWindow()
    {
        GetWindow<ScriptDependencyViewer>(
            "Script Dependencies"
        );
    }

    private void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChanged;

        UpdateSelectedScript();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        UpdateSelectedScript();

        Repaint();
    }

    private void UpdateSelectedScript()
    {
        MonoScript script = Selection.activeObject as MonoScript;

        if (script != null)
        {
            selectedScript = script;
            AnalyzeDependencies();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField(
            "Visor de dependencias",
            EditorStyles.boldLabel
        );

        EditorGUILayout.HelpBox(
            "Selecciona un script para consultar las referencias " +
            "que tiene hacia otros scripts del proyecto.",
            MessageType.Info
        );

        EditorGUILayout.Space(8);

        EditorGUI.BeginChangeCheck();

        selectedScript = (MonoScript)EditorGUILayout.ObjectField(
            "Script seleccionado",
            selectedScript,
            typeof(MonoScript),
            false
        );

        if (EditorGUI.EndChangeCheck())
        {
            AnalyzeDependencies();
        }

        EditorGUILayout.Space(8);

        if (GUILayout.Button("Analizar dependencias"))
        {
            AnalyzeDependencies();
        }

        EditorGUILayout.Space(10);

        if (selectedScript == null)
        {
            EditorGUILayout.HelpBox(
                "Selecciona un archivo .cs en el Project.",
                MessageType.Warning
            );

            return;
        }

        EditorGUILayout.LabelField(
            "Script: " + selectedScript.name,
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField(
            "Dependencias detectadas: " + dependencies.Count,
            EditorStyles.boldLabel
        );

        scrollPosition = EditorGUILayout.BeginScrollView(
            scrollPosition
        );

        if (dependencies.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "No se detectaron referencias a otros scripts.",
                MessageType.Info
            );
        }

        foreach (MonoScript dependency in dependencies)
        {
            if (dependency == null)
                continue;

            EditorGUILayout.BeginHorizontal(
                EditorStyles.helpBox
            );

            EditorGUILayout.ObjectField(
                dependency,
                typeof(MonoScript),
                false
            );

            if (GUILayout.Button("Seleccionar", GUILayout.Width(85)))
            {
                Selection.activeObject = dependency;
                EditorGUIUtility.PingObject(dependency);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void AnalyzeDependencies()
    {
        dependencies.Clear();

        if (selectedScript == null)
            return;

        string selectedPath =
            AssetDatabase.GetAssetPath(selectedScript);

        if (string.IsNullOrEmpty(selectedPath) ||
            !File.Exists(selectedPath))
            return;

        string selectedSource =
            File.ReadAllText(selectedPath);

        string[] guids =
            AssetDatabase.FindAssets("t:MonoScript");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (path == selectedPath)
                continue;

            MonoScript candidate =
                AssetDatabase.LoadAssetAtPath<MonoScript>(path);

            if (candidate == null)
                continue;

            System.Type candidateType = candidate.GetClass();

            if (candidateType == null)
                continue;

            string typeName = candidateType.Name;

            string pattern =
                @"\b" + Regex.Escape(typeName) + @"\b";

            if (Regex.IsMatch(selectedSource, pattern))
            {
                dependencies.Add(candidate);
            }
        }

        dependencies.Sort(
            (a, b) => string.Compare(
                a.name,
                b.name,
                System.StringComparison.OrdinalIgnoreCase
            )
        );
    }
}
