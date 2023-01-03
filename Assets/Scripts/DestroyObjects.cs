using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjects : MonoBehaviour
{
    public float removeAfter = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, removeAfter);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
