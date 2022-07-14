using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RarePowerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;    
    [SerializeField] private int _rarePowerupID;
    [SerializeField] private AudioClip _clip;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

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
                switch (_rarePowerupID)
                {
                    case 0:
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
