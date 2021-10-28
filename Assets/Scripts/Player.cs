using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    [SerializeField] private int _wallDamage = 1;
    [SerializeField] private int _pointsPerFood = 10;
    [SerializeField] private int _pointsPerSoda = 20;
    [SerializeField] private float _restartLevelDelay = 1f;
    [SerializeField] private Text _foodText;
    [SerializeField] private AudioClip _moveSound1;
    [SerializeField] private AudioClip _moveSound2;
    [SerializeField] private AudioClip _eatSound1;
    [SerializeField] private AudioClip _eatSound2;
    [SerializeField] private AudioClip _drinkSound1;
    [SerializeField] private AudioClip _drinkSound2;
    [SerializeField] private AudioClip _gameOverSound1;


    private Animator _animator;
    private int _food;
    private Vector2 _touchOrigin = -Vector2.one;

    protected override void Start()
    {
        _animator = GetComponent<Animator>();
        _food = GameManager.Instance.playerFoodPoints;
        _foodText.text = "Food: " + _food;

        base.Start();
    }

    private void OnDisable()
    {
        GameManager.Instance.playerFoodPoints = _food;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.playersTurn)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }
#elif UNITY_ANDROID || PLATFORM_ANDROID
        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];
            if (myTouch.phase == TouchPhase.Began)
            {
                _touchOrigin = myTouch.position;
            }
            else if (myTouch.phase == TouchPhase.Ended && _touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - _touchOrigin.x;
                float y = touchEnd.y - _touchOrigin.y;
                _touchOrigin.x = -1;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    horizontal = x > 0 ? 1 : -1;
                }
                else
                {
                    vertical = y > 0 ? 1 : -1;
                }
            }
        }
#endif
        if (horizontal != 0 || vertical != 0)
        {
            AttempMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttempMove<T>(int xDir, int yDir)
    {
        _food--;
        _foodText.text = "Food: " + _food;

        base.AttempMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.Instance.RandomizeSfx(_moveSound1, _moveSound2);
        }

        CheckIfGameOver();

        GameManager.Instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Exit")
        {
            Invoke("Restart", _restartLevelDelay);
            enabled = false;
        }
        else if (collision.tag == "Food")
        {
            _food += _pointsPerFood;
            _foodText.text = "+" + _pointsPerFood + " Food: " + _food;
            SoundManager.Instance.RandomizeSfx(_eatSound1, _eatSound2);
            collision.gameObject.SetActive(false);
        }
        else if (collision.tag == "Soda")
        {
            _food += _pointsPerSoda;
            _foodText.text = "+" + _pointsPerSoda + " Food: " + _food;
            SoundManager.Instance.RandomizeSfx(_drinkSound1, _drinkSound2);
            collision.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(_wallDamage);
        _animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoseFood(int loss)
    {
        _animator.SetTrigger("playerHit");
        _food -= loss;
        _foodText.text = "-" + loss + " Food: " + _food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (_food <= 0)
        {
            SoundManager.Instance.PlaySingle(_gameOverSound1);
            SoundManager.Instance.musicSource.Stop();
            GameManager.Instance.GameOver();
        }
    }
}
