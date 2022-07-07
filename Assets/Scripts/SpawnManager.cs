using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject[] _rarePowerups;

    private bool _stopSpawning = false;

 

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnRarePowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        while (_stopSpawning == false)
        {
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
