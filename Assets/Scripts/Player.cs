using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class Player : MonoBehaviour
{
    public Rigidbody2D rb2D;
    public BoxCollider2D boxCollider2D;
    public LayerMask blockingLayer;
    public float speed = 0.05f;
    public float maxHealth = 3;
    public float health = 1;
    public Transform WaitIcon;
    public Transform WeaponIcon;
    public Transform WeaponIcon2;
    private Animator animator;
    private int maxMovementSounds = 3;

    private void Start()
    {
        health = maxHealth;
        GameManager.instance.healthBar.Set(1);
        animator = GetComponent<Animator>();
        EventManager.StartListening("PlayerWaitIcon", OnPlayerWaitIcon);        
    }

    private void Update()
    {                
        if (GameManager.instance.GetState() == enumGameStates.Input)
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
            GameManager.instance.ChangeState(enumGameStates.PlayerMoving);
            // Set Animation
            int movement = 0;
            if (vertical > 0)
                movement = 1;
            else if (horizontal > 0)
                movement = 2;
            else if (vertical < 0)
                movement = 3;
            else if (horizontal < 0)
                movement = 4;            

            animator.SetInteger("movement", movement);

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
                Debug.Log("Enemy hit: " + enemy.name);
                StartCoroutine(PlayerAtatck(enemy, 1));                                
            } else            
                GameManager.instance.ChangeState(enumGameStates.EnemiesMove);

            animator.SetInteger("movement", 0);
        } else
        {
            PlayMovementSound();      
            StartCoroutine(SmoothMovement(end));            
        }        
    }
    
    IEnumerator SmoothMovement(Vector3 end)
    {
        float remaining = (transform.position - end).sqrMagnitude;

        yield return new WaitForSeconds(0.1f);

        while (remaining > float.Epsilon)
        {            
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, speed * Time.deltaTime);

            //rb2D.MovePosition(newPosition);
            transform.position = newPosition;

            remaining = (transform.position - end).sqrMagnitude;

            yield return null;
        }
                                
        GameManager.instance.ChangeState(enumGameStates.EnemiesMove);        
        animator.SetInteger("movement", 0);
    }

    public void DamagePlayer(int damage)
    {
        AudioManager.instance.PlaySFX("PlayerDamaged1");
        health -= damage;
        GameManager.instance.healthBar.Set(health / maxHealth);
        if (health <= 0)
        {
            Destroy(gameObject, 4.0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            if (GameManager.instance.level == GameManager.instance.levels.Length)
            {
                GameManager.instance.ChangeState(enumGameStates.GameEnd);
            }
            else
            {
                Restart();
            }
        }
    }

    private void Restart()
    {
        GameManager.instance.StartGame();
        Destroy(GameObject.Find("Map"));
    }

    private void OnDestroy()
    {
        EventManager.StopListening("PlayerWaitIcon", OnPlayerWaitIcon);        
    }

    private void PlayMovementSound()
    {
        int idx = Random.Range(0, maxMovementSounds);
        string sfxName = "PlayerMove" + (idx+1).ToString();
        AudioManager.instance.PlaySFX(sfxName);
    }

    private IEnumerator PlayerAtatck(Enemy enemy, int damage)
    {                
        WeaponIcon.gameObject.SetActive(true);
        AudioManager.instance.PlaySFX("PlayerHit");
        yield return new WaitForSeconds(0.5f);
        WeaponIcon2.gameObject.SetActive(true);
        AudioManager.instance.PlaySFX("PlayerHit");
        yield return new WaitForSeconds(0.5f);
        enemy.DamageEnemy(damage);
        yield return new WaitForSeconds(1f);
        WeaponIcon.gameObject.SetActive(false);
        WeaponIcon2.gameObject.SetActive(false);
        enemy.CheckEnemyHealth();
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.ChangeState(enumGameStates.EnemiesMove);
    }

    void OnPlayerWaitIcon(Dictionary<string, object> message)
    {        
        bool enable = (bool)message["enable"];        
        
        if (enable)
        {            
            WaitIcon.gameObject.SetActive(true);            
        } else
        {
            WaitIcon.gameObject.SetActive(false);
        }
    }    
}
