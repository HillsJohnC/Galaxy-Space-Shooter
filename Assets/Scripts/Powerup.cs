using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;    
    [SerializeField] private int _powerupID;    
    [SerializeField] private AudioClip _clip;
    private float _powerupAttractSpeed = 6f;
    private Player _player;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent <Player>();
    }

    // Update is called once per frame
    void Update()
    {
       transform.Translate(_speed * Time.deltaTime * Vector3.down);
        if (_player._isPowerupAttractActive == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _powerupAttractSpeed * Time.deltaTime);
        }

        if (transform.position.y < -4.5f)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)

    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                switch(_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.AmmoCollected();
                        break;
                    case 4:
                        player.HealthCollected();
                        break;
                    case 5:
                        player.AmmoDepleted();
                        break;
                    case 6:
                        player.TriProActive();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }            
            
            Destroy(this.gameObject);
        }
    }
}
