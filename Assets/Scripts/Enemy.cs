using UnityEngine;

public class Enemy : MovingObject
{
    [SerializeField] private int _playerDamage;
    [SerializeField] private AudioClip _enemyAttack1;
    [SerializeField] private AudioClip _enemyAttack2;

    private Animator _animator;
    private Transform _target;
    private bool _skipMove;

    protected override void Start()
    {
        GameManager.Instance.AddEnemyToList(this);
        _animator = GetComponent<Animator>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttempMove<T>(int xDir, int yDir)
    {
        if (_skipMove == true)
        {
            _skipMove = false;
            return;
        }
        base.AttempMove<T>(xDir, yDir);

        _skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = _target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = _target.position.x > transform.position.x ? 1 : -1;
        }
        AttempMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;        
        hitPlayer.LoseFood(_playerDamage);
        _animator.SetTrigger("enemyAttack");
        SoundManager.Instance.RandomizeSfx(_enemyAttack1, _enemyAttack2);
    }
}
