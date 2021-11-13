using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    PlayerStats playerStats;
    PlayerMovementController playerMovement;
    LayerMask EnemyLayer;
    // SpriteRenderer spriteRenderer;
    // [SerializeField] Color normalColor;
    [SerializeField] float pushBackForce;
    [SerializeField] float angleOfPush;
    [SerializeField] float recoilForce;
    [SerializeField] float atkRadius =1f;
    [SerializeField] int manaRecoveryOnHit;
    [SerializeField] GameObject firstDashEffect;
    [SerializeField] GameObject secondDashEffect;
    [SerializeField] float firstDashEffectTime= 0.01f;
    [SerializeField] float secondDashEffectTime= 0.01f;
    bool canAtk = true;
    float timeSinceLastAtk =0f;
    float damage;//total damge acctually applied
    float range;
    float atkSpeed;
    float windUpTime;
    int currentWeaponId;
    bool recoveryTimeDone = true;
    int currentWeaponAbillityCost;
    float currentWeaponAbillityWindUpTime;
    int currentWeaponAbillityDamage;
    float currentWeaponAbillityRange;
    [SerializeField] float timeSinceLastAbillityCast =0f; //for debugging
    float abillityRecoveryTime;

    private void Awake() 
    {
        playerMovement = GetComponent<PlayerMovementController>();
        playerStats = GetComponent<PlayerStats>();
        // spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // normalColor= spriteRenderer.color;
    }
    private void Start() 
    {
        EnemyLayer = LayerMask.NameToLayer ("Enemy");
    }

    void Update()
    {   
        timeSinceLastAtk +=Time.deltaTime;

        if(timeSinceLastAtk>= atkSpeed)
        {
            canAtk = true;
        }

        timeSinceLastAbillityCast +=Time.deltaTime;

        if(timeSinceLastAbillityCast>= abillityRecoveryTime)
        {
            recoveryTimeDone = true;
            playerMovement.StartMoving();
        }

        if(recoveryTimeDone)
        {

            if(currentWeaponId == 0 && Input.GetKeyDown(KeyCode.C))//waepon id 0 is for the Wooden sword
            {
                if(playerStats.HasEnoughMana(currentWeaponAbillityCost))
                {
                    //play abillity animation
                    playerMovement.StopMoving();
                    recoveryTimeDone = false;
                    timeSinceLastAbillityCast =0f;
                    Invoke("WoodenSwordAbillity", currentWeaponAbillityWindUpTime);
                }
            }

            if(currentWeaponId == 1 && Input.GetKeyDown(KeyCode.C))//waepon id 1 is for the katana
            {
                if(playerStats.HasEnoughMana(currentWeaponAbillityCost))
                {
                    //play abillity animation
                    playerMovement.StopMoving();
                    recoveryTimeDone = false;
                    timeSinceLastAbillityCast =0f;
                    Invoke("KatanaAbillity", currentWeaponAbillityWindUpTime);
                }
            }

            if(currentWeaponId == 2 && Input.GetKeyDown(KeyCode.C))//waepon id 2 is for the long katana
            {
                if(playerStats.HasEnoughMana(currentWeaponAbillityCost))
                {
                    //play abillity animation
                    playerMovement.StopMoving();
                    recoveryTimeDone = false;
                    timeSinceLastAbillityCast =0f;
                    Invoke("LongKatanaAbillity", currentWeaponAbillityWindUpTime);
                }
            }
        }


        if(canAtk)
        {
            //attacking up and down
            // if((Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow))&& Input.GetKeyDown(KeyCode.X))
            // {
            //     DamageAllInDirection(Vector2.up );
            // }
            // else if((Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow)) && Input.GetKeyDown(KeyCode.X))
            // {
            //     DamageAllInDirection(Vector2.down);
            // }
            // else
             if(Input.GetKeyDown(KeyCode.X))
            {
                canAtk = false;
                timeSinceLastAtk =0f;

                //if wepon id is 0 
                //play weapon 0 atk animation
                //if wepon id is 1 
                //play weapon 1 atk animation
                //if wepon id is 2 
                //play weapon 2 atk animation
                Invoke("SingleTargetAttack", windUpTime ); //change this to AoeAttack or to SingleTargetAttack
                // DamageAllInDirection(Vector2.right);
            }
        }

    }

    //abillities
    void WoodenSwordAbillity()
    {
        Vector2 lastPos = new Vector2(transform.position.x, transform.position.y);
        //instantiate lazer
        //destroy lazr object
        PlayDashEffect();
        //teleport
        AttackWithoutForce();
        transform.position+= transform.right * currentWeaponAbillityRange;
    }
    void KatanaAbillity()
    {
        Vector2 lastPos = new Vector2(transform.position.x, transform.position.y);
        //instantiate lazer
        //destroy lazr object
        PlayDashEffect();
        //teleport
        AttackWithoutForce();
        transform.position+= transform.right * currentWeaponAbillityRange;
    }
    void LongKatanaAbillity()
    {
        Vector2 lastPos = new Vector2(transform.position.x, transform.position.y);
        //instantiate lazer
        //destroy lazr object
        PlayDashEffect();
        //teleport
        AttackWithoutForce();
        transform.position+= transform.right * currentWeaponAbillityRange;
    }

    void PlayDashEffect()
    {
        firstDashEffect.GetComponent<SpriteRenderer>().enabled = true;
        Invoke("RemoveFirstDashEffect",firstDashEffectTime);
    }

    void RemoveFirstDashEffect()
    {
        firstDashEffect.GetComponent<SpriteRenderer>().enabled = false;
        secondDashEffect.GetComponent<SpriteRenderer>().enabled = true;
        Invoke("RemoveSecondDashEffect",secondDashEffectTime);
    }
    void RemoveSecondDashEffect()
    {
        secondDashEffect.GetComponent<SpriteRenderer>().enabled = false;
    }

    // public float TakeDamage(float damage)
    // {
    //     playerStats.TakeDamage(damage);
    //     spriteRenderer.color = Color.yellow;
    //     Invoke("RestoreColor",0.1f);
    //     return damage;
    // }

    // void RestoreColor()
    // {
    //     spriteRenderer.color = normalColor;
    // }

    //Area of Effect Attack
    //add a vector2 paramiter if u need to add other directions to atk in other than right and left , add Vector2 direction
    void AoeAttack()
    {
        // RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.TransformDirection(direction), range, 1 << LayerMask.NameToLayer("Enemy"));
        Debug.DrawRay(transform.position,transform.TransformDirection(Vector2.right)*range,Color.red);
        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, atkRadius, transform.TransformDirection(Vector2.right),range,1 << LayerMask.NameToLayer("Enemy"));
            
        for (int i = 0; i < hit.Length; i++)
        {
            //attack
            if(hit.Length == 0)
            {
                //no enemies in range
                return;
            }
            Enemy currentEnemy =  hit[i].transform.GetComponent<Enemy>();
            if(currentEnemy.GetIsAlive())
            {
                if(!currentEnemy.TakeDamageAndCheckIfAlive(damage))
                {
                    Component[] weapons;
                    weapons = GetComponentsInChildren<Weapon>();
                    foreach (Weapon weaponTemp in weapons)
                    {
                        if(weaponTemp.IsEquiped())
                        {
                            weaponTemp.GetExp(currentEnemy.GetEnemyExp());
                            playerStats.IncreaseCurrencyBy(currentEnemy.GetCoinDropValue());
                            currentEnemy.killObject();
                        }
                    }
                    //takeExp and destroy
                }
                ApplyRecoil(hit[i]);
                PushEnemyBack(hit[i]);
                //add mana to player
                playerStats.RecoverMana(manaRecoveryOnHit);
            }   
        }
    }

    //single target attack
    void SingleTargetAttack()
    {
        
        Debug.DrawRay(transform.position,transform.TransformDirection(Vector2.right)*range,Color.red);
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, atkRadius, transform.TransformDirection(Vector2.right),range,1 << LayerMask.NameToLayer("Enemy"));
        //attack
        if(!hit)
        {
            //no enemies in range
            return;
        }
        Enemy currentEnemy =  hit.transform.GetComponent<Enemy>();
        if(currentEnemy.GetIsAlive())
        {
            if(!currentEnemy.TakeDamageAndCheckIfAlive(damage))
            {
                Component[] weapons;
                weapons = GetComponentsInChildren<Weapon>();
                foreach (Weapon weaponTemp in weapons)
                {
                    if(weaponTemp.IsEquiped())
                    {
                        weaponTemp.GetExp(currentEnemy.GetEnemyExp());
                        playerStats.IncreaseCurrencyBy(currentEnemy.GetCoinDropValue());
                        currentEnemy.killObject();
                    }
                }
                //takeExp and destroy
            }
            ApplyRecoil(hit);
            PushEnemyBack(hit);
            //add mana to player
            playerStats.RecoverMana(manaRecoveryOnHit);
        }   
    }

    //abillity atack
    void AttackWithoutForce()
    {
        Debug.DrawRay(transform.position,transform.TransformDirection(Vector2.right)*currentWeaponAbillityRange,Color.red);
        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, atkRadius, transform.TransformDirection(Vector2.right),currentWeaponAbillityRange,1 << LayerMask.NameToLayer("Enemy"));   
        for (int i = 0; i < hit.Length; i++)
        {
            //attack
            if(hit.Length == 0)
            {
                //no enemies in range
                return;
            }
            Enemy currentEnemy =  hit[i].transform.GetComponent<Enemy>();
            //if enemy alive apply damge to it
            if(currentEnemy.GetIsAlive())
            {
                if(!currentEnemy.TakeDamageAndCheckIfAlive(currentWeaponAbillityDamage))
                {
                    //take exp if enemy is dead
                    Component[] weapons;
                    weapons = GetComponentsInChildren<Weapon>();
                    foreach (Weapon weaponTemp in weapons)
                    {
                        if(weaponTemp.IsEquiped())
                        {
                            weaponTemp.GetExp(currentEnemy.GetEnemyExp());
                            playerStats.IncreaseCurrencyBy(currentEnemy.GetCoinDropValue());
                            currentEnemy.killObject();
                        }
                    }
                }
            }
        }
    }

    void ApplyRecoil(RaycastHit2D hit)
    {
        //apply force in the oposite direction the of the point ray hit
        //creating a vector containing the direction to push enemy in
        Vector2 position2D= new Vector2();
        position2D = transform.position;
        Vector2 dir = hit.point - position2D;
        dir = dir.normalized;
        //get recoiled
        //applying the new velocity to recoil the player back
        GetComponent<Rigidbody2D>().velocity = recoilForce *(-dir.normalized);
    }

    void PushEnemyBack(RaycastHit2D hit)
    {       
        Vector2 position2D= new Vector2();
        position2D = transform.position;
        //creating a vector containing the direction to push enemy in
        //changing the y point on a vector to change the angle of the push to a higher point
        Vector2 angledDir = new Vector2 ( hit.point.x , hit.point.y + angleOfPush) - position2D;
        Rigidbody2D enemyRigid = hit.transform.GetComponent<Rigidbody2D>();//ref
        //applying the new velocity to push enemy in an angle
        enemyRigid.velocity = pushBackForce *(angledDir.normalized);
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
    public void SetWindUpTime(float totalWindUpTime)
    {
        windUpTime = totalWindUpTime;
    }
    public void SetAbillityWindUpTime(float abillityWindupTime)
    {
        currentWeaponAbillityWindUpTime = abillityWindupTime;
    }
    public void SetWeaponId(int weaponId)
    {
        currentWeaponId = weaponId;
    }
    public void SetAbillityCost(int AbillityCost)
    {
        currentWeaponAbillityCost = AbillityCost;
    }
    public void SetAbillityRecoveryTime(float recoveryTime)
    {
        abillityRecoveryTime = recoveryTime;
    }
    public void SetWeaponAbillityDamage(int abillityDamage)
    {
        currentWeaponAbillityDamage = abillityDamage;
    }
    public void SetWeaponAbillityRange(float abillityRange)
    {
        currentWeaponAbillityRange = abillityRange;
    }

    private void OnDrawGizmos() {
       Gizmos.color = Color.red;
       Gizmos.DrawWireSphere(transform.position, atkRadius);
    }
    public bool IsrecoveryTimeDone()
    {
        return recoveryTimeDone;
    }

}
