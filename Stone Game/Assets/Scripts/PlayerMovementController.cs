using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField]
    LayerMask lmWalls;
    [SerializeField]
    float fJumpVelocity = 5;

    Rigidbody2D rigid;

    float fJumpPressedRemember = 0;
    [SerializeField]
    float fJumpPressedRememberTime = 0.2f;

    float fGroundedRemember = 0;
    [SerializeField]
    float fGroundedRememberTime = 0.25f;

    [SerializeField]
    float fHorizontalAcceleration = 1;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingBasic = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenStopping = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenTurning = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    float fCutJumpHeight = 0.5f;

    //-----------------------------flip player
    private bool facingRight = true;
    private float moveDirection;
    //-------------------------

    //--------------------------------------------------------gizmos
    bool m_Started;
    [SerializeField] float groundBoxYPos;
    [SerializeField] float groundBoxXScal;

    bool canMove = true;
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if(m_Started)
        {
            Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, groundBoxYPos);
            Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(groundBoxXScal, 0);
            Gizmos.DrawWireCube(v2GroundedBoxCheckPosition,v2GroundedBoxCheckScale);
        }
    }
    //-----------------------------------------------------

    void Start ()
    {
        m_Started = true;

        rigid = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {

        if(canMove)
        {

            //------------------------flip player
            // moveSpeed = playerStats.GetMoveSpeed();

            GetInput();
            CheckIfFlipIsNeeded();
        
            //-----------------------------------------
    

            Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, groundBoxYPos);
            Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(groundBoxXScal, 0);

            bool bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);

            fGroundedRemember -= Time.deltaTime;
            if (bGrounded)
            {
                fGroundedRemember = fGroundedRememberTime;
            }

            fJumpPressedRemember -= Time.deltaTime;
            if (Input.GetButtonDown("Jump"))
            {
                fJumpPressedRemember = fJumpPressedRememberTime;
            }

            if (Input.GetButtonUp("Jump"))
            {
                if (rigid.velocity.y > 0)
                {
                    rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * fCutJumpHeight);
                }
            }

            if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0))
            {
                fJumpPressedRemember = 0;
                fGroundedRemember = 0;
                rigid.velocity = new Vector2(rigid.velocity.x, fJumpVelocity);
            }

            float fHorizontalVelocity = rigid.velocity.x;
            fHorizontalVelocity += Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
            else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(fHorizontalVelocity))
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
            else
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

            rigid.velocity = new Vector2(fHorizontalVelocity, rigid.velocity.y);
        }
    }

   //----------------------flip player
    private void FlipCharacter()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void GetInput()
    {
        moveDirection = Input.GetAxis("Horizontal");
    }

    private void CheckIfFlipIsNeeded()
    {
        if (moveDirection > 0 && !facingRight)
        {
            FlipCharacter();
        }
        else if (moveDirection < 0 && facingRight)
        {
            FlipCharacter();
        }
    }
    //---------------------------

    public void StopMoving()
    {
        canMove = false;
    }
}
