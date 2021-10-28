using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] private GameObject _gameManager;

    void Awake()
    {
        if (GameManager.Instance == null)
        {
            Instantiate(_gameManager);
        }
    }
}
