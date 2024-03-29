using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 8.0f;
    public bool isEnemyLaser = false;
    private bool _isBackFire = false;

    // Update is called once per frame
    void Update()
    {
        if (isEnemyLaser == false || _isBackFire == true)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 8.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    void MoveDown()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down);

        if (transform.position.y < -8.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        isEnemyLaser = true;
    }

    public void AssignBackFire()
    {
        _isBackFire = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && (isEnemyLaser == true || _isBackFire == true))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
        }
        else if (other.CompareTag("Powerup") && isEnemyLaser)
        {
            other.GetComponent<Powerup>().PowerupHit();
            Destroy(gameObject);
        }
    }
}
