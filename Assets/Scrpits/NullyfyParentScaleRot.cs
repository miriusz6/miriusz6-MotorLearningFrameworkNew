using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullyfyParentScaleRot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 RotP = this.transform.parent.localRotation.eulerAngles;
        Vector3 scaleP =  this.transform.parent.localScale;

        this.transform.localScale = new Vector3(1 / scaleP.x, 1 / scaleP.y, 1 / scaleP.z);
        this.transform.localRotation = Quaternion.Euler(new Vector3(-RotP.x, -RotP.y, -RotP.z)); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
