using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public Rigidbody2D rb2D;
    public BoxCollider2D boxCollider2D;
    public LayerMask blockingLayer;
    public float speed = 0.05f;
    public int health = 1;
    
    private void Update()
    {                
        if (GameManager.instance.gameState == enumGameStates.Input)
        {
            PlayerInput();
        }        
    }

    void PlayerInput()
    {
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            GameManager.instance.gameState = enumGameStates.PlayerMoving;
            Move(horizontal, vertical);
        }
    }
    
    
    void Move (int x, int y)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(x, y);
        
        
        boxCollider2D.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider2D.enabled = true;
        if (hit.transform != null)
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DamageEnemy(1);
                GameManager.instance.gameState = enumGameStates.EnemiesMove;
            }
        } else
        {
            StartCoroutine(SmoothMovement(end));
        }        
    }
    
    IEnumerator SmoothMovement(Vector3 end)
    {
        float remaining = (transform.position - end).sqrMagnitude;

        while (remaining > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, 1f / speed * Time.deltaTime);

            rb2D.MovePosition(newPosition);

            remaining = (transform.position - end).sqrMagnitude;

            yield return null;
        }
        
        GameManager.instance.gameState = enumGameStates.EnemiesMove;
    }

    public void DamagePlayer(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            Invoke("Restart", .5f);
            enabled = false;
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
