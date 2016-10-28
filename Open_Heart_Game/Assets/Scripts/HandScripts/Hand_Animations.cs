using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Controls the animator controller attached to the hand
/// </summary>
public class Hand_Animations : MonoBehaviour {

	public SixenseHands m_hand;


    public Animator m_animator;
    HandStateMachine m_handState;
    public SixenseInput.Controller m_controller = null;
    float m_fLastTriggerVal;

    HandMovement handMovementScript;
    HandStateMachine.HandState m_currentState;


	// Use this for initialization
	void Start () {
        m_animator = gameObject.GetComponent<Animator>();

        m_handState = new HandStateMachine();
        m_currentState = m_handState.CurrentState;

        m_animator.SetBool("" + m_currentState, true);

        handMovementScript = gameObject.GetComponent<HandMovement>();
       
	}
	
	// Update is called once per frame
	void Update () {
        if (m_controller == null)
        {
            m_controller = SixenseInput.GetController(m_hand);
        }else if (m_animator != null)
        {
            UpdateHandAnimations();
        }
	}



    public void ManualUpdateHandAnimationToNextState()
    {
        m_animator.SetBool("" + m_currentState, false);

        HandStateMachine.HandState nextState = m_handState.MoveNext(HandStateMachine.Command.Next);
        m_animator.SetBool("" + nextState, true);
        m_currentState = nextState;
    }


    private void UpdateHandAnimations()
    {

        if ((m_controller.GetButtonDown(SixenseButtons.ONE) || m_controller.GetButtonDown(SixenseButtons.TWO) || m_controller.GetButtonDown(SixenseButtons.THREE) || m_controller.GetButtonDown(SixenseButtons.FOUR)) && !handMovementScript.dontChangeModes)
        {
            handMovementScript.activateHandMovement();
         
            m_animator.SetBool(""+m_currentState, false);

            HandStateMachine.HandState nextState = m_handState.MoveNext(HandStateMachine.Command.Next);
            m_animator.SetBool("" + nextState, true);
            m_currentState = nextState;

        }

        // Pulling Trigger
        float triggerValue = m_controller.Trigger;
        triggerValue = Mathf.Lerp(m_fLastTriggerVal, triggerValue, 0.1f);
        m_fLastTriggerVal = triggerValue;
        if (m_animator.GetBool("Pinch"))
        {
            triggerValue = Mathf.Clamp(triggerValue, 0.5f, 1.0f);
        }
        m_animator.SetFloat("Trigger_Float", triggerValue);
        

    }

    /// <summary>
    /// typically used if you want the user to use the tool in the field
    /// and leave it, such as placing clamps.
    /// </summary>
    public void SetAnimationToIdle()
    {
        m_animator.SetBool("" + m_currentState, false);

        HandStateMachine.HandState nextState = m_handState.MoveToIdle();
        m_animator.SetBool("" + nextState, true);
        m_currentState = nextState;
    }
}
