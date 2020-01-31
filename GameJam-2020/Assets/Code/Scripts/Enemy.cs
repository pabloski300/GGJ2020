using UnityEngine;

public class Enemy : MonoBehaviour, IDamage
{
    public void ReceiveDamage(float damageAmount)
    {
        Debug.Log($"{transform.name} received {damageAmount}");
    }
}