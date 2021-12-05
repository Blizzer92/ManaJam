using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class HealtContainer : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Player>().HealPlayer(1);
                Destroy(gameObject);
            }
        }
    }
}