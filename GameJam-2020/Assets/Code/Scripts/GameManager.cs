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
    private bool tutorial;
    public bool Tutorial { set { tutorial = value; } }

    [BoxGroup("Tutorial"), SerializeField]
    private UIView repairText;
    [BoxGroup("Tutorial"), SerializeField]
    private UIView rechargeText;
    [BoxGroup("Tutorial"), SerializeField]
    private Turret tutorialTurret;

    [BoxGroup("Config Fields"), SerializeField]
    private float secondsToStart;
    [BoxGroup("Config Fields"), SerializeField]
    private UIView startText;

    enum GameState
    {
        Menus,
        GameStarting,
        GameStarted,
        GameFinished
    }
    private GameState gameState;

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
            StartCoroutine(PlayGame());
        }
    }

    public IEnumerator PlayTutorial()
    {
        repairText.Show();
        tutorialTurret.ReceiveDamage(50);
        yield return new WaitUntil(() => tutorialTurret.HealthRelative == 1);
        repairText.Hide();

        rechargeText.Show();
        yield return new WaitUntil(() => tutorialTurret.AmmunitionRelative == 1);
        rechargeText.Hide();

    }

    public IEnumerator PlayGame()
    {
        startText.Show();
        yield return new WaitForSeconds(secondsToStart);
        startText.Hide();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
