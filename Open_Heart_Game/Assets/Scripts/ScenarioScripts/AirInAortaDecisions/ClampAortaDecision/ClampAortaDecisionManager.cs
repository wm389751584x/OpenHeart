using UnityEngine;
using System.Collections;



public class ClampAortaDecisionManager : DecisionManagerAbstractClass {

    public static SixenseHands handThatClamped = SixenseHands.UNKNOWN;

    public GameObject right_hand;
    public GameObject left_hand;

    private Hand_Animations rightAnimationScript;
    private Hand_Animations leftAnimationScript;

    private BoxCollider thisCollider;
    private ClampAortaDecisionManager thisScript;


    void Awake()
    {
        rightAnimationScript = right_hand.GetComponent<Hand_Animations>();
        leftAnimationScript = left_hand.GetComponent<Hand_Animations>();
        thisCollider = gameObject.GetComponent<BoxCollider>();
        // needs to be in awake to prevent Start() race condition errors.
        // in other words, if this was in Start() and another function in Start()
        // specifically DFSConstructFromXML, tried to call a function that used
        // thisScript, you would get errors.
        thisScript = gameObject.GetComponent<ClampAortaDecisionManager>();
    }


    void Start()
    {

    }

    

    protected override void ActivateDecisionManagerScript()
    {
        // not necessary to enable this script since no Update() method
        // is being used. but just did it as an example.
        thisScript.enabled = true;
        thisCollider.enabled = true;
    }

    protected override void DeactivateDecisionManagerScript()
    {
        // disable collider to stop physics engine from calling OnTriggerStay
        thisCollider.enabled = false;
        // not necessary to disable this script since no Update() method
        // is being used. but just did it as an example.
        thisScript.enabled = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("RightAortaClampTrigger") )
        {
            if (rightAnimationScript.m_controller.Trigger > 0.99f)
            {
                handThatClamped = SixenseHands.RIGHT;
                Decision(DecisionManagerAbstractClass.Decisions.Correct);
            }
        }

        if (other.tag.Equals("LeftAortaClampTrigger"))
        {
            if (leftAnimationScript.m_controller.Trigger > 0.99f)
            {
                handThatClamped = SixenseHands.LEFT;
                Decision(DecisionManagerAbstractClass.Decisions.Correct);
            }
        }
           
    }
}
