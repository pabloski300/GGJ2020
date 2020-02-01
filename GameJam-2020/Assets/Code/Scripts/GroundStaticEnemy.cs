using UnityEngine;
using System.Linq;

public class GroundStaticEnemy : Enemy {
    protected override void Update() {
        if(GameManager.Instance.CurrentGameState != GameManager.GameState.GameFinished){
        if(timeToNextShoot > 0 && currentTarget == null && inGame){
            TakeAim();
        }
        base.Update();
        }
    }

    private void TakeAim(){
        possibleTargets = (from x in FindObjectsOfType<Turret>() where x.side == Turret.Side.Bottom && x.Alive select x).ToList();
        if(possibleTargets != null && possibleTargets.Count > 0){
            int x = Random.Range(0,possibleTargets.Count);
            currentTarget = possibleTargets[x];
            return;
        }
    }

}