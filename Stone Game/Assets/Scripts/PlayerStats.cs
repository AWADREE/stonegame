using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{   
    //basic stats

    //[SerializeField] float moveSpeed =10f;

    [SerializeField] float maxHealthPoints =1000f;
    [SerializeField] float hpRegin =10f;
    [SerializeField] bool isAlive = true;
    
    //combat Stats
    float damage;
    float atkSpeed;
    float range;
    float critChance =0.1f;
    float critMulti =1.25f;

    [SerializeField] float currentHealthPoints;

    [SerializeField] Text hpText;
    [SerializeField] Slider healthSlider;

    PlayerMovement playerMovement;
    SpriteRenderer spriteRenderer;
    Weapon weapon;
    float elapsed = 0f;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    private void Start() {
        //initialising hp and mp
        currentHealthPoints = maxHealthPoints;
        healthSlider.value = calculateHealth();
    }

    private void Update() 
    {

        healthSlider.value = calculateHealth();

        UpdateProfileText();

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

    //public float GetMoveSpeed()
    //{
    //    return moveSpeed;
    //}
    
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

    void UpdateProfileText()
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

    //function that calculates combat stats and return damage, range, and atkSpeed
    public float[] CalculatedCombatStats()
    {
        weapon = GetComponentInChildren<Weapon>();
        float[] stats = new float[3];
        float tempDamage;

        tempDamage = weapon.GetDamage();
        //get and calculate combat style damage and total dmg

        range = weapon.GetRng();
        atkSpeed = weapon.GetSpeed();

        stats[0]= tempDamage;
        stats[1]= range;
        stats[2]= atkSpeed;
        return stats;
    }
}
