using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{   
    //basic stats

    [SerializeField] float maxHealthPoints =1000f;
    [SerializeField] float hpRegin =10f;
    [SerializeField] bool isAlive = true;
    [SerializeField] int currency = 0; //serialized for debugging 
    //combat Stats
    float baseDamage;
    float damage;
    float atkSpeed;
    float windUpTime;
    int currentWeaponId;
    float range;
    float critChance =0.1f;
    float critMulti =1.25f;

    [SerializeField] float currentHealthPoints;

    [SerializeField] Text hpText;
    [SerializeField] Slider healthSlider;

    PlayerMovementController playerMovement;
    SpriteRenderer spriteRenderer;
    Weapon weapon;
    float elapsed = 0f;
    PlayerCombat playerCombat;


    private void Awake() {
        playerMovement = GetComponent<PlayerMovementController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Start() {
        //initialising hp and mp
        currentHealthPoints = maxHealthPoints;
        healthSlider.value = calculateHealth();
    }

    private void Update() 
    {

        healthSlider.value = calculateHealth();

        UpdateHPText();

        if(currentHealthPoints <= 0)
        {
            Die();
            currentHealthPoints= 0;
        }

        //every second effects
        if(isAlive)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 1f) 
            {
                elapsed = elapsed % 1f;

                if(currentHealthPoints < maxHealthPoints)
                {
                    HealRegin();
                }
            }
        }

        if(currentHealthPoints > maxHealthPoints)
        {
            currentHealthPoints = maxHealthPoints;
        }

    }

    public void Heal(float healingAmount)
    {
        currentHealthPoints += healingAmount;
    }
    void HealRegin()
    {
        currentHealthPoints += hpRegin;
    }

    float calculateHealth()
    {
        return currentHealthPoints/ maxHealthPoints;
    }
    
    public float GetMaxHP()
    {
        return maxHealthPoints;
    }
    public float GetCurrentHP()
    {
        return currentHealthPoints;
    }

    public float GetDamage()
    {
        //return calculated damge, its just the pAtk for now
        return damage;
    }

    public float GetRng()
    {
        return range;
    }

    public float GetAtkSpeed()
    {
        return atkSpeed;
    }

    public void TakeDamage(float damage)
    {
        if(isAlive)
        {
            currentHealthPoints -= damage;
        }
    }

    //update HP profile text
    void UpdateHPText()
    {
        hpText.text = (currentHealthPoints.ToString()+"/"+maxHealthPoints.ToString());
    }

    void Die(){
        isAlive = false;
        playerMovement.StopMoving();
        //sfx and vfx
        Invoke("DeathColor",0.3f);
    }
    void DeathColor()
    {
        spriteRenderer.color = Color.black;
    }

    // increase moeny
    public void IncreaseCurrencyBy(int newCurrency)
    {
        currency += newCurrency;
    }

    public void SetBaseDamage(float weaponDamage)
    {
        baseDamage = weaponDamage;
        //calculate total damage
        // damage = baseDamage*(critChance*critMulti);
        damage = baseDamage;
        playerCombat.SetTotalDamage(damage);
        // CalculateAndUpdateCombatStats();
    }
    public void SetRange(float weaponRange)
    {
        range = weaponRange;
        playerCombat.SetRange(range);
        // CalculateAndUpdateCombatStats();
    }
    public void SetatkSpeed(float weaponatkSpeed)
    {
        atkSpeed = weaponatkSpeed;
        playerCombat.SetAtkSpeed(atkSpeed);
        // CalculateAndUpdateCombatStats();
    }
    public void SetWindUpTime(float weaponWindUpTime)
    {
        windUpTime = weaponWindUpTime;
        playerCombat.SetWindUpTime(windUpTime);
        // CalculateAndUpdateCombatStats();
    }
    public void SetWeaponId(int weaponId)
    {
        currentWeaponId = weaponId;
        playerCombat.SetWeaponId(currentWeaponId);
        // CalculateAndUpdateCombatStats();
    }
}
