using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{   //basic stats
    [SerializeField] float moveSpeed =10f;
    [SerializeField] Text hpText;
    [SerializeField] Text mpText;
    [SerializeField] Text expText;
    [SerializeField] Text levelText;
    [SerializeField] float maxHealthPoints =1000f;
    [SerializeField] float maxMagicPoints =1000f;
    [SerializeField] float pAtk =50f;
    [SerializeField] float mpRegin =10f;
    [SerializeField] float hpRegin =10f;
    [SerializeField] float level =1f;
    [SerializeField] bool isAlive = true;
    [SerializeField] float exp =0f;
    [SerializeField] float expNeeded =1000f; //exp needed to level up to next level
    [SerializeField] float expNeededIncreasePerLevel = 1.25f;
    [SerializeField] float remaingExp;      //for debuging
    [SerializeField] float nextLevelExp;    //for debuging
    
    //current stats serilized for debugging
    [SerializeField] float currentHealthPoints;
    [SerializeField] float currentMagicPoints;
    
    //referances
    // [SerializeField] GameObject healthBarUI;
    PlayerMovement playerMovement;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider magicSlider;
    [SerializeField] Slider expSlider;
    [SerializeField] bool haveMagic = true;
    SpriteRenderer spriteRenderer;
    float elapsed = 0f;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start() {
        //initialising hp and mp
        currentHealthPoints = maxHealthPoints;
        currentMagicPoints = maxMagicPoints;
        healthSlider.value = calculateHealth();
        magicSlider.value = calculateMagic();

    }

    private void Update() 
    {
        remaingExp = expNeeded-exp;
        //updating hp bar and mp bar and exp bar
        healthSlider.value = calculateHealth();
        magicSlider.value = calculateMagic();
        expSlider.value = calculateExp();
        UpdateProfileText();

        if(currentHealthPoints <= 0)
        {
            Die();
            currentHealthPoints= 0;
        }
        if(exp < 0)
        {
            exp =0;
        }
        if(currentMagicPoints <= 0)
        {
            currentMagicPoints = 0;
            haveMagic = false;
        }
        if(currentMagicPoints >0)
        {
            haveMagic = true;
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
            if(currentMagicPoints < maxMagicPoints)
            {
                ReginMagic();
            }
        }
    }


        //if stats exceeds max
        if(exp >= expNeeded)
        {
            levelup();
            
        }
        if(currentHealthPoints > maxHealthPoints)
        {
            currentHealthPoints = maxHealthPoints;
        }
        if(currentMagicPoints> maxMagicPoints)
        {
            currentMagicPoints = maxMagicPoints;
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
    void ReginMagic()
    {
        currentMagicPoints += mpRegin;
    }

    float calculateExp()
    {
        return exp/ expNeeded;
    }

    float calculateHealth()
    {
        return currentHealthPoints/ maxHealthPoints;
    }

    float calculateMagic()
    {
        return currentMagicPoints/ maxMagicPoints;
    }


    void levelup ()
    {
        level++;
        exp = 0;
        expNeeded *= expNeededIncreasePerLevel;
    }

    public void GetExp(float expReward)
    {
        if(remaingExp > expReward)
        {
            exp +=expReward;
        }
        else
        {
           nextLevelExp = expReward - remaingExp;
           levelup();
           
            GetRestOfExp();
            //get exp




        }
    }
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    public float GetMaxHP()
    {
        return maxHealthPoints;
    }
    public float GetCurrentHP()
    {
        return currentHealthPoints;
    }
    
    public float GetCurrentMP()
    {
        return currentMagicPoints;
    }
    public float GetMaxMP()
    {
        return maxHealthPoints;
    }
    public float GetPDamage()
    {
        //return calculated damge, its just the pAtk for now
        return pAtk;
    }


    public void TakeDamage(float damage)
    {
        if(isAlive)
        {
            currentHealthPoints -= damage;
        }
    }
    
    public void SpendMagic(float magicCost)
    {
        if(isAlive && haveMagic)
        {
            currentMagicPoints -= magicCost;
        }
    }
    

    void UpdateProfileText()
    {
        hpText.text = (currentHealthPoints.ToString()+"/"+maxHealthPoints.ToString());
        mpText.text = (currentMagicPoints.ToString()+"/"+maxMagicPoints.ToString());
        expText.text = ((expSlider.value*100).ToString("F2")+"%");
        levelText.text = level.ToString();
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

    public bool HaveMagic(float magicCost)
    {
        if(currentMagicPoints>= magicCost)
        {
            haveMagic=true;
            return haveMagic;
        }
        else
        {
            haveMagic=false;
            return haveMagic;
        }
    }

    void GetRestOfExp()
    {
        if(nextLevelExp > expNeeded*2f)
        {
            nextLevelExp = expNeeded*2f;
        }
        else
        {
            exp+=nextLevelExp;
        }
    }
}
