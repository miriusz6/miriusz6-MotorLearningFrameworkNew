using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockupCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.transform.localPosition = new Vector3(0, 0.555f, -0.21f);
            this.transform.localRotation = Quaternion.Euler(45, 0, 0);
        }
        
    }
}
