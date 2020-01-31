using UnityEngine;

public class Projectile : MonoBehaviour {
    private float speed;
    private Vector3 direction;
    private float damageAmount;
    private Turret turret;
    [SerializeField]
    private float maxTimeAlive;
    private float currentTimeAlive = 0;

    public void Shoot(float _speed, Vector3 _dir, float _damageAmount, Turret _t){
        speed = _speed;
        direction = _dir;
        damageAmount = _damageAmount;
        turret = _t;
        this.transform.position = turret.transform.position;
        this.gameObject.SetActive(true);
    }

    private void Update(){
        this.transform.Translate(direction*speed*Time.deltaTime, Space.World);
        if(currentTimeAlive >= maxTimeAlive) {
            Die();
        }else{
            currentTimeAlive += Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        other.gameObject.GetComponent<IDamage>().ReceiveDamage(damageAmount);
        Die();
    }

    private void Die() {
        currentTimeAlive = 0;
            turret.RestoreBullet(this);
    }
}