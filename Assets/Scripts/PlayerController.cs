using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Transform viewPoint;

    private float verticalRotation;
    private Vector2 mouseInput;
    public float defaultSensibility = 1f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerRotation();
    }




    void PlayerRotation()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * defaultSensibility; 

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z); //horizontal rotation 

        verticalRotation = verticalRotation + mouseInput.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -60f, 60f);
        viewPoint.rotation = Quaternion.Euler(-verticalRotation, viewPoint.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z); //vertical rotation 
    }
}
