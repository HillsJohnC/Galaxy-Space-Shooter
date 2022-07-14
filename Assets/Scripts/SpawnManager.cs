using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject[] _rarePowerups;
    private bool _stopSpawning = false;
    private bool _isWaveInitiated = true;
    private int _waveCount = 1;
    [SerializeField] private int _numberOfEnemies = 10;
    [SerializeField] private int _enemiesSpawned = 0;
    public int enemiesKilled;
    private UIManager _uiManager;
    private Player _player;

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
    }


    private void Update()
    {
        Wave();
    }

    private void Wave()
    {
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
            StartCoroutine(SpawnRarePowerupRoutine());
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
            _enemiesSpawned++;
            Vector3 posToSpawn = new Vector3(Random.Range(-8.5f, 8.5f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(4.0f);
        while(_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8.5f, 8.5f), 7, 0);
            int randomPowerUp = Random.Range(0, 6);
            Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(4, 8));
        }
    }

    IEnumerator SpawnRarePowerupRoutine()
    {
        yield return new WaitForSeconds(20.0f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8.5f, 8.5f), 7, 0);
            int rareRandomPowerUp = Random.Range(0, 1);
            Instantiate(_rarePowerups[rareRandomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(15, 20));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
