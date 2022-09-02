using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMine : MonoBehaviour
{
    private float _mineSpeed = 3f;
    private bool _blownUp;
    private AudioSource _audioSource;
    private Animator _anim;
    Player _player;
    SpriteRenderer _spriteRend;    
    Vector3 _playerPos;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _spriteRend = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        if (_anim == null)
        {
            Debug.LogError("Animator is NULL");
        }

        if (_spriteRend == null)
        {
            Debug.LogError("SpriteRenderer is NULL");
        }
    }

    void Update()
    {
        if (_blownUp)
        {
            _mineSpeed = 0f;
        }
        else
        {
            _playerPos = _player.transform.position;
            Vector3 direction = transform.position - _playerPos;
            direction = -direction.normalized;
            transform.position += _mineSpeed * Time.deltaTime * direction;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {


        if (other.tag == "Player" && _player != null)
        {
            _anim.SetTrigger("OnMineCollision");
            _audioSource.Play();
            _blownUp = true;
            _player.Damage();
            Destroy(gameObject, 2.7f);
            Destroy(GetComponent<Collider2D>());
            _player.AddScore(5);
        }

        if (other.tag == "Shields" && _player != null)
        {
            _anim.SetTrigger("OnMineCollision");
            _audioSource.Play();
            _blownUp = true;
            Destroy(gameObject, 2.7f);
            Destroy(GetComponent<Collider2D>());
            _player.AddScore(5);
        }

        if (other.tag == "Laser" && _player != null)
        {
            _anim.SetTrigger("OnMineCollision");
            _audioSource.Play();
            _blownUp = true;
            Destroy(gameObject, 2.7f);
            Destroy(GetComponent<Collider2D>());
            Destroy(other.gameObject);
            _player.AddScore(5);
        }

        if (other.tag == "HeatSeeker" && _player != null)
        {
            _anim.SetTrigger("OnMineCollision");
            _audioSource.Play();
            _blownUp = true;
            Destroy(gameObject, 2.7f);
            Destroy(GetComponent<Collider2D>());
            Destroy(other.gameObject);
            _player.AddScore(5);
        }
    }
}
