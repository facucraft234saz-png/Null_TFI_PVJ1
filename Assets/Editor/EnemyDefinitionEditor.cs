
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyDefinition))]
public class EnemyDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnemyDefinition definition = (EnemyDefinition)target;

        DrawSection(
            "Informacion",
            "Datos que identifican al enemigo."
        );

        DrawProperty("enemyName");
        DrawProperty("description");

        EditorGUILayout.Space(10);

        DrawSection(
            "Tipo de enemigo",
            "Selecciona el comportamiento que utilizara este enemigo."
        );

        DrawProperty("enemyType");

        EditorGUILayout.Space(10);

        DrawSection(
            "Prefab",
            "Objeto que sera instanciado cuando el enemigo aparezca."
        );

        DrawProperty("enemyPrefab");

        EditorGUILayout.Space(10);

        DrawSection(
            "Movimiento",
            "Configuracion de desplazamiento del enemigo."
        );

        DrawProperty("movementSpeed");

        EditorGUILayout.Space(10);

        DrawSection(
            "Aparicion",
            "Configuracion de aparicion del enemigo."
        );

        DrawProperty("spawnDelay");
        DrawProperty("spawnDistance");

        EditorGUILayout.Space(10);

        DrawSection(
            "Tamaño del enemigo",
            "Escala visual que tendra el enemigo."
        );

        DrawProperty("enemyScale");

        EditorGUILayout.Space(12);

        if (definition.enemyType == EnemyType.Basic)
        {
            DrawBasicEnemyConfiguration();
        }
        else if (definition.enemyType == EnemyType.Distance)
        {
            DrawDistanceEnemyConfiguration();
        }

        serializedObject.ApplyModifiedProperties();
    }

    // --------------------------------------------
    // ENEMY BASIC
    // --------------------------------------------

    private void DrawBasicEnemyConfiguration()
    {
        DrawSection(
            "Ataque de contacto - EnemyBasic",
            "Este enemigo produce daño cuando entra en contacto con el jugador."
        );

        DrawProperty("damageHearts");
        DrawProperty("contactImmunityTime");
    }

    // --------------------------------------------
    // ENEMY DISTANCE
    // --------------------------------------------

    private void DrawDistanceEnemyConfiguration()
    {
        DrawSection(
            "Ataque a distancia - EnemyDistance",
            "Este enemigo no produce daño por contacto. Ataca mediante proyectiles."
        );

        DrawProperty("attackRange");
        DrawProperty("attackCooldown");

        EditorGUILayout.Space(8);

        DrawSection(
            "Configuracion de proyectiles",
            "Estadisticas de los proyectiles que dispara el enemigo."
        );

        DrawProperty("projectilePrefab");
        DrawProperty("projectileDamage");
        DrawProperty("projectileCount");
        DrawProperty("projectileSpeed");
        DrawProperty("projectileScale");
        DrawProperty("projectileLifetime");
    }

    // --------------------------------------------
    // UTILIDADES
    // --------------------------------------------

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
