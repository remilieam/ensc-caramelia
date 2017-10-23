using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour
{

    private Rigidbody rb;
    public float speed;

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void FixedUpdate()
    {
        rb = GetComponent<Rigidbody>();
        // Vector3 movement = new Vector3(0.5f, 0.0f, 2);
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
        
    }



}




