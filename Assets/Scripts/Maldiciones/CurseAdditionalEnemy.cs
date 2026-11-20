
using System;
using UnityEngine;

[Serializable]
public class CurseAdditionalEnemy
{
    [Header("Tipo de enemigo")]
    public EnemyType enemyType = EnemyType.Basic;

    [Header("Enemigo que se agregara")]
    public EnemyDefinition enemyDefinition;

    [Header("Cantidad")]
    [Min(1)]
    public int amount = 1;
}
