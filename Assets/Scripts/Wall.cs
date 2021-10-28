using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private Sprite _dmgSprite;
    [SerializeField] private int _hp = 4;
    [SerializeField] private AudioClip _chopSound1;
    [SerializeField] private AudioClip _chopSound2;

    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss)
    {
        SoundManager.Instance.RandomizeSfx(_chopSound1, _chopSound2);
        _spriteRenderer.sprite = _dmgSprite;
        _hp -= loss;

        if (_hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
