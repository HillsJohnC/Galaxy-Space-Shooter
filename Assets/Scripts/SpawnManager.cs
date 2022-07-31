using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private int _numberOfEnemies = 10;
    [SerializeField] private int _enemiesSpawned = 0;
    private bool _stopSpawning = false;
    private bool _isWaveInitiated = true;
    private int _waveCount = 1;
    public int enemiesKilled;
    private UIManager _uiManager;
    private Player _player;
    public int[] enemyTable =
    {
        950,        // Enemy 1
        50         // Enemy 2
    };
    public int enemyTotal;
    public int randomEnemyNumber;
    private bool _enemySpawnRestart;

    public int[] powerupTable =
    {
        250,        // Ammo
        180,        // Speed Boost
        150,        // Shields
        135,        // Health
        105,        // Triple Shot
        95,         // No Ammo
        45          // Tri Pro (Triple Shot Pro)
    };
    public int powerupTotal;
    public int randomPowerupNumber;
    private bool _powerupSpawnRestart;

    private void Start()
    {        
        _player = GameObject.Find("Player").GetComponent<Player>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL!");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UIManager is NULL!!");
        }

        foreach (var item in powerupTable)
        {
            powerupTotal += item;
        }

        foreach (var item in enemyTable)
        {
            enemyTotal += item;
        }
    }


    private void Update()
    {
        Wave();
        RestartSpawnPowerupRoutine();
        RestartSpawnEnemyRoutine();
    }

    private void Wave()
    {
        if (_enemiesSpawned == _numberOfEnemies)
        {
            StopCoroutine(SpawnEnemyRoutine());
        }

        if (enemiesKilled >= _numberOfEnemies)
        {
            StopAllCoroutines();
            enemiesKilled = 0;
            _enemiesSpawned = 0;
            _numberOfEnemies = (int)Mathf.Round(_numberOfEnemies + _numberOfEnemies / 1.5f);
            _waveCount++;
            _isWaveInitiated = true;
            StartSpawning();
        }
    }


    public void StartSpawning()
    {
        if (_isWaveInitiated != false && _player != null)
        {
            StartCoroutine(InitiateWave());
        }
        else
        {
            StartCoroutine(SpawnEnemyRoutine());
            StartCoroutine(SpawnPowerupRoutine());
        }
    }

    IEnumerator InitiateWave()
    {
        _uiManager.InitiateWave(_waveCount);
        yield return new WaitForSeconds(1.0f);
        _uiManager._initiateWave.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        _uiManager._initiateWave.gameObject.SetActive(false);
        _isWaveInitiated = false;
        StartSpawning();
    }

    IEnumerator SpawnEnemyRoutine()
    {        
        while (_stopSpawning == false && _enemiesSpawned < _numberOfEnemies)
        {
            yield return new WaitForSeconds(2.3f);
            randomEnemyNumber = Random.Range(0, enemyTotal);



            for (int i = 0; i < enemyTable.Length; i++)
            {
                if (randomEnemyNumber <= enemyTable[i])
                {
                    _enemiesSpawned++;
                    Vector3 posToSpawn = new Vector3(Random.Range(-8.5f, 8.5f), 7, 0);
                    GameObject newEnemy = Instantiate(_enemyPrefab[i], posToSpawn, Quaternion.identity);
                    newEnemy.transform.parent = _enemyContainer.transform;
                    _enemySpawnRestart = true;
                    yield break;
                }
                else
                {
                    randomEnemyNumber -= enemyTable[i];
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private void RestartSpawnEnemyRoutine()
    {
        if (_enemySpawnRestart == true)
        {
            _enemySpawnRestart = false;
            StartCoroutine(SpawnEnemyRoutine());
        }            
    }

    IEnumerator SpawnPowerupRoutine()
    {        
        while(_stopSpawning == false)
        {
            yield return new WaitForSeconds(4.0f);

            randomPowerupNumber = Random.Range(0, powerupTotal);

            for (int i = 0; i < powerupTable.Length; i++)
            {
                if (randomPowerupNumber <= powerupTable[i])
                {
                    Vector3 posToSpawn = new Vector3(Random.Range(-8.5f, 8.5f), 7, 0);                    
                    Instantiate(_powerups[i], posToSpawn, Quaternion.identity);
                    _powerupSpawnRestart = true;
                    yield break;
                }
                else
                {
                    randomPowerupNumber -= powerupTable[i];
                }
            }
            yield return new WaitForSeconds(Random.Range(4, 8));
        }
    }

    private void RestartSpawnPowerupRoutine()
    {
        if (_powerupSpawnRestart == true)
        {
            _powerupSpawnRestart = false;
            StartCoroutine(SpawnPowerupRoutine());
        }
    }
    
    public void OnPlayerDeath()
    {
        StopAllCoroutines();
    }
}
