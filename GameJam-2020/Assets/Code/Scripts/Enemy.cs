using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy : SerializedMonoBehaviour, IDamage, IShooter
{
    [SerializeField, FoldoutGroup("Stats")]
    protected float maxHealth;
    protected float currentHealth;
    [SerializeField, FoldoutGroup("Stats")]
    protected float shootSpeed;
    protected float timeToNextShoot;
    [SerializeField, FoldoutGroup("Stats")]
    protected float timeToGetToPosition;
    [SerializeField, FoldoutGroup("Stats")]
    protected Transform shootPoint;

    [SerializeField, FoldoutGroup("Projectile")]
    protected float projectileSpeed;
    [SerializeField, FoldoutGroup("Projectile")]
    protected int projectileDamage;
    [SerializeField, FoldoutGroup("Projectile")]
    protected IShootable projectilePrefab;

    [SerializeField, FoldoutGroup("Sound")]
    protected Sound shootSound;
    [SerializeField, FoldoutGroup("Sound")]
    protected Sound movementSound;
    [SerializeField, FoldoutGroup("Sound")]
    protected Sound explosionSound;

    [SerializeField, FoldoutGroup("FX")]
    protected GameObject explosion;



    protected bool inGame = false;
    public bool InGame { get { return inGame; } }

    protected List<IShootable> projectilePool;
    protected Turret currentTarget;
    protected List<Turret> possibleTargets;
    //TODO apuntar a jugador cuando no quedan torres en tu lado

    public virtual void Spawn(Vector3 startPosition, Vector3 endPosition)
    {
        shootSound.Init();
        movementSound.Init();
        explosionSound.Init();
        movementSound.Play(this.transform);
        currentHealth = maxHealth;
        transform.position = startPosition;
        timeToNextShoot = 1;
        transform.DOMove(endPosition, timeToGetToPosition).SetEase(Ease.OutBack).OnComplete(() => { inGame = true;/*movementSound.Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);*/});
        projectilePool = new List<IShootable>();
    }

    protected virtual void Update()
    {
        if (timeToNextShoot > 0 && currentTarget != null && inGame)
        {
            timeToNextShoot -= Time.deltaTime * shootSpeed;
        }
        if (timeToNextShoot <= 0 && currentTarget != null && inGame)
        {
            Shoot();
        }

        if (currentTarget != null)
        {
            Debug.DrawLine(this.transform.position, this.currentTarget.transform.position, Color.green);
        }
    }

    public virtual void Shoot()
    {
        shootSound.Play(this.transform);
        timeToNextShoot = 1;
        IShootable projectile = projectilePool.FirstOrDefault();
        if (projectile == null && currentTarget != null)
        {
            IShootable newProjectile = ((IShootable)Instantiate((Object)projectilePrefab, this.transform.position, Quaternion.identity));
            newProjectile.Shoot(projectileSpeed, currentTarget.transform.position - shootPoint.position, projectileDamage, this, shootPoint.position);
        }
        else if (currentTarget != null)
        {
            projectilePool.Remove(projectile);
            projectile.Shoot(projectileSpeed, currentTarget.transform.position - shootPoint.position, projectileDamage, this, shootPoint.position);
        }
        currentTarget = null;
    }

    public void ReceiveDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if(this.gameObject == null){
            return;
        }
        explosionSound.Play(this.transform);
        Instantiate(explosion, this.transform.position, Quaternion.Euler(-180, 0, 0));
        movementSound.Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        foreach (Projectile p in projectilePool)
        {
            if (p != null)
            {
                Destroy(p.gameObject);
            }
        }
        GameManager.Instance.EnemyKilled();
        if(this.gameObject != null){
            Destroy(this.gameObject);
        }
    }

    public void RestoreProjectile(IShootable projectile)
    {
        projectilePool.Add(projectile);
    }

    public void Destroy()
    {
        movementSound.Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        foreach (Projectile p in projectilePool)
        {
            if (p != null)
            {
                Destroy(p.gameObject);
            }
        }
        Destroy(this.gameObject);
    }
}