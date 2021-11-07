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
    [SerializeField] float angleOfPush;
    // [SerializeField] float pushUpForce;
    [SerializeField] float recoilForce;
    [SerializeField] float atkRadius =1f;

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
        // RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.TransformDirection(direction), range, 1 << LayerMask.NameToLayer("Enemy"));

        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, atkRadius, transform.TransformDirection(direction),range,1 << LayerMask.NameToLayer("Enemy"));
            
        for (int i = 0; i < hit.Length; i++)
        {
            //cast skill
            // Debug.Log("Hit"+i+"recorded");
            Enemy currentEnemy =  hit[i].transform.GetComponent<Enemy>();

            currentEnemy.TakeDamage(damage);

            if(!currentEnemy.GetIsAlive())
            {
                // Debug.Log("dead enemy "+i+" Detected");
                Component[] weapons;
                weapons = GetComponentsInChildren<Weapon>();
                foreach (Weapon weaponTemp in weapons)
                {
                    if(weaponTemp.IsEquiped())
                    {
                        // Debug.Log("exp sent from enemy " +i);
                        weaponTemp.GetExp(currentEnemy.GetEnemyExp());
                        currentEnemy.killObject();
                        // Debug.Log("Enemy " +i+" is ordered to die");
                    }
                }
                //takeExp and destroy
            }

            //apply force in the oposite direction the of the point ray hit
            //creating a vector containing the direction to push enemy in
            Vector2 position2D= new Vector2();
            position2D = transform.position;
            Vector2 dir = hit[i].point - position2D;
            dir = dir.normalized;
            //get recoiled
            //applying the new velocity to recoil the player back
            // GetComponent<Rigidbody2D>().AddForce(-dir.normalized*recoilForce);
            GetComponent<Rigidbody2D>().velocity = recoilForce *(-dir.normalized);

            
            //creating a vector containing the direction to push enemy in
            //changing the y point on a vector to change the angle of the push to a higher point
            // Vector2 aboveHitPoint = new Vector2( hit[i].point.x , hit[i].point.y+angleOfPush);
            // aboveHitPoint.y = hit[i].point.y+angleOfPush;
            // Vector2 dirAngled = aboveHitPoint - position2D;
            Vector2 angledDir = new Vector2 ( hit[i].point.x , hit[i].point.y + angleOfPush) - position2D;
            angledDir = angledDir.normalized;
            Rigidbody2D rigid = hit[i].transform.GetComponent<Rigidbody2D>();//ref
            // rigid.AddForce(dir*pushBackForce);
            // rigid.AddForce(Vector2.up *pushUpForce);
            //applying the new velocity to push enemy in an angle
            rigid.velocity = pushBackForce *(angledDir);

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

    private void OnDrawGizmos() {
       Gizmos.color = Color.red;
       Gizmos.DrawWireSphere(transform.position, atkRadius);

    }

}
