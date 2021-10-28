using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                //Debug.LogError("Game Manager is null!");
            }
            return _instance;
        }
    }

    [SerializeField] private float _turnDelay = 0.1f;
    [SerializeField] private float _levelStarDelay = 2.0f;
    [HideInInspector] public bool playersTurn = true;

    private Text _levelText;
    private GameObject _levelImage;
    private List<Enemy> _enemies;    
    private BoardManager _boardScript;
    private int _level = 1;
    private bool _enemiesMoving;
    private bool _doingSetup;

    public int playerFoodPoints = 100;

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;

        DontDestroyOnLoad(gameObject);

        _enemies = new List<Enemy>();
        _boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    
    private void OnLevelWasLoaded(int level)
    {
        _level++;
        InitGame();
    }

    private void InitGame()
    {
        _doingSetup = true;
        _levelImage = GameObject.Find("LevelImage");
        _levelText = GameObject.Find("LevelText").GetComponent<Text>();
        _levelText.text = "Day " + _level;
        _levelImage.SetActive(true);
        Invoke("HideLevelImage", _levelStarDelay);
        _enemies.Clear();
        _boardScript.SetupScene(_level);
    }

    private void HideLevelImage()
    {
        _levelImage.SetActive(false);
        _doingSetup = false;
    }

    public void GameOver()
    {
        _levelText.text = "After " + _level + " days, you have starved.";
        _levelImage.SetActive(true);
        enabled = false;
    }

    void Update()
    {
        if (playersTurn || _enemiesMoving || _doingSetup)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        _enemies.Add(script);
    }

    private IEnumerator MoveEnemies()
    {
        _enemiesMoving = true;
        yield return new WaitForSeconds(_turnDelay);
        if (_enemies.Count == 0)
        {
            yield return new WaitForSeconds(_turnDelay);
        }

        for(int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].MoveEnemy();
            yield return new WaitForSeconds(_enemies[i].moveTime);
        }

        playersTurn = true;
        _enemiesMoving = false;
    }
}
