using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAdder : MonoBehaviour
{
    public Vector3 dir;
    public  float strength = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            this.GetComponent<Rigidbody>().AddForce(dir*strength,ForceMode.Force);
        }
        
    }
}
