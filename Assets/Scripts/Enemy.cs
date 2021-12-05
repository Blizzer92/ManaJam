using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
    {
        public Rigidbody2D rb2D;
        public BoxCollider2D boxCollider2D;
        public LayerMask blockingLayer;
        public float speed = 0.05f;
        public int health = 1;
        public float chaseDistance = 3.0f;

        [HideInInspector] public bool isMoving;
        private Player player;        
        private int maxMovementSounds = 3;
        private bool isVisible;
        
        
        private List<Vector2> ranndomVector = new();
        private void Awake()
        {
            GameManager.instance.AddEnemieToList(this);
            ranndomVector.Add(new Vector2(-1, 0));
            ranndomVector.Add(new Vector2(1, 0));
            ranndomVector.Add(new Vector2(0, -1));
            ranndomVector.Add(new Vector2(0, 1));
        }

        private void Start() 
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();            
        }


        public void Move ()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            }
            Vector2 start = transform.position;
            Vector2 end; // = start + ranndomVector[random];

            // chase player?
            float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distToPlayer > chaseDistance)
            {
                int random = Random.Range(0, ranndomVector.Count);
                end = start + ranndomVector[random];                                
            } else
            {
                Vector2 target = player.transform.position - transform.position;
                if (target.x != 0.0f)
                    target.x /= Mathf.Abs(target.x);
                if (target.y != 0.0f)
                    target.y /= Mathf.Abs(target.y);
                
                // no diagnoal move allowed. randomly move vertical or horizontal
                if (target.x != 0f && target.y != 0f)
                {
                    if (Random.Range(0, 2) == 1)
                        target.x = 0f;
                    else
                        target.y = 0f;
                }

                end = start + target;
            }
                                                        
            boxCollider2D.enabled = false;
            RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);
            boxCollider2D.enabled = true;
            if (hit.transform != null)
            {
                Player player = hit.transform.GetComponent<Player>();
                if (player != null)
                {
                    player.DamagePlayer(1);
                    //Debug.Log("DamagePlayer by: " + gameObject.GetInstanceID());
                    isMoving = false;
                }
            } else
            {
                if (isVisible)
                {
                    PlayMovementSound();
                    StartCoroutine(SmoothMovement(end));                
                } else
                {
                    //rb2D.MovePosition(end);
                    transform.position = end;
                }
            }
        }
    
        IEnumerator SmoothMovement(Vector3 end)
        {
            isMoving = true;            

            while (Vector3.Distance(transform.position, end) > float.Epsilon)            
            {
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, speed * Time.deltaTime);

                //rb2D.MovePosition(newPosition);
                transform.position = newPosition;
                
                yield return null;                
            }     
            isMoving = false;       
        }

        public void DamageEnemy(int damage)
        {
            AudioManager.instance.PlaySFX("EnemyDamaged1");
            
            health -= damage;
            if (health <= 0)
            {
                GameManager.instance.RemoveEnemyFromList(this);
                Destroy(gameObject);
            }
        }

        private void PlayMovementSound()
        {
            int idx = Random.Range(0, maxMovementSounds);
            string sfxName = "EnemyMove" + (idx + 1).ToString();
            AudioManager.instance.PlaySFX(sfxName);
        }

        private void OnBecameInvisible() 
        {
            isVisible = false;            
        }

        private void OnBecameVisible() 
        {
            isVisible = true;            
        }

    }
