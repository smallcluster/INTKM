using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CharacterController))]
[RequireComponent(typeof(Animator))]

public class PlayerStateMachine : MonoBehaviour
{
    
    CharacterController characterController;
    Animator anim;
    Vector3 direction;
    float turnSmoothVelocity;
    public int health = 100;
    public bool downLoop = false;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        direction = new Vector3(0, 0, 1);
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        float dx = Input.GetAxis("Horizontal");
        float dz = Input.GetAxis("Vertical");

        float speed = dx * dx + dz * dz;

        anim.SetFloat("speed", speed);

        direction.Set(dx, 0, dz);
        direction.Normalize();

        if(speed != 0)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            characterController.Move(5.0f * direction * Time.deltaTime);
        }
    }
}
