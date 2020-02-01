using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private int timeBetweenWaves;

    [SerializeField, MinMaxSlider(-20, 20, true), BoxGroup("AerialEnemy")]
    private Vector2 minMaxAerialHeightSpawn;
    [SerializeField, MinMaxSlider(-20, 20, true), BoxGroup("AerialEnemy")]
    private Vector2 minMaxAerialWidthSpawn;

    [SerializeField, MinMaxSlider(-20, 20, true), BoxGroup("GroundEnemy")]
    private Vector2 minMaxGroundHeightSpawn;
    [SerializeField, MinMaxSlider(-20, 20, true), BoxGroup("GroundEnemy")]
    private Vector2 minMaxGroundWidthSpawn;
    [SerializeField, MinMaxSlider(-20, 20, true), BoxGroup("CircleEnemy")]
    private Vector2 minMaxAerialCircleHeightSpawn;
    [SerializeField, MinMaxSlider(-20, 20, true), BoxGroup("CircleEnemy")]
    private Vector2 minMaxAerialCircleWidthSpawn;
    [SerializeField, MinMaxSlider(0.1f,1, true)]
    private Vector2 minMaxTimeBetweenEnemies;

    [SerializeField]
    private List<GroupWave> groupWaves;
    private int currentWaveNumber = 0;

    [Button("Spawn Enemies")]
    public void SpawnFunction()
    {
        StartCoroutine(Spawn());
    }
    public IEnumerator Spawn()
    {
        currentWaveNumber++;
        currentWaveNumber = Mathf.Min(groupWaves.Max(x => x.maxWave), currentWaveNumber);

        List<GroupWave> groupsAllowed = (from x in groupWaves where x.minWave <= currentWaveNumber && x.maxWave >= currentWaveNumber select x).ToList();

        int randomWave = Random.Range(0, groupsAllowed.Count);

        Wave currentWave = groupsAllowed[randomWave].GetWave();

        GameManager.Instance.WaveStarted(currentWave.timeToNextWave);

        List<Enemy> enemiesShuffled = new List<Enemy>();
        yield return null;

        foreach (EnemyNumber e in currentWave.enemies)
        {
            int amount = Random.Range(e.minAmount, e.maxAmount);
            for (int i = 0; i < amount; i++)
            {
                enemiesShuffled.Add(e.enemyType);
            }
        }

        enemiesShuffled.Shuffle();

        Debug.Log(enemiesShuffled.Count);

        foreach (Enemy e in enemiesShuffled)
        {
            Enemy spawned = Instantiate(e);
            GameManager.Instance.EnemySpawned();
            if (typeof(GroundStaticEnemy) == e.GetType())
            {
                SpawnGround(spawned);
            }

            if (typeof(FlyingStaticEnemy) == e.GetType())
            {
                SpawnAir(spawned);
            }

            if (typeof(FlyingAroundEnemy) == e.GetType())
            {
                SpawnCircle(spawned);
            }

            yield return new WaitForSeconds(Random.Range(minMaxTimeBetweenEnemies.x, minMaxTimeBetweenEnemies.y));
        }
    }

    private void SpawnAir(Enemy e)
    {
        float yInit = Random.Range(minMaxAerialHeightSpawn.x, minMaxAerialHeightSpawn.y);
        float y = Random.Range(minMaxAerialHeightSpawn.x, minMaxAerialHeightSpawn.y);
        float x = Random.Range(minMaxAerialWidthSpawn.x, minMaxAerialWidthSpawn.y);

        e.Spawn(new Vector3(-12, yInit, 0), new Vector3(x, y, 0));
    }

    private void SpawnGround(Enemy e)
    {
        float yInit = Random.Range(minMaxGroundHeightSpawn.x, minMaxGroundHeightSpawn.y);
        float y = Random.Range(minMaxGroundHeightSpawn.x, minMaxGroundHeightSpawn.y);
        float x = Random.Range(minMaxGroundWidthSpawn.x, minMaxGroundWidthSpawn.y);

        e.Spawn(new Vector3(-12, yInit, 0), new Vector3(x, y, 0));
    }

    private void SpawnCircle(Enemy e)
    {
        float yInit = Random.Range(minMaxAerialCircleHeightSpawn.x, minMaxAerialCircleHeightSpawn.y);
        float y = Random.Range(minMaxAerialCircleHeightSpawn.x, minMaxAerialCircleHeightSpawn.y);
        float x = Random.Range(minMaxAerialCircleWidthSpawn.x, minMaxAerialCircleWidthSpawn.y);

        e.Spawn(new Vector3(-12, yInit, 0), new Vector3(x, y, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(Vector3.up * (minMaxAerialHeightSpawn.x + ((minMaxAerialHeightSpawn.y - minMaxAerialHeightSpawn.x) / 2)) + (Vector3.right * (minMaxAerialWidthSpawn.x + ((minMaxAerialWidthSpawn.y - minMaxAerialWidthSpawn.x) / 2))),
        new Vector3(minMaxAerialWidthSpawn.y - minMaxAerialWidthSpawn.x, minMaxAerialHeightSpawn.y - minMaxAerialHeightSpawn.x, 0));
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube((Vector3.up * (minMaxGroundHeightSpawn.x + ((minMaxGroundHeightSpawn.y - minMaxGroundHeightSpawn.x) / 2))) + (Vector3.right * (minMaxGroundWidthSpawn.x + ((minMaxGroundWidthSpawn.y - minMaxGroundWidthSpawn.x) / 2))),
         new Vector3(minMaxGroundWidthSpawn.y - minMaxGroundWidthSpawn.x, minMaxGroundHeightSpawn.y - minMaxGroundHeightSpawn.x, 0));
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube((Vector3.up * (minMaxAerialCircleHeightSpawn.x + ((minMaxAerialCircleHeightSpawn.y - minMaxAerialCircleHeightSpawn.x) / 2))) + (Vector3.right * (minMaxAerialCircleWidthSpawn.x + ((minMaxAerialCircleWidthSpawn.y - minMaxAerialCircleWidthSpawn.x) / 2))),
         new Vector3(minMaxAerialCircleWidthSpawn.y - minMaxAerialCircleWidthSpawn.x, minMaxAerialCircleHeightSpawn.y - minMaxAerialCircleHeightSpawn.x, 0));
    }
}