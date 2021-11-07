using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    PlayerStats playerStats;
    LayerMask EnemyLayer;
    SpriteRenderer spriteRenderer;
    [SerializeField] Color normalColor;
    [SerializeField] float pushBackForce;
    [SerializeField] float pushUpForce;
    [SerializeField] float recoilForce;
    
    float damage;
    float range;
    float atkSpeed;

    private void Awake() 
    {
        playerStats = GetComponent<PlayerStats>();
        // GetStats();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        normalColor= spriteRenderer.color;
    }
    private void Start() 
    {
        EnemyLayer = LayerMask.NameToLayer ("Enemy");
    }

    void Update()
    {   

        if((Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow))&& Input.GetKeyDown(KeyCode.X))
        {
            DamageAllInDirection(Vector2.up );
        }
        else if((Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow)) && Input.GetKeyDown(KeyCode.X))
        {
            DamageAllInDirection(Vector2.down);
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            DamageAllInDirection(Vector2.right);
        }

    }

    public float TakeDamage(float damage)
    {
        playerStats.TakeDamage(damage);
        spriteRenderer.color = Color.yellow;
        Invoke("RestoreColor",0.1f);
        return damage;
    }

       void RestoreColor()
    {
        spriteRenderer.color = normalColor;
    }

    void DealPysicalDamage(Enemy enemy)
    {
        enemy.TakeDamage(damage);

    }

    void DamageAllInDirection(Vector2 direction)
    {
        Debug.DrawRay(transform.position,transform.TransformDirection(direction)*range,Color.red);
        //use skill range
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.TransformDirection(direction), range, 1 << LayerMask.NameToLayer("Enemy"));

        for (int i = 0; i < hit.Length; i++)
        {
            //cast skill
            Enemy currentEnemy =  hit[i].transform.GetComponent<Enemy>();
            currentEnemy.TakeDamage(damage);
            if(!currentEnemy.GetIsAlive())
            {
                //takeExp and destroy
            }

            //apply force in the oposite direction the cated ray hit
            Vector2 hitPointv= new Vector2();
            hitPointv = transform.position;
            Vector2 dir = hit[i].point - hitPointv;
            dir = dir.normalized;

            Rigidbody2D rigid = hit[i].transform.GetComponent<Rigidbody2D>();//ref
            rigid.AddForce(dir*pushBackForce);
            rigid.AddForce(Vector2.up *pushUpForce);

            GetComponent<Rigidbody2D>().AddForce(-dir.normalized*recoilForce);

        }
    }

    public void SetTotalDamage(float totalDamage)
    {
        damage = totalDamage;
    }
    public void SetRange(float totalRange)
    {
        range = totalRange;
    }
    public void SetAtkSpeed(float totalAtkSpeed)
    {
        atkSpeed = totalAtkSpeed;
    }

    //make public later to call whenever stats update is needed
    // public void GetStats()
    // {
    //     //get stats
    //     damage = playerStats.CalculatedCombatStats()[0];
    //     range = playerStats.CalculatedCombatStats()[1];
    //     atkSpeed = playerStats.CalculatedCombatStats()[2];
    // }

}
