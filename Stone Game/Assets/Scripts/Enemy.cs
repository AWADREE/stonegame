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
    [SerializeField] float detectionRange= 10f;
    [SerializeField] float coolDownTime =2.0f;
    [SerializeField] bool isAlive =true;
    [SerializeField] float moveSpeed= 10f;
    [SerializeField] Weapon weapon;
    [SerializeField] bool attacking = false;       //serilized for debuging
    [SerializeField] bool chassing = false;       //serilized for debuging
    [SerializeField] Color normalColor;
    Transform target;
    SpriteRenderer spriteRenderer;
    bool facingRight =true;

    private void Awake() {
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

        Collider2D hit = Physics2D.OverlapCircle(transform.position, enemyRange,1 << LayerMask.NameToLayer("Player") );
        if(hit){
            //attack player
            if(!attacking)
            {
                //attack
                attacking= true;
                hit.transform.GetComponent<PlayerStats>().TakeDamage(damage);
                // hit.transform.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
                Invoke("nextAttack",coolDownTime);
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

    void Die(){

        isAlive= false;
        Debug.Log("Dead");
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

    void nextAttack()
    {
        attacking = false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, enemyRange);
    }
    
    void FlipEnemy()
    {
        facingRight = !facingRight;
        Vector3 tmpScale = gameObject.transform.localScale;
        tmpScale.x *= -1;
        gameObject.transform.localScale = tmpScale;
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
}
