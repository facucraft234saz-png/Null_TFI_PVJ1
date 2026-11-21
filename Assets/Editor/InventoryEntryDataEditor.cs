
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryEntryData))]
public class InventoryEntryDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawSection(
            "Informacion general",
            "Nombre y descripcion que identifican al elemento."
        );

        DrawProperty("entryName");
        DrawProperty("description");

        EditorGUILayout.Space(8);

        DrawSection(
            "Categoria",
            "Selecciona que tipo de elemento representa este registro."
        );

        DrawProperty("category");

        InventoryEntryData data =
            (InventoryEntryData)target;

        EditorGUILayout.Space(8);

        DrawProperty("icon");
        DrawProperty("placeholderColor");
        DrawProperty("maxStack");

        EditorGUILayout.Space(12);

        switch (data.category)
        {
            case InventoryCategory.Item:
                DrawItemConfiguration();
                break;

            case InventoryCategory.Enemy:
                DrawEnemyConfiguration();
                break;

            case InventoryCategory.Curse:
                DrawCurseConfiguration();
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    // ================================================
    // MEJORAS DEL JUGADOR
    // ================================================

    private void DrawItemConfiguration()
    {
        DrawSection(
            "Mejoras del jugador",
            "Este elemento puede modificar varias estadisticas del jugador. Puedes agregar tantas mejoras como necesites."
        );

        SerializedProperty effects =
            serializedObject.FindProperty("upgradeEffects");

        if (effects != null)
        {
            EditorGUILayout.PropertyField(
                effects,
                new GUIContent(
                    "Lista de mejoras",
                    "Agrega diferentes mejoras y configura el valor de cada una."
                ),
                true
            );
        }
        else
        {
            EditorGUILayout.HelpBox(
                "No se encontró la lista upgradeEffects. Revisá InventoryEntryData.cs.",
                MessageType.Error
            );
        }
    }

    // ================================================
    // CONFIGURACION DE ENEMIGOS
    // ================================================

    private void DrawEnemyConfiguration()
    {
        DrawSection(
            "Configuracion del enemigo",
            "Este elemento invoca el enemigo asignado cuando se obtiene."
        );

        DrawProperty("enemyDefinition");
    }

    // ================================================
    // CONFIGURACION DE MALDICIONES
    // ================================================

    private void DrawCurseConfiguration()
    {
        DrawSection(
            "Maldicion: enemigos afectados",
            "Selecciona los enemigos que recibiran las modificaciones de esta maldicion."
        );

        DrawProperty("affectedEnemies");

        EditorGUILayout.Space(10);

        DrawSection(
            "Maldicion: modificadores",
            "Configura las estadisticas que cambiaran en los enemigos seleccionados."
        );

        DrawProperty("enemyModifiers");

        EditorGUILayout.Space(10);

        DrawSection(
            "Maldicion: enemigos adicionales",
            "Esta maldicion tambien puede invocar enemigos nuevos. Selecciona su tipo, su definicion y la cantidad que aparecera."
        );

        DrawProperty("additionalEnemies");
    }

    // ================================================
    // ELEMENTOS VISUALES DEL INSPECTOR
    // ================================================

    private void DrawSection(
        string title,
        string description)
    {
        EditorGUILayout.LabelField(
            title,
            EditorStyles.boldLabel
        );

        EditorGUILayout.HelpBox(
            description,
            MessageType.Info
        );
    }

    private void DrawProperty(string propertyName)
    {
        SerializedProperty property =
            serializedObject.FindProperty(propertyName);

        if (property != null)
        {
            EditorGUILayout.PropertyField(
                property,
                true
            );
        }
    }
}
