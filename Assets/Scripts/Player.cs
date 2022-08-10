using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] public CameraShake cameraShake;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _triProPrefab;
    [SerializeField] private GameObject _heatSeeker;
    [SerializeField] private float _fireRate = 0.25f;
    [SerializeField] private bool _isThereAmmo = true;
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _shieldLives;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _leftEngine, _rightEngine;        
    [SerializeField] private int _score;
    [SerializeField] private AudioClip _laserSoundClip;
    private int _maxAmmo = 75;
    private float _speedMultiplier = 2;
    private float _thrusterValue = 1f;    
    private float _canFire = -1f;    
    private int _playerAmmo = 25;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    private bool _isShieldsActive = false;
    private bool _isTriProActive = false;
    public bool isPowerupAttractActive = false;
    private bool _isHeatSeekerActive = false;
    private UIManager _uiManager;       
    private AudioSource _audioSource;
    
    SpriteRenderer shieldSprite;

        // Start is called before the first frame update
    void Start()
    {        
        shieldSprite = transform.Find ("Shields").GetComponentInChildren<SpriteRenderer>();
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (shieldSprite == null)
        {
            Debug.LogError("Shields is NULL.");
        }
       
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
                FireWeapon();
            }
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKey(KeyCode.LeftShift) && (_thrusterValue < 12f))
        {
            transform.Translate(direction * (_speed * _speedMultiplier) * Time.deltaTime);
            ThrusterTimer();
        }

        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                ThrusterRegen();
            }
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9.5f, 9.5f), Mathf.Clamp(transform.position.y, -3.15f, 2f), 0);
        if (Input.GetKey(KeyCode.C))
        {
            isPowerupAttractActive = true;
        }
        else
        {
            isPowerupAttractActive = false;
        }
    }

    void ThrusterTimer()
    {
        if (_thrusterValue > 0f && _thrusterValue < 12f)
        {
            _uiManager.UpdateThrusterBar(_thrusterValue);
            _thrusterValue += .04f;
        }
    }

    void ThrusterRegen()
    {
        if (_thrusterValue > 1f)
        {
        _uiManager.UpdateThrusterBar(_thrusterValue);
        _thrusterValue -= .008f;
        }
    }

    void FireWeapon()
           
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            _playerAmmo--;
        }

        else if (_isTriProActive == true)
        {
            Instantiate(_triProPrefab, transform.position, Quaternion.identity);
            _playerAmmo--;
        }

        else if (_isHeatSeekerActive == true)
        {
            Instantiate(_heatSeeker, transform.position + new Vector3(0, 2.84f, 0), Quaternion.identity);
            _playerAmmo--;
        }

        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.025f, 0), Quaternion.identity);
            _playerAmmo--;
        }

        _audioSource.Play();

        if (_playerAmmo < 1)
        {
            _isThereAmmo = false;
            _uiManager._totalAmmoText.color = Color.red;            
        }
        else
        {
            _uiManager._totalAmmoText.color = Color.white;
        }

        _uiManager.UpdateAmmo(_playerAmmo);
        
    }

    public void Damage()
    {
        if (_lives > 1)
        {
            StartCoroutine(cameraShake.Shake(.15f, .4f));
        }        

        if (_isShieldsActive == true)
        {
            ShieldsDamage();
            return;
        }       
            _lives--;        

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
        _playerAmmo += 6;
        _isTriProActive = false;
        _isHeatSeekerActive = false;
        _isTripleShotActive = true;        
        MaxAmmo();
        StartCoroutine(TripleShotPowerDownRoutine()); 
        _isThereAmmo = true;
    }
    
 
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(8.0f);
        _isTripleShotActive = false;
    }

    public void TriProActive()
    {
        _playerAmmo += 6;
        _isTripleShotActive = false;
        _isHeatSeekerActive = false;
        _isTriProActive = true;
        MaxAmmo();
        StartCoroutine(TriProPowerDownRoutine());
        _isThereAmmo = true;
    }

    IEnumerator TriProPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTriProActive = false;
    }

    public void HeatSeekerCollected()
    {
        _playerAmmo += 6;
        _isTriProActive = false;
        _isTripleShotActive = false;
        _isHeatSeekerActive = true;
        MaxAmmo();
        StartCoroutine(HeatSeekerPowerDownRoutine());
        _isThereAmmo = true;
    }

    IEnumerator HeatSeekerPowerDownRoutine()
    {
        yield return new WaitForSeconds(8.0f);
        _isHeatSeekerActive = false;
    }

    public void SpeedBoostActive()
    {
        _thrusterValue = 1;
        _uiManager.UpdateThrusterBar(_thrusterValue);
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
        _playerAmmo += 25;
        _isThereAmmo = true;
        MaxAmmo();
    }

    public void MaxAmmo()
    {
        if (_playerAmmo > _maxAmmo)
        {
            _playerAmmo = _maxAmmo;
        }
        _uiManager.UpdateAmmo(_playerAmmo);
        _uiManager._totalAmmoText.color = Color.white;
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

    public void AmmoDepleted()
    {
        _playerAmmo = 0;
        _uiManager.UpdateAmmo(_playerAmmo);
        _isThereAmmo = false;
        _uiManager._totalAmmoText.color = Color.red;
    }
}
