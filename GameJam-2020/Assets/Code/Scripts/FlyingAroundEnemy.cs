using System.Collections;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class FlyingAroundEnemy : Enemy
{
    private bool shooting;
    private bool inShoot;
    private Turret inShotTarget;
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

    [SerializeField, FoldoutGroup("Animation")]
    private GameObject canon;


    private WaitForSeconds wait;

    private float verticalAxis;

    private float horizontalAxis;

    private float currentPositionOnEllipse;
    private Vector3 lastPosition;

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
            if (timeToNextShoot > 0 && currentTarget == null && inGame && !inShoot)
            {
                TakeAim();
            }

            shooting = timeToNextShoot < stopBeforeTimeToShoot;

            if (inGame && !shooting && !inShoot)
            {
                currentPositionOnEllipse += (Time.deltaTime * moveSpeed) % 360;
                transform.position = new Vector3(centerOfEllipse.x + (horizontalAxis * Mathf.Cos(currentPositionOnEllipse)), centerOfEllipse.y + (verticalAxis * Mathf.Sin(currentPositionOnEllipse)), 0);
                Vector3 dir = (transform.position - lastPosition).normalized;
                transform.localScale = new Vector3(Mathf.Sign(dir.x) * 1, 1, 1);
                if (currentTarget != null)
                {
                    canon.transform.right = Mathf.Sign(dir.x) * (currentTarget.transform.position - canon.transform.position).normalized;
                }
                lastPosition = transform.position;
            }

            if (!inShoot)
            {
                if (timeToNextShoot > 0 && currentTarget != null && inGame)
                {
                    timeToNextShoot -= Time.deltaTime * shootSpeed;
                }
                if (timeToNextShoot <= 0 && currentTarget != null && inGame)
                {
                    this.ShootNew();
                }

                if (currentTarget != null)
                {
                    Debug.DrawLine(this.transform.position, this.currentTarget.transform.position, Color.green);
                }
            }
        }
    }


    public void ShootNew()
    {
        inShotTarget = this.currentTarget;
        inShoot = true;
        StartCoroutine(ShootMultiple());
    }

    public IEnumerator ShootMultiple()
    {
        for (int i = 0; i < shotsPerShot; i++)
        {
            currentTarget = inShotTarget;
            base.Shoot();
            yield return wait;
        }
        inShoot = false;
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