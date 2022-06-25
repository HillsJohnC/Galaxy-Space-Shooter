using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{    
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2;    
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _triProPrefab;
    [SerializeField]
    private float _fireRate = 0.25f;
    private float _canFire = -1f;    
    public int _playerAmmo;
    [SerializeField]
    private bool _isThereAmmo = true;

    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldsActive = false;
    private bool _isTriProActive = false;
    
    [SerializeField]
    private int _shieldLives;

    [SerializeField]
    private GameObject _shieldVisualizer;

    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
        
    [SerializeField]
    private int _score;

    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    
    private AudioSource _audioSource;
    
    SpriteRenderer shieldSprite;

        // Start is called before the first frame update
    void Start()
    {
        _playerAmmo = 25;
        shieldSprite = transform.Find ("Shields").GetComponentInChildren<SpriteRenderer>();

        if (shieldSprite == null)
        {
            Debug.LogError("Shields is NULL.");
        }

        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
       
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }        
    }

    // Update is called once per frame
    void Update()
    {         
          
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            if (_isThereAmmo == true)
            {
                FireLaser();
            }
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (_isSpeedBoostActive == false)
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * (_speed * _speedMultiplier) * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(direction * (_speed * _speedMultiplier) * Time.deltaTime);
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }


        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9.5f, 9.5f), Mathf.Clamp(transform.position.y, -3.8f, 0), 0);
    }

    void FireLaser()
           
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }

        else if (_isTriProActive == true)
        {
            Instantiate(_triProPrefab, transform.position, Quaternion.identity);
        }

        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.025f, 0), Quaternion.identity);
        }

        _audioSource.Play();
        _playerAmmo--;

        if (_playerAmmo < 1)
        {
            _isThereAmmo = false;
        }
        
    }

    public void Damage()
    {
        if (_isShieldsActive == true)
        {
            ShieldsDamage();
            return;
        }
        
        else
        {
            _lives--;
        }
        
        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

      if (_lives < 1)
      {            
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
      }
    }

    public void TripleShotActive()
    {
        _isTriProActive = false;
        _isTripleShotActive = true;        
        StartCoroutine(TripleShotPowerDownRoutine());
        _playerAmmo = 25;
        _isThereAmmo = true;
    }
    
 
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(8.0f);
        _isTripleShotActive = false;
    }

    public void TriProActive()
    {
        _isTripleShotActive = false;
        _isTriProActive = true;
        StartCoroutine(TriProPowerDownRoutine());
        _playerAmmo = 25;
        _isThereAmmo= true;
    }

    IEnumerator TriProPowerDownRoutine()
    {
        yield return new WaitForSeconds(8.0f);
        _isTriProActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(8.0f);
        _isSpeedBoostActive = false;
    }

    public void ShieldsActive()
    {
        _shieldLives = 3;
        shieldSprite.color = new Color(1f, 1f, 1f, 1f);
        _isShieldsActive = true;
        _shieldVisualizer.SetActive(true);
    }

    void ShieldNotActive()
    {
        _shieldLives = 0;
        _isShieldsActive=false;
        _shieldVisualizer.SetActive(false);
    }

    public void AmmoCollected()
    {
        _playerAmmo = 25;
        _isThereAmmo = true;
    }
    
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void HealthCollected()

    {
        if (_lives < 3)
        {
            _lives++;
            if (_lives == 3)
            {
                _leftEngine.SetActive(false);
            }            
            else if (_lives == 2)
            {
                _rightEngine.SetActive(false);
            }

            _uiManager.UpdateLives(_lives);
        }
        else
        {
            _lives = 3;
        }
    }

    public void ShieldsDamage()
    {

        _shieldLives--;

        if (_shieldLives == 2)
        {
            shieldSprite.color = new Color(1f, 1f, 1f, 0.66f);
        }

        else if (_shieldLives == 1)
        {
            shieldSprite.color = new Color(1f, 1f, 1f, 0.33f);
        }

        else if (_shieldLives < 1)
        {
            ShieldNotActive();
        }
        
    }
}
