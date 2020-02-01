using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public int bullet_amount;
    public bool destroyable;
    public float time_to_live;
    PlayerController controller;

    public Bullets(int bullet_amount)
    {
        this.bullet_amount = bullet_amount;
        destroyable = false;
        time_to_live = 0;
    }    

    void Start()
    {
        controller = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        time_to_live -= Time.deltaTime;

        if (time_to_live <= 0 && destroyable)
        {
            Debug.Log(controller);
            controller.RemoveFromList(transform.gameObject);
            Destroy(transform.gameObject);
        }
    }
}
