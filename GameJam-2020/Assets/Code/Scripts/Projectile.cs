using UnityEngine;

public class Projectile : MonoBehaviour, IShootable
{
    private float speed;
    private Vector3 direction;
    private int damageAmount;
    private IShooter shooter;
    [SerializeField]
    private float maxTimeAlive;
    private float currentTimeAlive = 0;
    [SerializeField]
    private LayerMask collisionLayers;
    private IDamage goingToHit;
    [SerializeField]
    private Sound hitSound;

    public void Shoot(float _speed, Vector3 _dir, int _damageAmount, IShooter _shooter, Vector3 _shootPosition)
    {
        hitSound.Init();
        speed = _speed;
        direction = _dir;
        damageAmount = _damageAmount;
        shooter = _shooter;
        this.transform.position = _shootPosition;
        this.transform.right = _dir.normalized;
        this.gameObject.SetActive(true);
    }

    private void Update()
    {
        try
        {
            if (goingToHit != null)
            {
                hitSound.Play(this.transform);
                goingToHit?.ReceiveDamage(damageAmount);
                Die();
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction, speed * Time.deltaTime, collisionLayers);
            if (hit.collider != null)
            {
                this.transform.position = hit.point;
                this.goingToHit = hit.collider.GetComponent<IDamage>();
            }
            else
            {
                this.transform.Translate(direction * speed * Time.deltaTime, Space.World);
            }
            if (currentTimeAlive >= maxTimeAlive)
            {
                Die();
            }
            else
            {
                currentTimeAlive += Time.deltaTime;
            }
        }
        catch
        {
            Die();
        }
    }

    private void Die()
    {
        goingToHit = null;
        currentTimeAlive = 0;
        this.gameObject.SetActive(false);
        if (shooter != null)
        {
            shooter.RestoreProjectile(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}