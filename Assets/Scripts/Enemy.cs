using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private GameObject _laserPrefab;
    private Player _player;
    private Animator _anim;
    private Collider2D _enemyCollider;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    public int _movePattern;
    private SpawnManager _spawnManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _movePattern = Random.Range(0, 3);
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }
       
        if (_player == null)
        {
            Debug.LogError("The Player is NULL");
        }
        
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("The animator is NULL");
        }

        _enemyCollider = GetComponent<Collider2D>();

        if (_enemyCollider == null)
        {
            Debug.LogError("The Collider2D is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);       
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
          
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    public void CalculateMovement()
    {
        switch (_movePattern)
        {
            case 0:
                transform.Translate(new Vector3(Mathf.Cos(Time.time), -1, 0) * _speed * Time.deltaTime);
                break;
            case 1:
                transform.Translate(new Vector3(Mathf.Cos(Time.time), Mathf.Sin(Time.time) - .1f, 0) * _speed * Time.deltaTime);
                break;
            case 2:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                break;
            default:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                break;
        }
        

        if (_enemyCollider.enabled == false && transform.position.y < -5f)
        {
            Destroy(this.gameObject);
        }

        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _player != null)
        {
            _player.Damage();
            EnemyDestroyed();
        }      

        if (other.tag == "Laser" && _player != null)
        {
            Destroy(other.gameObject);
            _player.AddScore(10);
            EnemyDestroyed();
        }
    }

    private void EnemyDestroyed()
    {
        _enemyCollider.enabled = false;
        _anim.SetTrigger("OnEnemyDeath");
        _audioSource.Play();
        _spawnManager.enemiesKilled++;
        Destroy(this.gameObject, 2.8f);
    }
}
