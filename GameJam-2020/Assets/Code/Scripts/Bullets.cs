using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public int bullet_amount;
    public bool destroyable;
    public float time_to_live;

    public Bullets(int bullet_amount)
    {
        this.bullet_amount = bullet_amount;
        destroyable = false;
        time_to_live = 0;
    }    

    void Update()
    {
        if (destroyable)
        {
            time_to_live -= Time.deltaTime;

            if (time_to_live < 0)
            {
                Destroy(transform.gameObject);
            }
        }
    }
}
