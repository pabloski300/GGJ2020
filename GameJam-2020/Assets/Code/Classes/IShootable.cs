using UnityEngine;

public interface IShootable {
    void Shoot(float _speed, Vector3 _dir, int _damageAmount, IShooter _shooter, Vector3 _shootPosition);
}