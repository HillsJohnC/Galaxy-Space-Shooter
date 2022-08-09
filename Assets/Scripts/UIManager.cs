using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Player _player;
    [SerializeField] private Text _scoreText;    
    [SerializeField] public Text _totalAmmoText;
    [SerializeField] private Image _LivesImg;
    [SerializeField] private Sprite[] _liveSprites;
    [SerializeField] private Sprite[] _thrusterBarSprites;
    [SerializeField] private Image _thrusterBarImg;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    [SerializeField] public Text _initiateWave;
    private GameManager _gameManager;


    // Start is called before the first frame update
    void Start()
    {
        _totalAmmoText.text = "Ammo: " + 25 + " / 75";
        _player = GameObject.Find("Player").GetComponent<Player>();
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _initiateWave.gameObject.SetActive(false);

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }
    }

    public void InitiateWave(int nextWave)
    {
        _initiateWave.text = "Wave: " + nextWave.ToString();
    }

    public void UpdateAmmo(int playerAmmo)
    {
        _totalAmmoText.text = "Ammo: " + playerAmmo + " / 75";
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }
    public void UpdateThrusterBar(float currentThrusterValue)
    {
            _thrusterBarImg.sprite = _thrusterBarSprites[(int) currentThrusterValue];
    }
    
    
    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }


    IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }

    }


}
