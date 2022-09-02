using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss1 : MonoBehaviour
{
    [SerializeField] private GameObject _bossMinePrefab;
    [SerializeField] private GameObject _enemyLaser2Prefab;
    [SerializeField] private GameObject _laserBeamPrefabRight;
    [SerializeField] private GameObject _laserBeamPrefabLeft;
    [SerializeField] private int _bossHitPoints = 100;
    private bool _canMove = true;
    private float _bossSpeed = 2.0f;
    private float _canFireLaser = 5f;
    private float _canFireLaserBeam = -1f;
    private float _canFireMines = 7f;
    private float _fireRateLasers = 5f;
    private float _fireRateLaserBeam;
    private float _fireRateMines;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private Player _player;
    private Collider2D _bossCollider;
    private Vector3 _newPos;
    private AudioSource _audioSource;
    private Animator _anim;

    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _bossCollider = GetComponent<Collider2D>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _newPos = new Vector3(0f, 3f, 0f);

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }

        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UIManager is NULL");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is NULL");
        }

        _canFireLaser = Time.time + 10f;
        _canFireLaserBeam = Time.time + 7.5f;
        _canFireMines = Time.time + 6f;
    }

    void Update()
    {
        if (transform.position.y == 3.0f)
        {
            if (Time.time > _canFireLaserBeam)
            {
                _fireRateLaserBeam = Random.Range(3.0f, 9.0f);
                _canFireLaserBeam = Time.time + _fireRateLaserBeam;
                LaserBeamFire();
            }


            MineCooldown();
            LaserCooldown();
        }

        if (_canMove == true)
        {
            Movement();
        }
    }

    void Movement()
    {
        transform.position = Vector3.MoveTowards(transform.position, _newPos, _bossSpeed * Time.deltaTime);

        if (transform.position == _newPos)
        {
            _canMove = false;
            StartCoroutine(NextMovement());
        }
    }

    IEnumerator NextMovement()
    {
        yield return new WaitForSeconds(0.25f);
        _newPos = new Vector3(Random.Range(-6.48f, 6.48f), 3f, 0f);
        _bossSpeed = 3.0f;
        _canMove = true;
    }

    void LaserCooldown()
    {
        if (Time.time > _canFireLaser)
        {
            _canFireLaser = Time.time + _fireRateLasers;
            LaserFire();
        }
    }

    void LaserFire()
    {
        GameObject _enemyLaser2 = Instantiate(_enemyLaser2Prefab, transform.position, Quaternion.identity);
        Laser[] lasers = _enemyLaser2.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    IEnumerator BossLaserBeamCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        _laserBeamPrefabRight.SetActive(false);
        _laserBeamPrefabLeft.SetActive(false);
    }

    void LaserBeamFire()
    {
        _laserBeamPrefabRight.SetActive(true);
        _laserBeamPrefabLeft.SetActive(true);
        StartCoroutine(BossLaserBeamCooldown());
    }

    void MineCooldown()
    {
        if (Time.time > _canFireMines)
        {
            _fireRateMines = Random.Range(7f, 10f);
            _canFireMines = Time.time + _fireRateMines;
            DropMine();
        }
    }

    void DropMine()
    {
        GameObject _bossMine = Instantiate(_bossMinePrefab, transform.position, Quaternion.identity);
        Destroy(_bossMine, 3.0f);
    }

    public void ReduceBossHitPoints(int hitPoints)
    {
        _bossHitPoints -= hitPoints;
        _uiManager.UpdateBossHitPoints(_bossHitPoints);
    }

    private void BossDestroyed()
    {
        _spawnManager.OnBossDeath();
        _bossCollider.enabled = false;
        _anim.SetTrigger("OnBossDeath");
        _audioSource.Play();
        Destroy(this.gameObject, 2.0f);
        _uiManager.YouWinSequence();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_player != null)
        {
            if (other.CompareTag("Laser"))
            {
                if (this.gameObject != null)
                {
                    ReduceBossHitPoints(2);
                    _player.AddScore(2);
                    Destroy(other.gameObject);
                }
            }

            if (other.CompareTag("HeatSeeker"))
            {
                if (this.gameObject != null)
                {
                    ReduceBossHitPoints(5);
                    _player.AddScore(5);
                    Destroy(other.gameObject);
                }
            }

            if (other.CompareTag("Player"))
            {
                if (this.gameObject != null)
                {
                    ReduceBossHitPoints(1);
                    _player.AddScore(1);
                    _player.Damage();
                }
            }

            if (_bossHitPoints <= 0)
            {
                _bossHitPoints = 0;
                BossDestroyed();
            }
        }
    }
}
