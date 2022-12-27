using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isAutomatic;
    public float fireRate = 0.1f;
    public int magsAmmoCapacity = 30;
    public float reloadDelay = 5f;
    public ParticleSystem muzzleFlash;
    public GameObject weaponImg;
}
