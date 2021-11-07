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
    [SerializeField] bool collected;
    bool uIConnected = false;
    [SerializeField] float exp =0f;         //current exp in this lvl
    [SerializeField] float expNeeded =1000f; //exp needed to level up to next level
    [SerializeField] float expNeededIncreasePerLevel = 1.25f;
    [SerializeField] float remaingExp;      //for debuging exp waitinf to be used
    [SerializeField] float nextLevelExp;    //for debuging
    Transform playerHand;

    PlayerStats playerStats;

    private void Awake() 
    {
        // remaingExp = expNeeded;
        playerStats = FindObjectOfType<PlayerStats>();
        playerHand = FindObjectOfType<PlayerHand>().transform;
        if(GetComponentInParent<PlayerHand>() != null)
        {
            collected= true;
            equiped = true;
        }

    }

    private void Update() 
    {
        if(equiped)
        {
            ConnectUI();

            // remaingExp = expNeeded-exp;
            //updating UI
            expSlider.value = calculateExp();
            UpdateProfileText();

            if(exp < 0)
            {
                exp =0;
            }

            //if stats exceeds max
            // if(exp >= expNeeded)
            // {
            //     levelup();
            // }

        }
        // else
        // {
        //     if(GetComponentInParent<PlayerHand>())
        //     {
        //         GetComponent<SpriteRenderer>().enabled = false;
        //     }
        // }
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
        exp = 0f;
        expNeeded *= expNeededIncreasePerLevel;
        sendWeaponStats();
    }

    public void GetExp(float expReward)
    {
        if(expNeeded - exp > expReward)
        {
            exp +=expReward;
            nextLevelExp =0f;
        }
        else if(expNeeded - exp <= expReward)
        {

            nextLevelExp = expReward - (expNeeded- exp);
            levelup();
            GetRestOfExp(nextLevelExp);
            //get exp
        }
    }


    void GetRestOfExp(float expReward)
    {
        if(expReward < expNeeded)
        {
            exp+=expReward;
            nextLevelExp =0f;
        }
        else if(expReward >= expNeeded)
        {
            nextLevelExp = expReward - (expNeeded-exp);
            levelup();
            GetRestOfExp(nextLevelExp);
            // nextLevelExp = expNeeded*2f;
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
            if(weaponTemp.IsEquiped())
            {
                weaponTemp.GetUnequiped();
            }
        }
        //equip this weapon
        GetEquiped();


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

    public void GetEquiped()
    {
        equiped = true;

        GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(true);
        SpriteRenderer[] renderers = GetComponentInChildren<SpriteRenderer>().GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = true;
        }
        sendWeaponStats();
        // GetComponent<SpriteRenderer>().enabled = true;
        // GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void GetUnequiped()
    {
        equiped = false;
        SpriteRenderer[] renderers = GetComponentInChildren<SpriteRenderer>().GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        // GetComponent<SpriteRenderer>().enabled = false;
        // GetComponentInChildren<SpriteRenderer>().enabled = false;
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
