using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dush : MonoBehaviour
{
    public Vector3 moveDirection;

    public const float maxDashTime = 1.0f;
    public float dashDistance = 10f;
    public float dashStoppingSpeed = 0.1f;
    float currentDashTime = maxDashTime;
    float dashSpeed = 6f;
    CharacterController controller;

    Animator m_anim;



    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Dush")) //Left shift
        {
            m_anim.SetTrigger("Dush");
            currentDashTime = 0;
        }
        if (currentDashTime < maxDashTime)
        {
            moveDirection = transform.forward * dashDistance;
            currentDashTime += dashStoppingSpeed;
        }
        else
        {
            moveDirection = Vector3.zero;
        }
        controller.Move(moveDirection * Time.deltaTime * dashSpeed);
    }
}
