using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 10f;
    public float jumpForce = 10f;
    public float laneDistance = 2f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private int currentLane = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        // Move Pra frente
        transform.position += Vector3.forward * speed * Time.deltaTime;

        // Jump if the player is grounded and the user swipes up
        if (isGrounded && Input.touchCount > 0 && Input.GetTouch(0).deltaPosition.y > 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }


        // Move the player to the left lane
        if (Input.touchCount > 0 && Input.GetTouch(0).deltaPosition.x < 0 && currentLane > 0)
        {
            transform.position += Vector3.left * laneDistance;
            currentLane--;
        }

        // Move the player to the right lane
        if (Input.touchCount > 0 && Input.GetTouch(0).deltaPosition.x > 0 && currentLane < 2)
        {
            transform.position += Vector3.right * laneDistance;
            currentLane++;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player is grounded
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
