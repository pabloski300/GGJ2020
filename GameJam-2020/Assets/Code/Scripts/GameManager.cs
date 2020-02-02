using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField, BoxGroup("Main Fields")]
    private GameObject player;
    [SerializeField, BoxGroup("Main Fields")]
    private int turretAmount;
    [SerializeField, BoxGroup("Main Fields")]
    private Transform spawnPosition;

    private Spawner spawner;
    private bool tutorial = true;
    public bool Tutorial { set { tutorial = value; } }

    [BoxGroup("Tutorial"), SerializeField]
    private List<UIView> repairTexts;
    [BoxGroup("Tutorial"), SerializeField]
    private List<UIView> rechargeTexts;
    [BoxGroup("Tutorial"), SerializeField]
    private Turret tutorialTurret;

    [BoxGroup("Config Fields"), SerializeField]
    private float secondsToStart;
    [BoxGroup("Config Fields"), SerializeField]
    private UIView startText;
    [BoxGroup("Config Fields"), SerializeField]
    private UIView endGameView;
    [BoxGroup("Config Fields"), SerializeField]
    private UIView titleView;
    [BoxGroup("Config Fields"), SerializeField]
    private UIView startView;
    [BoxGroup("Config Fields"), SerializeField]
    private UIView tutorialView;
        [BoxGroup("Config Fields"), SerializeField]
    private Parallax parallax;

            [BoxGroup("Sounds"), SerializeField]
    private Sound ambient;

    
            [BoxGroup("Sounds"), SerializeField]
    private Sound main;

    private int enemyNumber = 0;
    private float timeToWait;

    public enum GameState
    {
        Menus,
        GameStarting,
        GameStarted,
        GameFinished
    }
    private GameState gameState;
    public GameState CurrentGameState {get{return gameState;}}

    public static GameManager Instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
        spawner = FindObjectOfType<Spawner>();
        ambient.Init();
        ambient.Play(this.transform);
        main.Init();
        main.Play(this.transform);
        gameState = GameState.Menus;
    }

    public void StartGame()
    {
        gameState = GameState.GameStarting;
        Instantiate(player, spawnPosition.transform.position, Quaternion.Euler(0, 0, 0));
        if (tutorial)
        {
            StartCoroutine(PlayTutorial());
        }
        else
        {
            PlayGame();
        }
    }

    public IEnumerator PlayTutorial()
    {
        gameState = GameState.GameStarted;
        foreach (UIView repairText in repairTexts)
        {
            repairText.Show();
        }
        tutorialTurret.ReceiveDamage(50);
        yield return new WaitUntil(() => tutorialTurret.HealthRelative == 1);
        foreach (UIView repairText in repairTexts)
        {
            repairText.Hide();
        }

        foreach (UIView rechargeText in rechargeTexts)
        {
            rechargeText.Show();
        }
        tutorialTurret.RemoveBullets(10);
        yield return new WaitUntil(() => tutorialTurret.AmmunitionRelative == 1);
        foreach (UIView rechargeText in rechargeTexts)
        {
            rechargeText.Hide();
        }

    }

    public void PlayGame()
    {
        gameState = GameState.GameStarted;
        startText.Show();
    }

    public void StartSpawner(){
        StartCoroutine(WaitForNextWave());
    }

    public void WaveStarted(float _timeToWait){
        timeToWait = _timeToWait;
    }

    public void EnemySpawned() {
        enemyNumber++;
    }
    public void EnemyKilled(){
        enemyNumber--;
        if(enemyNumber == 0){
            StartCoroutine(WaitForNextWave());
        }
    }

    public void TurretKilled(){
        turretAmount--;
        if(turretAmount == 0){
            LoseGame();
        }
    }

    public IEnumerator WaitForNextWave() {
        yield return new WaitForSeconds(timeToWait);
        StartCoroutine(spawner.Spawn());
    }

    public void LoseGame(){
        gameState = GameState.GameFinished;
        endGameView.Show();
    }

    public void RestartGame(){
        timeToWait = 0;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach(Enemy e in  enemies){
            e.Destroy();
        }
        PlayerController p = FindObjectOfType<PlayerController>();
        Destroy(p.gameObject);
        Turret[] turrets = FindObjectsOfType<Turret>();
        foreach(Turret t in turrets){
            t.Init();
            turretAmount++;
        }
        endGameView.Hide();
        titleView.Show();
        startView.Show();
        tutorialView.Show();
        gameState = GameState.Menus;
        parallax.MasterSpeed = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
