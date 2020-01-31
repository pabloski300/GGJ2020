using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Prueba.
public class PlayerController : MonoBehaviour
{
    public PlayerProperties properties;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(properties.life);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
