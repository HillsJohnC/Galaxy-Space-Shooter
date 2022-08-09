using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Powerup")
        {
            transform.parent.GetComponent<Enemy>().ShootPU();
        }
    }
}
