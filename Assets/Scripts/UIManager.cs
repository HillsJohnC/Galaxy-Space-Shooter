using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{    
    [SerializeField] private Text _bossHitPointsText;
    [SerializeField] private Text _scoreText;
    [SerializeField] public Text _totalAmmoText;
    [SerializeField] private Image _LivesImg;
    [SerializeField] private Sprite[] _liveSprites;
    [SerializeField] private Sprite[] _thrusterBarSprites;
    [SerializeField] private Image _thrusterBarImg;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    [SerializeField] public Text _announceWave;
    [SerializeField] public Text _announceBoss;
    [SerializeField] private Text _youWinText;
    private GameManager _gameManager;
    private Player _player;



    void Start()
    {
        _bossHitPointsText.text = "Boss Hit Points: " + 50 + " / 50";
        _totalAmmoText.text = "Ammo: " + 25 + " / 75";
        _player = GameObject.Find("Player").GetComponent<Player>();
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _announceWave.gameObject.SetActive(false);
        _announceBoss.gameObject.SetActive(false);

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }

        if (_player = null)
        {
            Debug.LogError("Player is NULL in UIManager");
        }
    }

    public void UpdateBoss(int _nextBoss)
    {
        _announceBoss.text = "BOSS " + _nextBoss.ToString() + "\n" + "AIM FOR THE FACE!!!";
    }

    public void UpdateWave(int nextWave)
    {
        _announceWave.text = "Wave: " + nextWave.ToString();
    }

    public void UpdateAmmo(int playerAmmo)
    {
        _totalAmmoText.text = "Ammo: " + playerAmmo + " / 75";
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void ShowBossHitPointsText()
    {
        _bossHitPointsText.gameObject.SetActive(true);
    }

    public void UpdateBossHitPoints(int bossHitPoints)
    {
        _bossHitPointsText.text = "Boss Hit Points: " + bossHitPoints + " / 50";
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
        _thrusterBarImg.sprite = _thrusterBarSprites[(int)currentThrusterValue];
    }

    public void YouWinSequence()
    {
        _youWinText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(YouWinFlickerRoutine());
    }

    public void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }

    }

    IEnumerator YouWinFlickerRoutine()
    {
        while (true)
        {
            _youWinText.text = "YOU WIN!!!";
            yield return new WaitForSeconds(0.5f);
            _youWinText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
