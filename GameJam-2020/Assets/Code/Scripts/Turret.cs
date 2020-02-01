using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Turret : SerializedMonoBehaviour, IDamage, IShooter
{
    [SerializeField, FoldoutGroup("Stats")]
    private float maxHealth;
    private float currentHealth;
    public float HealthRelative { get { return (float)currentHealth / (float)maxHealth; } }
    public UnityEvent<float> changeHealthEvent;
    [SerializeField, FoldoutGroup("Stats")]
    private int maxAmmo;
    private int currentAmmo;
    public float AmmunitionRelative { get { return (float)currentAmmo / (float)maxAmmo; } }
    public UnityEvent<float> changeAmmoEvent;
    [SerializeField, FoldoutGroup("Stats")]
    private float shootSpeed;
    [SerializeField, FoldoutGroup("Stats")]
    private float maxCartridges;
    private float timeToNextShoot;
    [SerializeField, PropertyRange(0, 1), FoldoutGroup("Stats")]
    private float timeTakingAim;
    [EnumToggleButtons, FoldoutGroup("Stats")]
    public Side side;
    public enum Side
    {
        Top, Bottom
    }

    [SerializeField, FoldoutGroup("Projectile")]
    private float projectileSpeed;
    [SerializeField, FoldoutGroup("Projectile")]
    private int projectileDamage;
    [SerializeField, FoldoutGroup("Projectile")]
    private IShootable projectilePrefab;

    [SerializeField, FoldoutGroup("Sounds")]
    private Sound shootSound;
    [SerializeField, FoldoutGroup("Sounds")]
    private Sound movementSound;

    [SerializeField, FoldoutGroup("Animation")]
    private GameObject headUp;
    [SerializeField, FoldoutGroup("Animation")]
    private GameObject headDown;
    [SerializeField, FoldoutGroup("Animation")]
    private Transform rotationPoint;

    private Collider2D collider;
    private Enemy currentTarget;
    private bool alive = true;
    public bool Alive { get { return alive; } }
    private List<Enemy> possibleTargets;
    private List<IShootable> projectilePool;

    // Start is called before the first frame update
    void Start()
    {
        shootSound.Init();
        movementSound.Init();
        alive = true;
        collider = GetComponent<Collider2D>();
        collider.enabled = true;
        currentHealth = maxHealth;
        currentAmmo = maxAmmo;
        timeToNextShoot = 1;
        currentTarget = null;
        projectilePool = new List<IShootable>();
    }

    public void Init()
    {
        alive = true;
        collider.enabled = true;
        currentHealth = maxHealth;
        currentAmmo = maxAmmo;
        timeToNextShoot = 1;
        currentTarget = null;
        changeAmmoEvent.Invoke(AmmunitionRelative);
        changeHealthEvent.Invoke(HealthRelative);
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            if (timeToNextShoot > (1 - timeTakingAim))
            {
                TakeAim();
            }

            if (currentTarget != null)
            {
                movementSound.Play(this.transform);

                rotationPoint.right = (currentTarget.transform.position - rotationPoint.transform.position).normalized;
                if(rotationPoint.right.y > 0){
                    headUp.SetActive(true);
                    headDown.SetActive(false);
                }else{
                    headUp.SetActive(false);
                    headDown.SetActive(true);
                }
            }
            else
            {
                movementSound.Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            if (timeToNextShoot > 0 && currentAmmo > 0 && currentTarget != null)
                timeToNextShoot -= Time.deltaTime * shootSpeed;
            else if (currentAmmo > 0 && currentTarget != null)
                Shoot();

            if (currentTarget)
                Debug.DrawLine(this.transform.position, currentTarget.transform.position, Color.red);
        }
    }

    private void TakeAim()
    {
        possibleTargets = (from x in FindObjectsOfType<Enemy>() where x.InGame select x).ToList();
        if (possibleTargets != null && possibleTargets.Count > 0)
            currentTarget = possibleTargets.OrderBy(o => (o.transform.position - this.transform.position).magnitude).ToList()[0];
    }

    private void Shoot()
    {
        shootSound.Play(this.transform);
        currentAmmo--;
        changeAmmoEvent.Invoke(AmmunitionRelative);
        timeToNextShoot = 1;
        IShootable projectile = projectilePool.FirstOrDefault();
        if (projectile == null)
        {
            IShootable newProjectile = ((IShootable)Instantiate((Object)projectilePrefab, this.transform.position, Quaternion.identity));
            newProjectile.Shoot(projectileSpeed, currentTarget.transform.position - this.transform.position, projectileDamage, this, transform.position);
        }
        else
        {
            projectilePool.Remove(projectile);
            projectile.Shoot(projectileSpeed, currentTarget.transform.position - this.transform.position, projectileDamage, this, transform.position);
        }
        currentTarget = null;
    }

    public void RestoreProjectile(IShootable projectile)
    {
        projectilePool.Add(projectile);
    }

    public void ReceiveDamage(int damageAmount)
    {
        if (alive)
        {
            currentHealth -= damageAmount;
            changeHealthEvent.Invoke(HealthRelative);
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        currentTarget = null;
        collider.enabled = false;
        GameManager.Instance.TurretKilled();
        alive = false;
        // this.gameObject.SetActive(false);
    }

    public void Repair(float repairAmount)
    {
        currentHealth += repairAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        changeHealthEvent.Invoke(HealthRelative);
    }

    public void Bullets(int ammo)
    {
        currentAmmo += ammo;
        currentAmmo = Mathf.Min(currentAmmo, maxAmmo);
        changeAmmoEvent.Invoke(AmmunitionRelative);
    }

    public void RemoveBullets(int amount)
    {
        currentAmmo -= amount;
    }
}
