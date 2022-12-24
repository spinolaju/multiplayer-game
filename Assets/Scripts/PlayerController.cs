using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    public float fireRate = 0.1f;
    public float fireRateCounter;

    public float magsAmmoCapacity = 30f;

    public float bulletPerShot = 1f;

    public float reloadDelay = 5f;

    public float currentAmmo = 30f;

    private bool isMagsEmpty; 

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
    
        MovementController();
        CursorEscape();

        if (!isMagsEmpty)
        {
            if (Input.GetMouseButtonDown(0)) HandleShoot();
            if (Input.GetMouseButton(0)) AutomaticShoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && isMagsEmpty) StartCoroutine(HandleReloadWeapon());

        UIElementsRender();
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
        mCamera.transform.position = viewPoint.position;
        mCamera.transform.rotation = viewPoint.rotation;
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
            Debug.Log($"hit {raycastHit.collider}");
            GameObject bh = Instantiate(bulletHole, raycastHit.point + (raycastHit.normal * 0.002f), Quaternion.LookRotation(raycastHit.normal, Vector3.up));
            Destroy(bh, 8f);
        }
        fireRateCounter = fireRate;

        currentAmmo -= bulletPerShot;

        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            isMagsEmpty = true;
            UIController.instance.noAmmo.gameObject.SetActive(true);
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
        yield return new WaitForSeconds(reloadDelay);
        currentAmmo = magsAmmoCapacity;
        isMagsEmpty = false;
        Debug.Log("RELOADED");
        UIController.instance.noAmmo.gameObject.SetActive(false);

    }

    public void UIElementsRender()
    {
        UIController.instance.currentAmmoUI.text = currentAmmo.ToString();
        UIController.instance.SetAmmo(currentAmmo);

        UIController.instance.currentAmmoUI.gameObject.SetActive(true);
        UIController.instance.magsCapacityUI.text = magsAmmoCapacity.ToString();
        UIController.instance.magsCapacityUI.gameObject.SetActive(true);


       
    }
}
