using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    PlayerStats playerStats;
    float pDamage;
    LayerMask EnemyLayer;
    SpriteRenderer spriteRenderer;
    [SerializeField] Color normalColor;
    
    [SerializeField] float skill1Damage =1.2f;
    [SerializeField] float skill1Range =10f;
    [SerializeField] float skill1MagicCost = 50f;

    [SerializeField] float skill1CastTime = 0.5f;
    [SerializeField] float skill2MagicCost = 20f;
    [SerializeField] float skill2MagicHealAmount = 300f;
    // Start is called before the first frame update

    private void Awake() 
    {
        playerStats = GetComponent<PlayerStats>();
        pDamage = playerStats.GetPDamage();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        normalColor= spriteRenderer.color;
    }
private void Start() {
    EnemyLayer = LayerMask.NameToLayer ("Enemy");
}
    // Update is called once per frame
    void Update()
    {
        //aoe damging abilites
        if(playerStats.HaveMagic(skill1MagicCost))
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.DrawRay(transform.position,transform.TransformDirection(Vector2.right)*skill1Range,Color.red);
                //use skill range
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.TransformDirection(Vector2.right), skill1Range, 1 << LayerMask.NameToLayer("Enemy"));

                playerStats.SpendMagic(skill1MagicCost);
                for (int i = 0; i < hit.Length; i++)
                {
                    //cast skill
                    hit[i].transform.GetComponent<Enemy>().TakeDamage(pDamage* skill1Damage); 
                }
            }
            
        }

            //single target ability
            // if(Input.GetKeyDown(KeyCode.Alpha1))
            //{
            // RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), skillRange,1 << LayerMask.NameToLayer("Enemy"));
            // if(hit){
            //     hit.transform.GetComponent<Enemy>().TakeDamage(pDamage);
            //     hit.transform.GetComponent<SpriteRenderer>().color = Color.red;
            // }
            //}


        //healing self inflective skills
        if(playerStats.HaveMagic(skill2MagicCost))
        {

            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerStats.Heal(skill2MagicHealAmount);
                playerStats.SpendMagic(skill2MagicCost);
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

    void DealPysicalDamage(Enemy enemy)
    {
        enemy.TakeDamage(pDamage);

    }
    

 
}
