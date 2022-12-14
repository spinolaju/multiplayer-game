using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{

    public Transform viewPoint;

    private float verticalRotation;
    private Vector2 mouseInput;
    public float defaultSensibility = 1f;


    public float walkingPace = 4f;
    public float runningPace = 7f;
    public float currentPace;
    public float jumpHeight = 10f;
    public float gravity = 2.5f;
    private Vector3 direction;

    private Vector3 moveControl;

    private Camera mCamera;

    public CharacterController characterController;

    public Transform groundChecking;
    private bool isPlayerGrounded;
    public LayerMask groundLayer;


    public GameObject bulletHole;

    //public float fireRate = 0.1f;
    public float fireRateCounter;

   

    public int bulletPerShot = 1;



    public List<int> bulletsLeft;
    public float currentAmmoCounter;

    private bool isReloading;


    public List<WeaponInfo> weaponsList;
    private int selectedWeaponIndex;

    public WeaponInfo currentWeaponInfo;

    public float muzzleDuration;
    public float muzzleDurationCounter;

    public GameObject playerImpact;


    // Start is called before the first frame update
    void Awake()
    {

        weaponsList[1].bulletsLeft = weaponsList[1].weapon.magsAmmoCapacity;
        weaponsList[2].bulletsLeft = weaponsList[2].weapon.magsAmmoCapacity;
        currentWeaponInfo = weaponsList[0];
        currentWeaponInfo.bulletsLeft = currentWeaponInfo.weapon.magsAmmoCapacity;

    }
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mCamera = Camera.main;
        UIController.instance.SetWeaponIMG(0);

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        { 
            MovementController();
        CursorEscape();
    
       


        if (!currentWeaponInfo.isMagEmpty)
        {
            if (Input.GetMouseButtonDown(0)) HandleShoot();
            if (Input.GetMouseButton(0) && currentWeaponInfo.weapon.isAutomatic) AutomaticShoot();
            UIController.instance.noAmmo.gameObject.SetActive(false);
            UIController.instance.noAmmo.gameObject.SetActive(false);
        }

        if (currentWeaponInfo.bulletsLeft <= 0)
        {
            currentWeaponInfo.bulletsLeft = 0;
            currentWeaponInfo.isMagEmpty = true;
            UIController.instance.noAmmo.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.R) && currentWeaponInfo.isMagEmpty) StartCoroutine(HandleReloadWeapon());

        

        SwitchWeapons();
        SwitchWeaponsByNum();

        RenderSelectedWeapon();

        

        if (currentWeaponInfo.weapon.muzzleFlash.isPlaying)
        {
            muzzleDurationCounter -= Time.deltaTime;
            if (muzzleDurationCounter <= 0) currentWeaponInfo.weapon.muzzleFlash.Stop();
        }

        UIElementsRender();
        }
    }



    void PlayerRotation()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * defaultSensibility; 

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z); //horizontal rotation 

        verticalRotation += mouseInput.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -60f, 60f);
        viewPoint.rotation = Quaternion.Euler(-verticalRotation, viewPoint.rotation.eulerAngles.y, transform.rotation.eulerAngles.z); //vertical rotation 
    }

    void MovementController()
    {
        PlayerRotation();
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        CheckActivePace();

        float tempY = moveControl.y;

        moveControl = ((transform.right * direction.x)+(transform.forward * direction.z)).normalized * currentPace;

        moveControl.y = tempY;

        if(characterController.isGrounded)
        {
            moveControl.y = 0f;
        }
        

        isPlayerGrounded = Physics.Raycast(groundChecking.position, Vector3.down, 0.25f, groundLayer);

        Jump();

        moveControl.y += Physics.gravity.y * Time.deltaTime * gravity;

        characterController.Move(Time.deltaTime * moveControl); 

    }
    
    void CheckActivePace()
    {
        if(Input.GetKey(KeyCode.LeftShift)) currentPace = runningPace;
        else currentPace = walkingPace;
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            mCamera.transform.position = viewPoint.position;
            mCamera.transform.rotation = viewPoint.rotation;
        }
        
    }
    
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isPlayerGrounded) moveControl.y = jumpHeight;

    }

    void CursorEscape()//TO DO: PREVENT THE CAMERA MOVEMENT WHEN ESC IS HIT AND ADD A MENU
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        } else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButton(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void HandleShoot()
    {
        
       Ray ray = mCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
       ray.origin = mCamera.transform.position;

        

        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            
            if(raycastHit.collider.gameObject.tag == "Player")
            {
                Debug.Log($"hit {raycastHit.collider.gameObject.GetPhotonView().Owner.NickName}");
                PhotonNetwork.Instantiate(playerImpact.name, raycastHit.point, Quaternion.identity);

                raycastHit.collider.gameObject.GetPhotonView().RPC("DealingDamage", RpcTarget.All, photonView.Owner.NickName);
            }
            else
            {
                GameObject bh = Instantiate(bulletHole, raycastHit.point + (raycastHit.normal * 0.002f), Quaternion.LookRotation(raycastHit.normal, Vector3.up));
                Destroy(bh, 8f);
            }
            
        }
        fireRateCounter = currentWeaponInfo.weapon.fireRate;

        currentWeaponInfo.bulletsLeft -= bulletPerShot;



        //currentWeaponInfo.weapon.muzzleFlash.SetActive(true);

        if (!currentWeaponInfo.weapon.muzzleFlash.isPlaying)
        {
            currentWeaponInfo.weapon.muzzleFlash.Play();
        }
        muzzleDurationCounter = muzzleDuration;

        if (currentWeaponInfo.bulletsLeft <= 0)
        {
            currentWeaponInfo.bulletsLeft = 0;
            currentWeaponInfo.isMagEmpty = true;
            UIController.instance.noAmmo.gameObject.SetActive(true);
        }
       

    }
    [PunRPC]

    public void DealingDamage(string shooter )
    {
        TakingDamage(shooter);
    }

    public void TakingDamage(string shooter)
    {
        if (photonView.IsMine)
        {
            Debug.Log($"{photonView.Owner.NickName} been hit by {shooter}");
            PlayerSpawner.instance.Die(shooter);
            
        }
        
    }

    public void AutomaticShoot()
    {
        fireRateCounter -= Time.deltaTime;
        if (fireRateCounter <= 0) HandleShoot();
    }

    IEnumerator HandleReloadWeapon()
    {
        Debug.Log("Reloading...");
        isReloading = true;
        yield return new WaitForSeconds(currentWeaponInfo.weapon.reloadDelay);
        currentWeaponInfo.bulletsLeft = currentWeaponInfo.weapon.magsAmmoCapacity;
        currentWeaponInfo.isMagEmpty = false;
        Debug.Log("RELOADED");
        UIController.instance.noAmmo.gameObject.SetActive(false);
        isReloading = false;

    }

    public void UIElementsRender()
    {

        UIController.instance.currentAmmoUI.gameObject.SetActive(true);
        UIController.instance.currentImg.SetActive(true);
        UIController.instance.currentAmmoUI.text = currentWeaponInfo.bulletsLeft.ToString();
        UIController.instance.magsCapacityUI.text = currentWeaponInfo.weapon.magsAmmoCapacity.ToString();
        UIController.instance.magsCapacityUI.gameObject.SetActive(true);
        
        UIController.instance.SetAmmo(currentWeaponInfo.bulletsLeft, currentWeaponInfo.weapon.magsAmmoCapacity);

        UIController.instance.magsCapacityUI.gameObject.SetActive(true);


       
    }

    public void SwitchWeapons()
    {
        
        if (isReloading) return;

        UIController.instance.currentImg.SetActive(false);
        //currentWeaponInfo.weapon.muzzleFlash.SetActive(false);
        //currentWeaponInfo.weapon.muzzleFlash.Stop();
        int nextIndex = selectedWeaponIndex;

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f )
        {
            nextIndex++;
            if (nextIndex >= weaponsList.Count) nextIndex = 0;

        } else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            nextIndex--;
            if (nextIndex < 0) nextIndex = weaponsList.Count - 1;
        }
        currentWeaponInfo = weaponsList[nextIndex];
        selectedWeaponIndex = nextIndex;
        
        RenderSelectedWeapon();
    }

    public void SwitchWeaponsByNum()
    {
        for(int i = 0; i < weaponsList.Count; i++ )
        {
            if(Input.GetKeyDown((i+1).ToString()))
            {
                selectedWeaponIndex = i;
                currentWeaponInfo = weaponsList[i];
                RenderSelectedWeapon();

            }
        }
    }

    public void RenderSelectedWeapon()
    {
        foreach (WeaponInfo w in weaponsList) w.weapon.gameObject.SetActive(false);

        UIController.instance.currentImg.SetActive(false);

        currentWeaponInfo.weapon.gameObject.SetActive(true);
        UIController.instance.SetWeaponIMG(selectedWeaponIndex);
        UIController.instance.currentImg.SetActive(true);
        
    }
}

[System.Serializable]
public class WeaponInfo
{
    [SerializeField]
    public Weapon weapon;
    public int bulletsLeft;
    public bool isMagEmpty;
}