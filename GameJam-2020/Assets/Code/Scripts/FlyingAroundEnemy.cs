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

    private Vector2 centerOfEllipse;
    [SerializeField, MinMaxSlider(3, 7, true)]
    private Vector2 minMaxVerticalAxis;
    [SerializeField, MinMaxSlider(3, 7, true)]
    private Vector2 minMaxHorizontalAxis;

    private float verticalAxis;

    private float horizontalAxis;

    private float currentPositionOnEllipse;

    public override void Spawn(Vector3 startPosition, Vector3 endPosition)
    {
        centerOfEllipse = new Vector2(endPosition.x,endPosition.y);
        verticalAxis = Random.Range(minMaxVerticalAxis.x,minMaxVerticalAxis.y);
        horizontalAxis = Random.Range(minMaxHorizontalAxis.x,minMaxHorizontalAxis.y);
        currentPositionOnEllipse = Random.Range(0,360);
        endPosition = new Vector3(centerOfEllipse.x + (horizontalAxis * Mathf.Cos(currentPositionOnEllipse)), centerOfEllipse.y + (verticalAxis * Mathf.Sin(currentPositionOnEllipse)), 0);
        base.Spawn(startPosition, endPosition);
    }

    protected override void Update()
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

    private void TakeAim()
    {
        possibleTargets = FindObjectsOfType<Turret>().ToList();
        if (possibleTargets != null && possibleTargets.Count > 0)
        {
            int x = Random.Range(0, possibleTargets.Count);
            currentTarget = possibleTargets[x];
            return;
        }
    }
}