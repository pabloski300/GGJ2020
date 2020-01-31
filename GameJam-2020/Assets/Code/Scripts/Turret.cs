using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : MonoBehaviour, IDamage
{
    [SerializeField]
    private float maxHealth;
    private float currentHealth;
    [SerializeField]
    private float maxAmmo;
    private float currentAmmo;
    [SerializeField]
    private float shootSpeed;
    private float timeToNextShoot;
    [SerializeField]
    private float timeTakingAim;
    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    private float projectileDamage;
    [SerializeField]
    private float maxCartridges;
    [SerializeField]
    private GameObject projectilePrefab;
    private Enemy currentTarget;
    private bool alive = true;
    private List<Enemy> possibleTargets;
    private List<Projectile> projectilePool;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentAmmo = maxAmmo;
        timeToNextShoot = 1;
        currentTarget = null;
        projectilePool = new List<Projectile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToNextShoot > (1 - timeTakingAim))
            TakeAim();

        if (timeToNextShoot > 0 && currentAmmo > 0)
            timeToNextShoot -= Time.deltaTime * shootSpeed;
        else if(currentAmmo > 0)
            Shoot();

        if (currentTarget)
            Debug.DrawRay(this.transform.position, currentTarget.transform.position, Color.red);
    }

    private void TakeAim()
    {
        possibleTargets = FindObjectsOfType<Enemy>().ToList();
        currentTarget = possibleTargets.OrderBy(o => (o.transform.position - this.transform.position).magnitude).ToList()[0];
    }

    private void Shoot()
    {
        currentAmmo--;
        timeToNextShoot = 1;
        Projectile projectile = projectilePool.FirstOrDefault();
        if(projectile == null){
            Projectile newProjectile = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity).GetComponent<Projectile>();
            newProjectile.Shoot(projectileSpeed,currentTarget.transform.position - this.transform.position,projectileDamage, this);
        }else{
            projectilePool.Remove(projectile);
            projectile.Shoot(projectileSpeed,currentTarget.transform.position - this.transform.position,projectileDamage, this);
        }
    }

    public void RestoreBullet(Projectile projectile){
        projectile.gameObject.SetActive(false);
        projectilePool.Add(projectile);
    }

    public void ReceiveDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        alive = true;
    }
}
