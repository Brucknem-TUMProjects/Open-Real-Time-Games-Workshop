using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickAnimation : MonoBehaviour
{
    public InputDevice input;

    private Animator animator;

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (input.aileron == 0)
            animator.SetFloat("x", input.sideRudder);
        else
            animator.SetFloat("x", input.aileron);
        animator.SetFloat("y", input.elevator);
	}
}
