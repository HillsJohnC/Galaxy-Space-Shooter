using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    private Player _player;
    private Animator _anim;
    private Collider2D _enemyCollider;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        // Null check player
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
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

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
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            _enemyCollider.enabled = false;
            _anim.SetTrigger("OnEnemyDeath");
            Destroy(this.gameObject, 2.8f);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(1);
            }

            _enemyCollider.enabled = false;
            _anim.SetTrigger("OnEnemyDeath");
            Destroy(this.gameObject, 2.8f);
        }
    }
}
