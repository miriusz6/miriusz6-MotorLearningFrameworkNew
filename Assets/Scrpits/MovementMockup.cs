using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMockup : MonoBehaviour
{
    private KeyCode Forward = KeyCode.UpArrow;
    private KeyCode Back = KeyCode.DownArrow;
    private KeyCode Left = KeyCode.LeftArrow;
    private KeyCode Right = KeyCode.RightArrow;

    private float speed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPressedMockup(Forward)) { this.transform.position += new Vector3(0, 0, 1 * speed * Time.deltaTime); }
        if (IsPressedMockup(Back)) { this.transform.position += new Vector3(0, 0, -1 * speed * Time.deltaTime); }
        if (IsPressedMockup(Left)) { this.transform.position += new Vector3(-1 * speed * Time.deltaTime, 0, 0); }
        if (IsPressedMockup(Right)) { this.transform.position += new Vector3(1 * speed * Time.deltaTime, 0, 0); }
    }

    bool IsPressedMockup(KeyCode button)
    {
        return Input.GetKey(button);
    }
}
