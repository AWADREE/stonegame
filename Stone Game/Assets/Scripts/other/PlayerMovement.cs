using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    float moveSpeed;
    private Rigidbody2D rb;
    private PlayerStats playerStats;
    private bool facingRight = true;
    private float moveDirection;
    [SerializeField] bool canMove = true;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        //moveSpeed = playerStats.GetMoveSpeed();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
        {
            //get input
            GetInput();

            //move
            MoveCharacter();

        }

    }

    private void MoveCharacter()
    {
        //moveSpeed = playerStats.GetMoveSpeed();

        if (moveDirection > 0 && !facingRight)
        {
            FlipCharacter();
        }
        else if (moveDirection < 0 && facingRight)
        {
            FlipCharacter();
        }

        rb.velocity = new Vector2(moveDirection * moveSpeed  , rb.velocity.y);
    }

    private void GetInput()
    {
        moveDirection = Input.GetAxis("Horizontal");
    }

    private void FlipCharacter()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void StopMoving()
    {
        canMove = false;
    }
}
