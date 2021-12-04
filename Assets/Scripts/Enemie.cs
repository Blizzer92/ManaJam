using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemie : MonoBehaviour
    {
        public Rigidbody2D rb2D;
        private List<Vector2> ranndomVector = new();
        private void Awake()
        {
            GameManager.instance.AddEnemieToList(this);
            ranndomVector.Add(new Vector2(-1, 0));
            ranndomVector.Add(new Vector2(1, 0));
            ranndomVector.Add(new Vector2(0, -1));
            ranndomVector.Add(new Vector2(0, 1));
        }


        public void Move ()
        {
            int random = Random.Range(0, ranndomVector.Count);

            Vector2 start = transform.position;
            Vector2 end = start + ranndomVector[random];
        
            StartCoroutine(SmoothMovement(end));
        }
    
        IEnumerator SmoothMovement(Vector3 end)
        {
            float remaining = (transform.position - end).sqrMagnitude;

            while (remaining > float.Epsilon)
            {
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, 1f / .1f * Time.deltaTime);

                rb2D.MovePosition(newPosition);

                remaining = (transform.position - end).sqrMagnitude;

                yield return null;
            }
        }

    }
