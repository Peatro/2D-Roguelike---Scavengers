using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable] private class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    [SerializeField] private int _columns = 8;
    [SerializeField] private int _rows = 8;
    [SerializeField] private Count _wallCount = new Count(5, 9);
    [SerializeField] private Count _foodCount = new Count(1, 5);
    [SerializeField] private GameObject _exit;
    [SerializeField] private GameObject[] _floorTiles;
    [SerializeField] private GameObject[] _wallTiles;
    [SerializeField] private GameObject[] _foodTiles;
    [SerializeField] private GameObject[] _enemyTiles;
    [SerializeField] private GameObject[] _outerWallTiles;

    private Transform _boardHolder;
    private List<Vector3> _gridPositions = new List<Vector3>();

    private void InitialiseList()
    {
        _gridPositions.Clear();

        for (int x = 1; x < _columns - 1; x++)
        {
            for (int y = 1; y < _rows - 1; y++)
            {
                _gridPositions.Add(new Vector3(x, y, 0));
            }
        }
    }

    private void BoardSetup()
    {
        _boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < _columns + 1; x++)
        {
            for (int y = -1; y < _rows + 1; y++)
            {
                GameObject toInstantiate = _floorTiles[Random.Range(0, _floorTiles.Length)];
                if (x == -1 || x == _columns || y == -1 || y == _rows)
                {
                    toInstantiate = _outerWallTiles[Random.Range(0, _outerWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                instance.transform.SetParent(_boardHolder);
            }
        }
    }

    private Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, _gridPositions.Count);
        Vector3 randomPosition = _gridPositions[randomIndex];
        _gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(_wallTiles, _wallCount.minimum, _wallCount.maximum);
        LayoutObjectAtRandom(_foodTiles, _foodCount.minimum, _foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(_enemyTiles, enemyCount, enemyCount);
        Instantiate(_exit, new Vector3(_columns - 1, _rows - 1, 0), Quaternion.identity);
    }
}
