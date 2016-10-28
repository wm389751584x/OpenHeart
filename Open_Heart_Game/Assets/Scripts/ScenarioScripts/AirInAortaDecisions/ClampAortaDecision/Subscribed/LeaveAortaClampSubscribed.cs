using UnityEngine;
using System.Collections;

/// <summary>
/// Subscribed to when the Clamp is placed on the Aorta. Only purpose
/// is to leave the clamp in place on the Aorta
/// </summary>
public class LeaveAortaClampSubscribed : SubscribedAbstractClass {


    public GameObject RightHand;
    public GameObject LeftHand;

    HandMovement rightHandMovementScript;
    HandMovement leftHandMovementScript;
    Hand_Animations rightHandAnimationsScript;
    Hand_Animations leftHandAnimationsScript;
    LeaveAortaClampSubscribed thisScript;

    public GameObject RightHandClampRightPiece;
    public GameObject RightHandClampLeftPiece;
    public GameObject LeftHandClampRightPiece;
    public GameObject LeftHandClampLeftPiece;


    public Transform Table;

    
    private bool inTransition = false;

    

	// Use this for initialization
	void Start () 
    {
        
        rightHandAnimationsScript = RightHand.GetComponent<Hand_Animations>();
        rightHandMovementScript = RightHand.GetComponent<HandMovement>();
        leftHandAnimationsScript = LeftHand.GetComponent<Hand_Animations>();
        leftHandMovementScript = LeftHand.GetComponent<HandMovement>();
        thisScript = gameObject.GetComponent<LeaveAortaClampSubscribed>();


	}
	
	// Update is called once per frame
	void Update () 
    {
        switch (ClampAortaDecisionManager.handThatClamped)
        {
            case SixenseHands.LEFT:

                if (leftHandAnimationsScript.m_animator.GetAnimatorTransitionInfo(0).IsName("Release And Leave Clamp -> Idle") && !inTransition)
                {

                    GameObject leftPiece = (GameObject) GameObject.Instantiate(LeftHandClampLeftPiece, LeftHandClampLeftPiece.transform.position, LeftHandClampLeftPiece.transform.rotation);
                    GameObject rightPiece = (GameObject)GameObject.Instantiate(LeftHandClampRightPiece, LeftHandClampRightPiece.transform.position, LeftHandClampRightPiece.transform.rotation);
                    leftPiece.transform.parent = Table;
                    rightPiece.transform.parent = Table;
                    LeftHandClampLeftPiece.SetActive(false);
                    LeftHandClampRightPiece.SetActive(false);
                    leftHandAnimationsScript.SetAnimationToIdle();
                    leftHandMovementScript.activateHandMovement();
                    inTransition = true;
                    thisScript.enabled = false;
                }

                break;

            case SixenseHands.RIGHT:

                if (rightHandAnimationsScript.m_animator.GetAnimatorTransitionInfo(0).IsName("Release And Leave Clamp -> Idle") && !inTransition)
                {

                    GameObject leftPiece = (GameObject)GameObject.Instantiate(RightHandClampLeftPiece, RightHandClampLeftPiece.transform.position, RightHandClampLeftPiece.transform.rotation);
                    GameObject rightPiece = (GameObject)GameObject.Instantiate(RightHandClampRightPiece, RightHandClampRightPiece.transform.position, RightHandClampRightPiece.transform.rotation);
                    leftPiece.transform.parent = Table;
                    rightPiece.transform.parent = Table;
                    RightHandClampLeftPiece.SetActive(false);
                    RightHandClampRightPiece.SetActive(false);
                    rightHandAnimationsScript.SetAnimationToIdle();
                    rightHandMovementScript.activateHandMovement();
                    inTransition = true;
                    thisScript.enabled = false;
                }

                break;
        }
	}

    public override void subscribedMethod(DecisionManagerAbstractClass.Decisions decision)
    {


        
        // need to disable both the left and right hand aorta clamp. One of the hands may have the aorta clamp selected, and not be the hand
        // that clamped the heart

        switch (ClampAortaDecisionManager.handThatClamped)
        {
            
            case SixenseHands.LEFT:
                leftHandMovementScript.deactivateHandMovement();
                leftHandAnimationsScript.m_animator.SetTrigger("ClampingHeart");
                break;

            case SixenseHands.RIGHT:
                rightHandMovementScript.deactivateHandMovement();
                rightHandAnimationsScript.m_animator.SetTrigger("ClampingHeart");
                
                break;
        }
    }


    
}
