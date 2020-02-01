using UnityEngine;
using System.Linq;

public class GroundStaticEnemy : Enemy {
    protected override void Update() {
        if(timeToNextShoot > 0 && currentTarget == null && inGame){
            TakeAim();
        }
        base.Update();
    }

    private void TakeAim(){
        possibleTargets = (from x in FindObjectsOfType<Turret>().ToList() where x.side == Turret.Side.Bottom select x).ToList();
        if(possibleTargets != null && possibleTargets.Count > 0){
            int x = Random.Range(0,possibleTargets.Count);
            currentTarget = possibleTargets[x];
            return;
        }

    }
}