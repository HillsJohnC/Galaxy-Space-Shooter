using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
    [SerializeField] private GameObject _enemyBoss1Prefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private int _numberOfEnemies = 5;
    [SerializeField] private int _enemiesSpawned = 0;
    private bool _stopSpawning = false;
    private bool _isWaveInitiating = true;
    public int waveCount = 1;
    private int _nextBoss = 1;
    public int enemiesKilled;
    private UIManager _uiManager;
    private Player _player;
    public int[] enemyTable =
    {
        700,        // Enemy 1
        200         // Enemy 2
    };

    public int enemyTotal;
    public int randomEnemyNumber;
    private bool _enemySpawnRestart;

    public int[] powerupTable =
    {
        300,        // Ammo
        250,        // Speed Boost
        210,        // Shields
        170,        // Health
        120,        // Triple Shot
        100,         // No Ammo
        85,         // Tri Pro (Triple Shot Pro)
        45          // Heat Seeking Missle
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

    public void Wave()
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
            waveCount++;
            _isWaveInitiating = true;
            StartSpawning();
        }
    }    

    public void StartSpawning()
    {
        if (_player != null)
        {

            if (waveCount >= 4)
            {
                StartCoroutine(AnnounceBoss());
                StartCoroutine(SpawnPowerupRoutine());
            }

            if (waveCount < 4)
            {

                if (_isWaveInitiating != false)
                {
                    StartCoroutine(AnnounceWave());
                }
                else
                {
                    _stopSpawning = false;
                    StartCoroutine(SpawnEnemyRoutine());
                    StartCoroutine(SpawnPowerupRoutine());
                }
            }
        }
    }

    IEnumerator AnnounceBoss()
    {
        _uiManager.UpdateBoss(_nextBoss);
        yield return new WaitForSeconds(1.0f);
        _uiManager._announceBoss.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        _uiManager._announceBoss.gameObject.SetActive(false);
        StartCoroutine(SpawnBoss());
    }

    IEnumerator SpawnBoss()
    {
        yield return new WaitForSeconds(2.4f);
        Vector3 bossSpawnPosition = new Vector3(0f, 11.0f, 0f);
        GameObject newBoss = Instantiate(_enemyBoss1Prefab, bossSpawnPosition, Quaternion.identity);
        _uiManager.ShowBossHitPointsText();
        newBoss.transform.parent = _enemyContainer.transform;
    }

    IEnumerator AnnounceWave()
    {
        _uiManager.UpdateWave(waveCount);
        yield return new WaitForSeconds(1.0f);
        _uiManager._announceWave.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        _uiManager._announceWave.gameObject.SetActive(false);
        _isWaveInitiating = false;
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
            yield return new WaitForSeconds(4.0f);
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
        while (_stopSpawning == false || (waveCount >= 4 && _enemyBoss1Prefab != null))
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

    public void OnBossDeath()
    {
        StopAllCoroutines();        
    }
}
