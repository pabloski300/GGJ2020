using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject sky;
    public GameObject clouds;
    public GameObject far;
    public GameObject near;

    public float master_speed;
    public float MasterSpeed { set { master_speed = value; } }
    public float[] parallax_speed;

    private GameObject[] background1;
    private GameObject[] background2;

    private float initial_position_x;

    void Start()
    {
        Vector3 position = transform.position;
        background1 = new GameObject[4] { Instantiate(sky), Instantiate(clouds), Instantiate(far), Instantiate(near) };
        background2 = new GameObject[4] { Instantiate(sky), Instantiate(clouds), Instantiate(far), Instantiate(near) };

        initial_position_x = position.x;
        for (int i = 0; i < 4; i++)
        {
            //Parent camera
            background1[i].transform.position = new Vector3(position.x, position.y, 0);
            background2[i].transform.position = new Vector3(position.x + background1[i].GetComponent<SpriteRenderer>().bounds.extents.x * 2, position.y, 0);
        }

        background1[0].GetComponent<SpriteRenderer>().sortingLayerName = "Parallax_Sky";
        background2[0].GetComponent<SpriteRenderer>().sortingLayerName = "Parallax_Sky";
        background1[1].GetComponent<SpriteRenderer>().sortingLayerName = "Parallax_Cloud";
        background2[1].GetComponent<SpriteRenderer>().sortingLayerName = "Parallax_Cloud";
        background1[2].GetComponent<SpriteRenderer>().sortingLayerName = "Parallax_Far";
        background2[2].GetComponent<SpriteRenderer>().sortingLayerName = "Parallax_Far";
        background1[3].GetComponent<SpriteRenderer>().sortingLayerName = "Parallax_Near";
        background2[3].GetComponent<SpriteRenderer>().sortingLayerName = "Parallax_Near";

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            background1[i].transform.position -= new Vector3(master_speed * parallax_speed[i] * Time.deltaTime, 0, 0);
            background2[i].transform.position -= new Vector3(master_speed * parallax_speed[i] * Time.deltaTime, 0, 0);

            if (background1[i].transform.position.x <= initial_position_x - background1[i].GetComponent<SpriteRenderer>().bounds.extents.x * 2)
            {
                background1[i].transform.position = new Vector3(initial_position_x, background1[i].transform.position.y, 0);
                background2[i].transform.position = new Vector3(initial_position_x + background1[i].GetComponent<SpriteRenderer>().bounds.extents.x * 2, background2[i].transform.position.y, 0);
            }
        }
    }
}
