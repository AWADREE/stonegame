using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{   
    //basic stats
    [SerializeField] float maxHealthPoints =1000f;
    [SerializeField] float maxManaPoints =1000f;
    [SerializeField] float hpRegin =10f;
    [SerializeField] bool isAlive = true;
    [SerializeField] int currency = 0; //serialized for debugging 
    [SerializeField] int currencyGold = 0; //serialized for debugging 
    [SerializeField] int currencySilver = 0; //serialized for debugging 
    [SerializeField] int currencyCopper = 0; //serialized for debugging 
    //combat Stats
    float baseDamage;
    float damage;
    float atkSpeed;
    float windUpTime;
    int currentWeaponId;
    int currentWeaponAbillityCost;
    float currentWeaponAbillityWindUpTime;
    float currentWeaponAbillityRecoveryTime;
    int currentWeaponAbillityBaseDamage;
    float currentWeaponAbillityBaseRange;
    float range;
    float critChance =0.1f;
    float critMulti =1.25f;

    [SerializeField] float currentHealthPoints;
    [SerializeField] float currentManaPoints;

    Text currencyGoldText;
    Text currencySilverText;
    Text currencyCopperText;
    Text hpText;
    Text manaText;
    Slider healthSlider;
    Slider manaSlider;

    PlayerMovementController playerMovement;
    SpriteRenderer spriteRenderer;
    Weapon weapon;
    float elapsed = 0f;
    PlayerCombat playerCombat;

    Color playerColor;


    private void Awake() {
        playerColor = GetComponent<SpriteRenderer>().color;
        // hpText = GameObject.Find("HPText").GetComponent<Text>();
        // manaText = GameObject.Find("MPText").GetComponent<Text>();
        currencyGoldText = GameObject.Find("Gold Text").GetComponent<Text>();
        currencySilverText = GameObject.Find("Silver Text").GetComponent<Text>();
        currencyCopperText = GameObject.Find("Copper Text").GetComponent<Text>();

        manaSlider = GameObject.Find("Magic Slider").GetComponent<Slider>();
        healthSlider = GameObject.Find("Health Slider").GetComponent<Slider>();
        playerMovement = GetComponent<PlayerMovementController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Start() {
        //initialising hp and mp
        currentHealthPoints = maxHealthPoints;
        healthSlider.value = CalculateHealth();
        currentManaPoints = maxManaPoints;
        manaSlider.value = CalculateMana();
        UpdateCurrencyText();
    }

    private void Update() 
    {

        healthSlider.value = CalculateHealth();
        manaSlider.value = CalculateMana();

        // UpdateHPText();
        // UpdateMpText();

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
        if(currentManaPoints > maxManaPoints)
        {
            currentManaPoints = maxManaPoints;
        }
    }


    public bool HasEnoughMana(float abillityCost)
    {
        if(abillityCost <= currentManaPoints)
        {
            currentManaPoints -= abillityCost;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Heal(float healingAmount)
    {
        currentHealthPoints += healingAmount;
    }

    public void RecoverMana(float manaAmount)
    {
        currentManaPoints += manaAmount;
    }

    void HealRegin()
    {
        currentHealthPoints += hpRegin;
    }

    float CalculateHealth()
    {
        return currentHealthPoints/ maxHealthPoints;
    }
    
    float CalculateMana()
    {
        return currentManaPoints/ maxManaPoints;
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
            spriteRenderer.color = Color.red;
            Invoke("RestoreColor",0.1f);
        }
    }

    //update HP profile text
    // void UpdateHPText()
    // {
    //     hpText.text = (currentHealthPoints.ToString()+"/"+maxHealthPoints.ToString());
    // }

    // void UpdateMpText()
    // {
    //     manaText.text = (currentManaPoints.ToString()+"/"+maxManaPoints.ToString());
    // }

    void UpdateCurrencyText()
    {
        currencyGoldText.text = (currencyGold.ToString());
        currencySilverText.text = (currencySilver.ToString());
        currencyCopperText.text = (currencyCopper.ToString());
    }

    void RestoreColor()
    {
        if(isAlive)
        {
            spriteRenderer.color = playerColor;
        }
    }

    void Die(){
        isAlive = false;
        playerMovement.PlayerDead();
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
        if(currency !=0)
        {
            currencyGold = currency /10000;
            currencySilver = (currency -(currencyGold*10000))/100;
            currencyCopper = currency - (currencyGold*10000)-(currencySilver*100);
        }
        else
        {
            currencyGold = 0;
            currencySilver = 0;
            currencyCopper = 0;
        }

        //update text
        UpdateCurrencyText();
    }

    public void SetBaseDamage(float weaponDamage)
    {
        baseDamage = weaponDamage;
        //calculate total damage
        // damage = baseDamage*(critChance*critMulti);
        damage = baseDamage;
        playerCombat.SetTotalDamage(damage);
    }
    public void SetRange(float weaponRange)
    {
        range = weaponRange;
        playerCombat.SetRange(range);
    }
    public void SetatkSpeed(float weaponatkSpeed)
    {
        atkSpeed = weaponatkSpeed;
        playerCombat.SetAtkSpeed(atkSpeed);
    }
    public void SetWindUpTime(float weaponWindUpTime)
    {
        windUpTime = weaponWindUpTime;
        playerCombat.SetWindUpTime(windUpTime);
    }
    public void SetAbillityWindUpTime(float weaponAbillityWindUpTime)
    {
        currentWeaponAbillityWindUpTime = weaponAbillityWindUpTime;
        playerCombat.SetAbillityWindUpTime( currentWeaponAbillityWindUpTime);
    }
    public void SetAbillityRecoveryTime(float weaponAbillityRecoveryTime)
    {
         currentWeaponAbillityRecoveryTime = weaponAbillityRecoveryTime;
        playerCombat.SetAbillityRecoveryTime(currentWeaponAbillityRecoveryTime);
    }
    public void SetWeaponId(int weaponId)
    {
        currentWeaponId = weaponId;
        playerCombat.SetWeaponId(currentWeaponId);
    }
    public void SetAbillityCost(int weaponAbillityCost)
    {
        currentWeaponAbillityCost = weaponAbillityCost;
        playerCombat.SetAbillityCost(currentWeaponAbillityCost);
    }
    public void SetAbillityBaseDamage(int weaponAbillityBaseDamage)
    {
        currentWeaponAbillityBaseDamage = weaponAbillityBaseDamage;
        //calculate total ability damage
        int totalAbilityDamage = currentWeaponAbillityBaseDamage;
        playerCombat.SetWeaponAbillityDamage(totalAbilityDamage);
    }
    public void SetAbillityBaseRange(float weaponAbillityBaseRange)
    {
        currentWeaponAbillityBaseRange = weaponAbillityBaseRange;
        //calculate total ability Range
        float totalAbilityRange = currentWeaponAbillityBaseRange;
        playerCombat.SetWeaponAbillityRange(totalAbilityRange);
    }

    public int GetEquipedWeaponId()
    {
        return currentWeaponId;
    }
}
