using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    

    private void Awake()
    {
        instance = this;
    }
    public TMP_Text currentAmmoUI;
    public TMP_Text magsCapacityUI;
    public TMP_Text noAmmo;
    public Slider ammoBar;
    public GameObject[] weaponImage;
    public GameObject currentImg;

    public GameObject deathPanel;
    public TMP_Text deathText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void SetAmmo(float currentAmmo, float maxCapacity)
    {
        ammoBar.value = currentAmmo;
        ammoBar.maxValue = maxCapacity;

    
    }

    public void SetWeaponIMG(int selectedWeapon)
    {
        currentImg = weaponImage[selectedWeapon];
    }
}
