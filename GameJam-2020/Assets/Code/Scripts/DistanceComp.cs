using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceComparer : IComparer<GameObject>
{
    public Vector3 position { get; set; }

    public int Compare(GameObject x, GameObject y)
    {
        if (Vector3.Distance(position, x.transform.position) < Vector3.Distance(position, y.transform.position))
            return -1;
        else if (Vector3.Distance(position, x.transform.position) > Vector3.Distance(position, y.transform.position))
            return 1;
        else
            return 0;
    }
}
