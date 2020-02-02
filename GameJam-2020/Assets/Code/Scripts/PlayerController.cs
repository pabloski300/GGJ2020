using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

//Prueba.
public class PlayerController : MonoBehaviour, IDamage
{
    //Movement
    float x, y;
    int translation_x, translation_y;
    [SerializeField, FoldoutGroup("Stats")]
    public int factor;

    //Keys
    private bool main_down;
    private bool main_down_sustained;
    private bool secondary_down;

    //Object Types
    enum item { none, bullets, repair_kit }
    item carried_obj;

    //Close objects
    bool near_obj;
    Transform obj;

    //Active sprites
    Transform carrying_arm;
    Transform body;
    GameObject carried_item;

    Animator body_animator;
    SpriteRenderer body_renderer;
    bool facing_left;
    bool last_facing;

    //Item List
    List<GameObject> items_to_carry;
    DistanceComparer comparer;

    //Item spawn positions
    [SerializeField, FoldoutGroup("Stats")]
    private float drop_distance;
    [SerializeField, FoldoutGroup("Stats")]
    private float carry_offset_x;
    [SerializeField, FoldoutGroup("Stats")]
    private float drop_offset;
    private GameObject scenary;
    Vector3 scenary_center, scenary_half; 

    //Repair
    bool repairing;
    [SerializeField, FoldoutGroup("Repair")]
    private float timeToRepair;
    [SerializeField, FoldoutGroup("Repair")]
    private float repairAmount;
    float repair_counter;

    //Bullets
    [SerializeField, FoldoutGroup("Bullets")]
    private Sprite ammo_box;
    private int bullet_packages = 0;
    int MAX_CARRIED_BULLETS = 3;
    [SerializeField, FoldoutGroup("Bullets")]
    private int bulletsPerPackage;
    [SerializeField, FoldoutGroup("Bullets")]
    private float timeToLoad;
    [SerializeField, FoldoutGroup("Bullets")]
    private float slowPerBox;
    float load_counter;
    bool loading_bullets;

    [SerializeField, FoldoutGroup("Sound")]
    private Sound footSteps;
    [SerializeField, FoldoutGroup("Sound")]
    private float timeBetweenFootsteps;
    private float currentTimeBetweenFootsteps;
    [SerializeField, FoldoutGroup("Sound")]
    private Sound rechargeSound;
    [SerializeField, FoldoutGroup("Sound")]
    private Sound grabBulletsSound;
    [SerializeField, FoldoutGroup("Sound")]
    private Sound grabRepairKit;
    [SerializeField, FoldoutGroup("Sound")]
    private Sound repairSound;

    void Start()
    {
        currentTimeBetweenFootsteps = timeBetweenFootsteps;
        items_to_carry = new List<GameObject>();
        comparer = new DistanceComparer();
        body = transform.GetChild(0);
        body_animator = body.GetComponent<Animator>();
        body_renderer = body.GetComponent<SpriteRenderer>();
        carrying_arm = transform.GetChild(1);

        repair_counter = timeToRepair;
        load_counter = timeToLoad;

        near_obj = false;
        repairing = false;
        loading_bullets = false;
        main_down = false;
        main_down_sustained = false;
        secondary_down = false;
        facing_left = false;
        obj = null;

        scenary = GameObject.FindGameObjectWithTag("scenary");
        TilemapRenderer tr = scenary.GetComponent<TilemapRenderer>();
        scenary_center = tr.bounds.center;
        scenary_half = tr.bounds.extents;

        footSteps.Init();
        rechargeSound.Init();
        grabBulletsSound.Init();
        grabRepairKit.Init();
        repairSound.Init();
    }

    void Update()
    {
        main_down = Input.GetKeyDown(KeyCode.Z);
        main_down_sustained = Input.GetKey(KeyCode.Z);
        secondary_down = Input.GetKeyDown(KeyCode.X);

        if (loading_bullets && load_counter >= 0)
        {
            load_counter -= Time.deltaTime;
            currentTimeBetweenFootsteps = timeBetweenFootsteps;
            return;
        }
        else if (loading_bullets)
        {
            rechargeSound.Play(this.transform);
            loading_bullets = false;
            load_counter = timeToLoad;

            //Restore bullets
            Debug.Log("Bullets restored");
            Turret t = obj.GetComponent<Turret>();
            t.Bullets(bullet_packages * bulletsPerPackage);
            bullet_packages = 0;
        }

        if (repairing && main_down_sustained)
        {
            Repair();
            currentTimeBetweenFootsteps = timeBetweenFootsteps;
            return;
        }
        else if (repairing)
        {
            repairing = false;
            carrying_arm.gameObject.SetActive(true);
            body_animator.SetBool("Repairing", false);
            repair_counter = timeToRepair;
        }

        if (near_obj && main_down) TryUse();
        else if (secondary_down) Drop();
        else Move();
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

        if ((x != 0 || y != 0) && currentTimeBetweenFootsteps <= 0)
        {
            currentTimeBetweenFootsteps = timeBetweenFootsteps;
            footSteps.Play(this.transform);
        }
        else if (x != 0 || y!= 0)
        {
            currentTimeBetweenFootsteps -= Time.deltaTime;
        }
        else
        {
            currentTimeBetweenFootsteps = timeBetweenFootsteps;
        }

        last_facing = facing_left;

        if (x < 0) facing_left = true;
        else if (x > 0) facing_left = false;

        body_renderer.flipX = facing_left;
        carrying_arm.GetComponent<SpriteRenderer>().flipX = facing_left;

        if (carrying_arm.transform.childCount > 0 && last_facing != facing_left) {
            Vector3 pos = carrying_arm.transform.GetChild(0).position;
            if (facing_left)
                carrying_arm.transform.GetChild(0).position -= new Vector3(carry_offset_x, 0, 0);
            else
                carrying_arm.transform.GetChild(0).position += new Vector3(carry_offset_x,0,0);
        }

        if (translation_x != 0 || translation_y != 0) body_animator.SetBool("Walking", true);
        else body_animator.SetBool("Walking", false);

        float currentSpeed = 1 - (bullet_packages * slowPerBox);
        body_animator.SetFloat("WalkSpeed", currentSpeed);

        transform.Translate(translation_x * currentSpeed * Time.deltaTime, translation_y * currentSpeed * Time.deltaTime, 0);
    }

