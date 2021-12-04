using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public Rigidbody2D rb2D;
    
    private void Update()
    {
        if (!GameManager.instance.playerCanMove) {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");

        if (horizontal != 0) {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0) {
            Move(horizontal, vertical);
        }
    }
    
    
    void Move (int x, int y)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(x, y);
        
        StartCoroutine(SmoothMovement(end));

        GameManager.instance.playerCanMove = false;

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
