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
    public float HealthRelative {get{return (float) currentHealth/ (float)maxHealth;}}
    public UnityEvent<float> changeHealthEvent;
    [SerializeField, FoldoutGroup("Stats")]
    private int maxAmmo;
    private int currentAmmo;
    public float AmmunitionRelative {get{return (float) currentAmmo/ (float)maxAmmo;}}
    public UnityEvent<float> changeAmmoEvent;
    [SerializeField, FoldoutGroup("Stats")]
    private float shootSpeed;
    [SerializeField, FoldoutGroup("Stats")]
    private float maxCartridges;
    private float timeToNextShoot;
    [SerializeField, PropertyRange(0,1), FoldoutGroup("Stats")]
    private float timeTakingAim;
    [EnumToggleButtons, FoldoutGroup("Stats")]
    public Side side;
    public enum Side {
        Top, Bottom
    }

    [SerializeField, FoldoutGroup("Projectile")]
    private float projectileSpeed;
    [SerializeField, FoldoutGroup("Projectile")]
    private int projectileDamage;
    [SerializeField, FoldoutGroup("Projectile")]
    private IShootable projectilePrefab;

    private Enemy currentTarget;
    private bool alive = true;
    private List<Enemy> possibleTargets;
    private List<IShootable> projectilePool;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentAmmo = maxAmmo;
        timeToNextShoot = 1;
        currentTarget = null;
        projectilePool = new List<IShootable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToNextShoot > (1 - timeTakingAim))
            TakeAim();

        if (timeToNextShoot > 0 && currentAmmo > 0 && currentTarget != null)
            timeToNextShoot -= Time.deltaTime * shootSpeed;
        else if(currentAmmo > 0 && currentTarget != null)
            Shoot();

        if (currentTarget)
            Debug.DrawLine(this.transform.position, currentTarget.transform.position, Color.red);
    }

    private void TakeAim()
    {
        possibleTargets = (from x in FindObjectsOfType<Enemy>() where x.InGame select x).ToList();
        if(possibleTargets != null && possibleTargets.Count > 0)
            currentTarget = possibleTargets.OrderBy(o => (o.transform.position - this.transform.position).magnitude).ToList()[0];
    }

    private void Shoot()
    {
        currentAmmo--;
        changeAmmoEvent.Invoke(AmmunitionRelative);
        timeToNextShoot = 1;
        IShootable projectile = projectilePool.FirstOrDefault();
        if(projectile == null){
            IShootable newProjectile = ((IShootable)Instantiate((Object)projectilePrefab, this.transform.position, Quaternion.identity));
            newProjectile.Shoot(projectileSpeed,currentTarget.transform.position - this.transform.position,projectileDamage, this, transform.position);
        }else{
            projectilePool.Remove(projectile);
            projectile.Shoot(projectileSpeed,currentTarget.transform.position - this.transform.position,projectileDamage, this, transform.position);
        }
        currentTarget = null;
    }

    public void RestoreProjectile(IShootable projectile){
        projectilePool.Add(projectile);
    }

    public void ReceiveDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        changeHealthEvent.Invoke(HealthRelative);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        alive = true;
        this.gameObject.SetActive(false);
    }

    public void Repair(float repairAmount){
        currentHealth += repairAmount;
        currentHealth = Mathf.Min(currentHealth,maxHealth);
        changeHealthEvent.Invoke(HealthRelative);
    }

    public void Bullets(int ammo){
        currentAmmo += ammo;
        currentAmmo = Mathf.Min(currentAmmo,maxAmmo);
        changeAmmoEvent.Invoke(AmmunitionRelative);
    }
}
