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

    [SerializeField, FoldoutGroup("Projectile")]
    protected float projectileSpeed;
    [SerializeField, FoldoutGroup("Projectile")]
    protected int projectileDamage;
    [SerializeField, FoldoutGroup("Projectile")]
    protected IShootable projectilePrefab;

    protected bool inGame = false;
    public bool InGame { get { return inGame; } }

    protected List<IShootable> projectilePool;
    protected Turret currentTarget;
    protected List<Turret> possibleTargets;
    //TODO apuntar a jugador cuando no quedan torres en tu lado

    public virtual void Spawn(Vector3 startPosition, Vector3 endPosition)
    {
        currentHealth = maxHealth;
        transform.position = startPosition;
        timeToNextShoot = 1;
        transform.DOMove(endPosition, timeToGetToPosition).SetEase(Ease.OutBack).OnComplete(() => inGame = true);
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

    public void Shoot()
    {
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

    public void ReceiveDamage(int damageAmount)
    {
        Debug.Log($"{transform.name} received {damageAmount}");
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.EnemyKilled();
        Destroy(this.gameObject);
    }

    public void RestoreProjectile(IShootable projectile)
    {
        projectilePool.Add(projectile);
    }
}