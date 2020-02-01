using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Prueba.
public class PlayerController : MonoBehaviour
{
    //Movement
    float x, y;
    int translation_x, translation_y;
    public float speed;
    public int factor;

    //Keys
    public bool main_down;
    public bool secondary_down;

    //Object Types
    enum item { none, bullets, repair_kit}
    item carried_obj;

    public int bullet_packages = 0;
    int MAX_CARRIED_BULLETS = 3;

    //Close objects
    bool near_obj;
    Transform obj;

    //Active sprites
    Transform carrying_body;
    Transform body;
    GameObject carried_item;

    //Item List
    List<GameObject> items_to_carry;
    DistanceComparer comparer;

    void Start()
    {
        items_to_carry = new List<GameObject>();
        comparer = new DistanceComparer();
        body = transform.GetChild(0);
        carrying_body = transform.GetChild(1);

        near_obj = false;
        main_down = false;
        secondary_down = false;
        obj = null;
    }

    void Update()
    {
        main_down = Input.GetKeyDown(KeyCode.Z);
        secondary_down = Input.GetKeyDown(KeyCode.X);

        Move();
        if (near_obj && main_down) TryUse();
        else if (secondary_down) Drop();
        main_down = false;
        secondary_down = false;
    }

    void Move()
    {
        translation_x = 0;
        translation_y = 0;
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        if (x > 0)
        {
            translation_x = factor;
        }
        else if (x < 0)
        {
            translation_x = -factor;
        }

        if (y > 0)
        {
            translation_y = factor;
        }
        else if (y < 0)
        {
            translation_y = -factor;
        }

        transform.Translate(translation_x * speed * Time.deltaTime, translation_y * speed * Time.deltaTime, 0);
    }

    void TryUse()
    {
        //Object to use
        comparer.position = transform.position;
        items_to_carry.Sort(comparer);
        obj = items_to_carry[0].transform;
        items_to_carry.RemoveAt(0);

        if (obj.CompareTag("tower"))
        {
            switch (carried_obj)
            {
                case item.bullets:
                    //Restore bullets
                    Debug.Log("Bullets restored");
                    bullet_packages = 0;

                    Destroy(carried_item);
                    carried_obj = item.none;
                    carrying_body.gameObject.SetActive(false);
                    body.gameObject.SetActive(true);
                    break;
                case item.repair_kit:
                    //Repair
                    break;
            }

            return;
        }
        else if (obj == null || (carried_obj != item.none && carried_obj != item.bullets) || bullet_packages >= MAX_CARRIED_BULLETS) return;

        Pick();

    }

    void Pick()
    {
        carrying_body.gameObject.SetActive(true);
        body.gameObject.SetActive(false);

        switch (obj.tag)
        {
            case "bullets":
                carried_obj = item.bullets;

                if (bullet_packages == 0)
                {
                    SetCarried();
                    carried_item.GetComponent<Bullets>().destroyable = false;
                }
                
                bullet_packages++;

                if (obj.GetComponent<Bullets>().destroyable)
                {
                    RemoveFromList(obj.gameObject);
                    Destroy(obj.gameObject);
                }
                break;
            case "repair_kit":
                SetCarried();
                carried_obj = item.repair_kit;

                RemoveFromList(obj.gameObject);
                Destroy(obj.gameObject);
                break;
        }


    }

    void SetCarried()
    {
        carried_item = Instantiate(obj).gameObject;
        carried_item.GetComponent<Collider2D>().enabled = false;
        carried_item.GetComponent<SpriteRenderer>().sortingOrder = carrying_body.GetComponent<SpriteRenderer>().sortingOrder + 1;
        carried_item.transform.SetParent(carrying_body);
    }

    void Drop()
    {
        if (carried_obj == item.none) return;
        else if (carried_obj == item.bullets)
        {
            Bullets b = carried_item.GetComponent<Bullets>();
            b.time_to_live = 5;
            b.destroyable = true;

        }
        else bullet_packages++;

        //Set item down
        carrying_body.gameObject.SetActive(false);
        body.gameObject.SetActive(true);

        carried_item.GetComponent<Collider2D>().enabled = true;
        //Remove later
        carried_item.transform.localScale = new Vector3(0.05f, 0.05f, 0);

        for(int i = 0; i < bullet_packages; i++)
            Instantiate(carried_item, body.position, Quaternion.identity);

        Destroy(carried_item);
        carried_obj = item.none;
        bullet_packages = 0;
    }

    //Change to nearest using a list instead of last entered
    void OnTriggerEnter2D(Collider2D collider)
    {
        near_obj = true;
        obj = collider.transform;
        items_to_carry.Add(obj.gameObject);
    }

    void OnTriggerExit2D()
    {
        near_obj = false;
        obj = null;
    }

    public void RemoveFromList(GameObject go)
    {
        //Remove from list
        if (items_to_carry.Contains(go.gameObject)) items_to_carry.Remove(go);
    }
}
