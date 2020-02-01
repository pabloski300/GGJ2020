using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Wave", menuName = "Wave", order = 0)]
public class Wave : SerializedScriptableObject {
    public List<EnemyNumber> enemies;
    public float timeToNextWave = 15;
}