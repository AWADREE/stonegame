using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] float maxHealthPoints =1000f;
    [SerializeField] float currentHealthPoints =200;
    [SerializeField] Slider healthSlider;
    [SerializeField] GameObject healthBarUI;
    [SerializeField] float expReward =10f;
    [SerializeField] float damage =75f;
    [SerializeField] float enemyRange = 10f;
    [SerializeField] float initiateAtkRange = 10f;
    [SerializeField] float detectionRange= 10f;
    [SerializeField] float atkWindUpTime =2.0f;
    [SerializeField] bool isAlive =true;
    [SerializeField] float moveSpeed= 10f;
    [SerializeField] float pushBackForce;
    [SerializeField] float angleOfPush;
    [SerializeField] GameObject[] drops;
    [SerializeField] float[] dropChances;  //0.1 = 10%
    [SerializeField] int coinValueFrom;
    [SerializeField] int coinValueTo;
    [SerializeField] int coinDropValue; //for debugging

    SpawnPoint mySpawnPoint;
    // [SerializeField] float pushUpForce;
    Weapon weapon; //serizlised for debugging
    [SerializeField] bool chassing = false;       //serilized for debuging
    [SerializeField] float TimeBeforeDeath =1.1f;
    [SerializeField] Color normalColor;
    bool givenExp =false;
    Transform target;
    SpriteRenderer spriteRenderer;
    bool facingRight =true;
    EnemyHand enemyHand;
    bool finishedAttacking = true;
    bool attacking = false;

    private void Awake() 
    {
        enemyHand = GetComponentInChildren<EnemyHand>();
        weapon = FindObjectOfType<PlayerStats>().GetComponentInChildren<Weapon>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        normalColor= spriteRenderer.color;
    }

    void Start()
    {
        mySpawnPoint = GetComponentInParent<SpawnPoint>();
        coinDropValue = Random.Range(coinValueFrom, coinValueTo);
        currentHealthPoints = maxHealthPoints;
        healthSlider.value = calculateHealth();
    }


    void Update()
    {
        if(isAlive)
        {
            Collider2D chaseCollider = Physics2D.OverlapCircle(transform.position, detectionRange,1 << LayerMask.NameToLayer("Player") );

            if(chaseCollider)
            {
                //chassing player
                target= chaseCollider.transform;
                if(target.transform.position.x < transform.position.x && facingRight)
                {
                    //flip enemy
                    FlipEnemy();
                }
                if(target.transform.position.x > transform.position.x && !facingRight)
                {
                    //flip enemy
                    FlipEnemy();
                }
                if(Vector2.Distance(transform.position,target.position) > (enemyRange- 0.2f))
                {
                    transform.position = Vector2.MoveTowards(transform.position,target.position, moveSpeed *Time.deltaTime);
                }
            }

            //Collider2D hit = Physics2D.OverlapCircle(transform.position, enemyRange,1 << LayerMask.NameToLayer("Player"));
            //used CircleCast insteam of OverlapCircle to be able to use contact point, and set the didtance to 0.0f so that it wouldnt move

            //check if player in atk range
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, initiateAtkRange, transform.position,0.0f,1 << LayerMask.NameToLayer("Player"));
            if(hit)
            {
                if(finishedAttacking)
                {   
                    //started attacking, as long as this is false means that we are still attacking and the invoke will not be called in update
                    finishedAttacking = false;
                    attacking = true; //for debugging
                    Invoke("StartAttacking",atkWindUpTime);
                }
            }

            if(currentHealthPoints < maxHealthPoints)
            {
                healthBarUI.SetActive(true);
            }


            healthSlider.value = calculateHealth();
            if(currentHealthPoints <= 0)
            {
                Die();
            }

            if(currentHealthPoints > maxHealthPoints)
            {
                currentHealthPoints = maxHealthPoints;
            }

        }
    }

    float calculateHealth()
    {
        return currentHealthPoints/ maxHealthPoints;
    }

    public bool TakeDamageAndCheckIfAlive (float damage)
    {
        currentHealthPoints -= damage;
        GetComponentInChildren<SpriteRenderer>().color = Color.red;
        Invoke("RestoreColor",0.1f);
        if(currentHealthPoints <= 0)
        {
            Die();
        }
        return isAlive;
    }

    void Die()
    {
        isAlive= false;
        Invoke("DeathColor",0.3f);
    }

    void RestoreColor()
    {
        spriteRenderer.color = normalColor;
    }

    void DeathColor()
    {
        spriteRenderer.color = Color.black;
    }


    private void OnDrawGizmos() {
       Gizmos.color = Color.red;
       Gizmos.DrawWireSphere(transform.position, detectionRange);
       Gizmos.DrawWireSphere(transform.position, initiateAtkRange);
       Gizmos.DrawWireSphere(transform.position, enemyRange);
    }

    void FlipEnemy()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void killObject()
    {
        Invoke("DestroyThisObject",TimeBeforeDeath );
    }

    public void DestroyThisObject()
    {
        int i=0;
        foreach (GameObject drop in drops)
        {
            if(i< drops.Length)
            {
                if(Random.Range(0f,1f)<=dropChances[i])
                {
                    InstantiateDrop(drops[i]);
                }
            }
            i++;
        }
        mySpawnPoint.EnemyIsDead();
        Destroy(gameObject);
    }


    void InstantiateDrop(GameObject drop)
    {
        Instantiate(drop,transform.position,Quaternion.identity);
    }


    public int GetCoinDropValue()
    {
        return coinDropValue;
    }

    public float GetEnemyExp()
    {
        if(!givenExp)
        {
            givenExp = true;
            return expReward;
        }
        else
        {
            return 0f;
        }
    }


    void StartAttacking()
    {
        if(isAlive)
        {
            Debug.DrawRay(GetComponentInChildren<EnemyHand>().transform.position,transform.TransformDirection(Vector2.right)*enemyRange,Color.red);
            // RaycastHit2D attackHit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), enemyRange, 1 << LayerMask.NameToLayer("Player"));
            RaycastHit2D attackHit = Physics2D.Raycast(transform.position, enemyHand.transform.position - transform.position, enemyRange, 1 << LayerMask.NameToLayer("Player"));

            if(attackHit)
            {            
                attackHit.transform.GetComponent<PlayerStats>().TakeDamage(damage);
                //apply force in the oposite direction the cated ray hit
                Vector2 position2D= new Vector2();
                position2D = transform.position;
                Vector2 dir = new Vector2 ( attackHit.point.x , attackHit.point.y + angleOfPush) - position2D;
                dir = dir.normalized;
            
                //apply forces on player
                if(attackHit.transform.GetComponent<PlayerCombat>().IsrecoveryTimeDone())
                {
                    Rigidbody2D rigid = attackHit.transform.GetComponent<Rigidbody2D>();//ref
                    rigid.velocity= pushBackForce*(dir.normalized);
                }

            }
            attacking = false;//for debugging
            finishedAttacking = true;
        }
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

}
