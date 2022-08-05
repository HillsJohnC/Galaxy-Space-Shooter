using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _enemyShieldVisualizer;
    [SerializeField] private int _movePattern;
    [SerializeField] private float _enemyAgroDistance = 3.2f;
    [SerializeField] private float _enemyAgroSpeed = 2f;
    [SerializeField] private GameObject _backFire;
    [SerializeField] private float _backFireDistance = 6f;
    private SpriteRenderer _spriteRenderer;
    private Player _player;
    private Animator _anim;
    private Collider2D _enemyCollider;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    private SpawnManager _spawnManager;
    private int _randomEnemyShield;
    private bool _isEnemyShieldActive = false;
    
    // Start is called before the first frame update
    void Start()
    {
        EnemyShield();
        _movePattern = Random.Range(1, 3);
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

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
        if (_player == null)
        {
            Destroy(this.gameObject);
        }
        CalculateMovement();
        CanFire();
    }

    private void EnemyShield()
    {
        _randomEnemyShield = Random.Range(0, 7);

        if (_randomEnemyShield > 5)
        {
            _isEnemyShieldActive = true;
            _enemyShieldVisualizer.SetActive(true);
        }
        else
        {
            _isEnemyShieldActive = false;
            _enemyShieldVisualizer.SetActive(false);
        }
    }

    private void CanFire()
    {
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

            if (Vector3.Distance(transform.position, _player.transform.position) <= _backFireDistance)
            {
                if (transform.position.y < _player.transform.position.y)
                {
                    GameObject backFire = Instantiate(_backFire, transform.position, Quaternion.identity);
                    Laser laser = backFire.GetComponent<Laser>();
                    laser.AssignBackFire();
                }
            }
        }
    }

    public void CalculateMovement()
    {
        switch (_movePattern)
        {
            case 0:
                transform.Translate(new Vector3(Mathf.Cos(Time.time), 1, 0) * _speed * Time.deltaTime);
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

        if (_player != null)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) <= _enemyAgroDistance)
            {
                if (transform.position.y > _player.transform.position.y)
                {
                    StartCoroutine(AgroColor());
                    transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _enemyAgroSpeed * Time.deltaTime);
                }
                else
                {
                    StopCoroutine(AgroColor());
                    _spriteRenderer.color = Color.white;
                }
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
    }

    IEnumerator AgroColor()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(1.5f);
        _spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(1.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _player != null)
        {
            EnemyDestroyed();
            _player.Damage();            
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
        if (_isEnemyShieldActive == true)
        {
            _isEnemyShieldActive = false;
            _enemyShieldVisualizer.SetActive(false);
        }
        else
        {
            _enemyCollider.enabled = false;
            _anim.SetTrigger("OnEnemyDeath");
            _audioSource.Play();
            _spawnManager.enemiesKilled++;
            Destroy(this.gameObject, 2.8f);
        }
    }
}
