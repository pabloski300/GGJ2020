using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class GroupWave {
    public List<Wave> waves;

    [HorizontalGroup("Group1")]
    public int maxWave;
    [HorizontalGroup("Group1")]
    public int minWave;

    public Wave GetWave(){
        int x = Random.Range(0,waves.Count);
        return waves[x];
    }
}