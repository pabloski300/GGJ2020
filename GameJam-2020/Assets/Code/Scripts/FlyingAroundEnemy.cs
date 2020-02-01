using System.Collections;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class FlyingAroundEnemy : Enemy
{
    private bool shooting;
    [SerializeField, FoldoutGroup("Stats")]
    private float moveSpeed;
    [SerializeField, PropertyRange(0, 1), FoldoutGroup("Stats")]
    private float stopBeforeTimeToShoot;
    [SerializeField, FoldoutGroup("Stats")]
    private float shotsPerShot;
    [SerializeField, FoldoutGroup("Stats")]
    private float timeBetweenShots;

    private Vector2 centerOfEllipse;
    [SerializeField, MinMaxSlider(3, 7, true), FoldoutGroup("Movement")]
    private Vector2 minMaxVerticalAxis;
    [SerializeField, MinMaxSlider(3, 7, true), FoldoutGroup("Movement")]
    private Vector2 minMaxHorizontalAxis;


    private WaitForSeconds wait;

    private float verticalAxis;

    private float horizontalAxis;

    private float currentPositionOnEllipse;

    public override void Spawn(Vector3 startPosition, Vector3 endPosition)
    {
        wait = new WaitForSeconds(timeBetweenShots);
        centerOfEllipse = new Vector2(endPosition.x, endPosition.y);
        verticalAxis = Random.Range(minMaxVerticalAxis.x, minMaxVerticalAxis.y);
        horizontalAxis = Random.Range(minMaxHorizontalAxis.x, minMaxHorizontalAxis.y);
        currentPositionOnEllipse = Random.Range(0, 360);
        endPosition = new Vector3(centerOfEllipse.x + (horizontalAxis * Mathf.Cos(currentPositionOnEllipse)), centerOfEllipse.y + (verticalAxis * Mathf.Sin(currentPositionOnEllipse)), 0);
        base.Spawn(startPosition, endPosition);
    }

    protected override void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.GameFinished)
        {
            if (timeToNextShoot > 0 && currentTarget == null && inGame)
            {
                TakeAim();
            }

            shooting = timeToNextShoot < stopBeforeTimeToShoot;

            if (inGame && !shooting)
            {
                currentPositionOnEllipse += (Time.deltaTime * moveSpeed) % 360;
                transform.position = new Vector3(centerOfEllipse.x + (horizontalAxis * Mathf.Cos(currentPositionOnEllipse)), centerOfEllipse.y + (verticalAxis * Mathf.Sin(currentPositionOnEllipse)), 0);
            }
            base.Update();
        }
    }

    public override void Shoot()
    {
        StartCoroutine(ShootMultiple());
    }

    public IEnumerator ShootMultiple()
    {
        for (int i = 0; i < shotsPerShot; i++)
        {
            base.Shoot();
            yield return wait;
        }
    }

    private void TakeAim()
    {
        possibleTargets = (from x in FindObjectsOfType<Turret>() where x.Alive select x).ToList();
        if (possibleTargets != null && possibleTargets.Count > 0)
        {
            int x = Random.Range(0, possibleTargets.Count);
            currentTarget = possibleTargets[x];
            return;
        }
    }
}