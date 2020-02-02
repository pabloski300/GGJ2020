using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class FlyingStaticEnemy : Enemy
{
    [SerializeField, FoldoutGroup("Animation")]
    private Transform canon;
    protected override void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.GameFinished)
        {
            if (timeToNextShoot > 0 && currentTarget == null && inGame)
            {
                TakeAim();
            }
            if(currentTarget != null){
                canon.right = (currentTarget.transform.position - canon.position).normalized;
            }
            base.Update();
        }
    }

    private void TakeAim()
    {
        possibleTargets = (from x in FindObjectsOfType<Turret>() where x.side == Turret.Side.Top && x.Alive select x).ToList();
        if (possibleTargets != null && possibleTargets.Count > 0)
        {
            int x = Random.Range(0, possibleTargets.Count);
            currentTarget = possibleTargets[x];
            return;
        }
    }
}