    void TryUse()
    {
        //Object to use
        comparer.position = transform.position;
        items_to_carry.Sort(comparer);
        obj = items_to_carry[0].transform;
        if (obj.CompareTag("tower"))
        {
            switch (carried_obj)
            {
                case item.bullets:

                    Destroy(carried_item);
                    carried_obj = item.none;
                    body_animator.SetBool("Carrying", false);
                    carrying_arm.gameObject.SetActive(false);

                    loading_bullets = true;
                    break;
                case item.repair_kit:
                    Debug.Log("Repairing Tower");
                    repairing = true;
                    if (facing_left) body.GetComponent<SpriteRenderer>().flipX = true;
                    body_animator.SetBool("Repairing", true);
                    carrying_arm.gameObject.SetActive(false);
                    break;
            }

            return;
        }
        else if (obj == null || (carried_obj != item.none && carried_obj != item.bullets) || bullet_packages >= MAX_CARRIED_BULLETS) return;

        Pick();

    }

    void Repair()
    {
        Debug.Log("Repairing");
        repair_counter -= Time.deltaTime;

        if (repair_counter <= 0)
        {
            Turret t = obj.GetComponent<Turret>();
            t.Repair(repairAmount);
            repair_counter = timeToRepair;

            if (t.HealthRelative == 1)
            {
                repairing = false;
                carrying_arm.gameObject.SetActive(true);
                body_animator.SetBool("Repairing", false);
            }
        }
    }

    void Pick()
    {
        body_animator.SetBool("Carrying", true);
        carrying_arm.gameObject.SetActive(true);

        //Workaroud
        if (obj != items_to_carry[0].transform) return;
        switch (obj.tag)
        {
            case "bullets":
                if (bullet_packages < MAX_CARRIED_BULLETS)
                {
                    grabBulletsSound.Play(this.transform);
                    carried_obj = item.bullets;

                    if (bullet_packages == 0)
                    {
                        SetCarried();
                        carried_item.GetComponent<Bullets>().destroyable = false;
                    }

                    bullet_packages++;

                    if (obj.GetComponent<Bullets>().destroyable)
                    {
                        Destroy(obj.gameObject);
                    }
                }
                break;
            case "repair_kit":
                if (bullet_packages > 0) break;

                grabRepairKit.Play(this.transform);
                SetCarried();
                carried_obj = item.repair_kit;

                Destroy(obj.gameObject);
                break;
        }


    }

    void SetCarried()
    {
        carried_item = Instantiate(obj).gameObject;
        if (obj.CompareTag("bullets"))
            carried_item.GetComponent<SpriteRenderer>().sprite = ammo_box;

        carried_item.GetComponent<Collider2D>().enabled = false;
        carried_item.transform.position = transform.position + new Vector3(0, 0.75f, 0);
        carried_item.transform.SetParent(carrying_arm);
        
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
        body_animator.SetBool("Carrying", false);
        carrying_arm.gameObject.SetActive(false);

        for (int i = 0; i < bullet_packages; i++)
        {
            GameObject instance = Instantiate(carried_item, CalculateDropPosition(), Quaternion.identity);
            instance.GetComponent<Collider2D>().enabled = true;
        }

        Destroy(carried_item);
        carried_obj = item.none;
        bullet_packages = 0;
    }

    Vector2 CalculateDropPosition()
    {
        float angle = Random.Range(0, 359) * Mathf.Deg2Rad;
        float x = transform.position.x + drop_distance * Mathf.Cos(angle);
        float y = transform.position.y + drop_distance * Mathf.Sin(angle);

        if (x > scenary_center.x + scenary_half.x - drop_offset)
            x = scenary_center.x + scenary_half.x - drop_offset;
        else if (x < scenary_center.x - scenary_half.x + drop_offset)
            x = scenary_center.x - scenary_half.x + drop_offset;

        if (y > scenary_center.y + scenary_half.y - drop_offset)
            y = scenary_center.y + scenary_half.y - drop_offset;
        else if (y < scenary_center.y - scenary_half.y + drop_offset)
            y = scenary_center.y - scenary_half.y + drop_offset;

        return new Vector2(x, y);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        near_obj = true;
        obj = collider.transform;
        items_to_carry.Add(obj.gameObject);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        items_to_carry.Remove(collider.gameObject);
        near_obj = items_to_carry.Count != 0;
        obj = null;
    }

    public void ReceiveDamage(int damageAmount)
    {
        Drop();
    }
}
