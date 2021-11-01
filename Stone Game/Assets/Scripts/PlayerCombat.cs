using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    PlayerStats playerStats;
    LayerMask EnemyLayer;
    SpriteRenderer spriteRenderer;
    [SerializeField] Color normalColor;
    
    float damage;
    float range;
    float atkSpeed;

    private void Awake() 
    {
        playerStats = GetComponent<PlayerStats>();
        GetStats();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        normalColor= spriteRenderer.color;
    }
    private void Start() 
    {
        EnemyLayer = LayerMask.NameToLayer ("Enemy");
    }

    void Update()
    {

        //aoe damging abilites
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.DrawRay(transform.position,transform.TransformDirection(Vector2.right)*range,Color.red);
            //use skill range
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.TransformDirection(Vector2.right), range, 1 << LayerMask.NameToLayer("Enemy"));

            for (int i = 0; i < hit.Length; i++)
            {
                //cast skill
                hit[i].transform.GetComponent<Enemy>().TakeDamage(damage); 
            }
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

    void attack()
    {
        //cast ray and use range and cast skill
    }

    void DealPysicalDamage(Enemy enemy)
    {
        enemy.TakeDamage(damage);

    }

    //make public later to call whenever stats update is needed
    void GetStats()
    {
        damage = playerStats.CalculatedCombatStats()[0];
        range = playerStats.CalculatedCombatStats()[1];
        atkSpeed = playerStats.CalculatedCombatStats()[2];
    }
}
