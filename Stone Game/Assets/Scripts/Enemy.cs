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
    [SerializeField] float pushUpForce;
    [SerializeField] Weapon weapon;
    [SerializeField] bool chassing = false;       //serilized for debuging
    [SerializeField] Color normalColor;
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

    public void TakeDamage (float damage)
    {
        if(isAlive)
        {
            currentHealthPoints -= damage;
            GetComponentInChildren<SpriteRenderer>().color = Color.red;
            Invoke("RestoreColor",0.1f);
        }
    }

    void Die()
    {
        // weapon = FindObjectOfType<PlayerStats>().GetComponentInChildren<Weapon>();
        Component[] weapons;
        weapons = FindObjectOfType<PlayerStats>().GetComponentsInChildren<Weapon>();
        foreach (Weapon weaponTemp in weapons)
        {
            if(weaponTemp.IsEquiped())
            {
                weapon = weaponTemp;
            }
        }

        isAlive= false;
        Invoke("GiveWeaponExp",Random.Range(0.0f, 1f));
        // player.GetExp(expReward);
        //do vfx and sfx
        Invoke("DeathColor",0.3f);
        Invoke("killObject",1.1f );

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

    public bool IsEnemyAlive()
    {
        return isAlive;
    }
    void killObject()
    {
        Destroy(gameObject);
    }
    void GiveWeaponExp()
    {
        weapon.GetExp(expReward);
    }


    void StartAttacking()
    {
        Debug.DrawRay(GetComponentInChildren<EnemyHand>().transform.position,transform.TransformDirection(Vector2.right)*enemyRange,Color.red);
        // RaycastHit2D attackHit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), enemyRange, 1 << LayerMask.NameToLayer("Player"));
        RaycastHit2D attackHit = Physics2D.Raycast(transform.position, enemyHand.transform.position - transform.position, enemyRange, 1 << LayerMask.NameToLayer("Player"));

        if(attackHit)
        {            
            attackHit.transform.GetComponent<PlayerStats>().TakeDamage(damage);
            //apply force in the oposite direction the cated ray hit
            Vector2 hitPointv= new Vector2();
            hitPointv = transform.position;
            Vector2 dir = attackHit.point - hitPointv;
            dir = dir.normalized;
                
            Rigidbody2D rigid = attackHit.transform.GetComponent<Rigidbody2D>();//ref
            rigid.AddForce(dir*pushBackForce);
            rigid.AddForce(Vector2.up *pushUpForce);
        }
        attacking = false;//for debugging
        finishedAttacking = true;
    }


}
