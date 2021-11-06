using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    //referances
    [SerializeField] Text expText;
    [SerializeField] Text levelText;

    //current stats serilized for debugging
    [SerializeField] Slider expSlider;
    
    //basic stats
    [SerializeField] string weaponName;
    [SerializeField] bool melee =true;
    [SerializeField] float baseDmg =50f;
    [SerializeField] float baseRng = 50f;
    [SerializeField] float baseSpeed =5f;
    [SerializeField] bool oneHanded =true;
    [SerializeField] bool twoHanded =true;
    [SerializeField] bool sheath =true;
    [SerializeField] float level =1f;

    //other
    [SerializeField] bool equiped;
    bool uIConnected = false;
    [SerializeField] float exp =0f;
    [SerializeField] float expNeeded =1000f; //exp needed to level up to next level
    [SerializeField] float expNeededIncreasePerLevel = 1.25f;
    [SerializeField] float remaingExp;      //for debuging
    [SerializeField] float nextLevelExp;    //for debuging
    Transform playerHand;

    PlayerStats playerStats;

    private void Awake() 
    {
        playerStats = FindObjectOfType<PlayerStats>();
        playerHand = FindObjectOfType<PlayerHand>().transform;
        if(GetComponentInParent<PlayerStats>() != null)
        {
            equiped = true;
        }

    }

    private void Update() 
    {
        if(equiped)
        {
            ConnectUI();

            remaingExp = expNeeded-exp;
            //updating UI
            expSlider.value = calculateExp();
            UpdateProfileText();

            if(exp < 0)
            {
                exp =0;
            }

            //if stats exceeds max
            if(exp >= expNeeded)
            {
                levelup();
            }

        }
        else
        {
            if(GetComponentInParent<PlayerStats>())
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    void UpdateProfileText()
    {
        expText.text = ((expSlider.value*100).ToString("F2")+"%");
        levelText.text = level.ToString();
    }

    float calculateExp()
    {
        return exp/ expNeeded;
    }

    void levelup ()
    {
        level++;
        baseDmg +=20f;
        exp = 0;
        expNeeded *= expNeededIncreasePerLevel;
        sendWeaponStats();
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

    void ConnectUI()
    {

        if (uIConnected)
        {
            return;
        }
        else
        { 
            expSlider = GameObject.Find("EXP Slider").GetComponent<Slider>();
            expText = GameObject.Find("EXPText").GetComponent<Text>();
            levelText = GameObject.Find("LevelText").GetComponent<Text>();
            uIConnected = true;
        }

    }

    //public variable access


    public void GetPickedUp()
    {
        //setting playerhand as the parent of this object
        gameObject.transform.SetParent(playerHand);
        //unequip all weapons
        Component[] weapons;
        weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weaponTemp in weapons)
        {
            weaponTemp.GetUnequiped();
        }
        //equip this weapon
        equiped = true;

        //resset pos and rotation
        gameObject.transform.localPosition = new Vector3(0f,0f,0f);
        // gameObject.transform.localRotation = Quaternion.identity;
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<Collider2D>());

        //Link newWeaponStats to player
        // FindObjectOfType<PlayerCombat>().GetStats();
        sendWeaponStats();
    }

    void sendWeaponStats()
    {
        playerStats.SetBaseDamage(baseDmg); 
        playerStats.SetRange(baseRng); 
        playerStats.SetatkSpeed(baseSpeed); 
    }
    public bool IsEquiped()
    {
        return equiped;
    }

    // public void GetEquiped()
    // {
    //     equiped = true;
    // }

    public void GetUnequiped()
    {
        equiped = false;
    }

    // public float GetSpeed()
    // {
    //     return baseSpeed;
    // }

    // public float GetRng()
    // {
    //     return baseRng;
    // }

    // public float GetDamage()
    // {
    //     //return calculated damge, its just the baseDmg for now
    //     return baseDmg;
    // }

    public bool IsMelee()
    {
        return melee;
    }
    
    public bool IsOneHanded()
    {
        return oneHanded;
    }
    
    public bool IsTwoHanded()
    {
        return twoHanded;
    }
    
    public bool HaveSheath()
    {
        return sheath;
    }

}
