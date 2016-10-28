using UnityEngine;
using System.Collections;

public class HandMovement : MonoBehaviour {



    public SixenseHands hand;
    public GameObject[] TubeIKTargets;
    public GameObject[] TubeClampTargets;
    public TubeClampTarget[] TubeClampTargetsScripts; // to avoid calling getcomponent every frame
    int currentIKTargetIndex=0;
    int currentTubeTargetIndex = 0;
    
    

    
    public SixenseInput.Controller m_controller = null;



    private const float SCALE = 0.005f;

    private bool canHandMove = true;

    // used to lerp between positions when hand is intially activated
    // or after being activated after being deactivated
    float timeHandsActivated = 0.0f;


    private Vector3 lastPosition = Vector3.zero;
    private Vector3 lastFinalPosition = Vector3.zero;
    private Quaternion lastQuat = Quaternion.identity;
    private Quaternion lastFinalQuat = Quaternion.identity;
    private Vector3 pinchLastPosition = Vector3.zero;
    private Vector3 tubeClampLastPosition = Vector3.zero;

    private float handMinX = -10.0f;
    private float handMaxX = 10.0f;
    private float handMinY = -0.2f;
    private float handMaxY = 10.0f;
    private float handMinZ = -10.0f;
    private float handMaxZ = 10.0f;

    // used for switching between ik targets
    float pinchZ;

    Animator my_animator;

    // if we have grabbed a tube
    public bool tubeGrabed = false;
    // tells how much to offset when grabbing a tube
    Vector3 offsetTubeGrab = Vector3.up;


    public bool clampingTube = false;
    public GameObject LeftSmallClamp;
    public GameObject RightSmallClamp;
    private Hand_Animations myHandAnimationsScript;


    public bool unclampingTube;

    // to stop modes from being changed at critical points during the simulation
    public bool dontChangeModes = false;

    void Start()
    {
        my_animator = gameObject.GetComponent<Animator>();
        pinchZ = gameObject.transform.position.z;
        myHandAnimationsScript = gameObject.GetComponent<Hand_Animations>();


        TubeClampTargetsScripts = new TubeClampTarget[TubeClampTargets.Length]; // initialize array size
        // cache tube scripts to avoid calling get component every frame
        for (int i = 0; i < TubeClampTargets.Length; i++)
        {
            TubeClampTargetsScripts[i] = TubeClampTargets[i].GetComponent<TubeClampTarget>();
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (m_controller == null)
        {
            m_controller = SixenseInput.GetController(hand);
            timeHandsActivated = Time.time;
        }else 
        {
            UpdateHand(); // updates position of the hand
            if (my_animator.GetBool("Pinch"))
            {
                UpdatePinch(); // overwrites UpdateHand, still need the information from UpdateHand
            }
            else if (my_animator.GetBool("SmallClamp"))
            {
                UpdateTubeClamp(); // overwrites UpdateHand
            }
            else if (my_animator.GetBool("RetrieveSmallClamp"))
            {
                UpdateRetrieveSmallClamp();
            }
            else
            {
                pinchZ = gameObject.transform.position.z; // update for the modes that use this variable
            }
     
        }


        // swiggity swooty your way to the final position as determined by the above method calls.
        // Hand_Animations will call activateHandMovement between each state
        lastFinalQuat = Quaternion.Slerp(lastFinalQuat, transform.rotation, (Time.time - timeHandsActivated) * 0.5f);
        transform.rotation = lastFinalQuat;

        lastFinalPosition = Vector3.Lerp(lastFinalPosition, transform.position, (Time.time - timeHandsActivated) * 0.5f);
        transform.position = lastFinalPosition;

	}



    public void activateHandMovement()
    {
        canHandMove = true;

        timeHandsActivated = Time.time;
        
    }

    public void deactivateHandMovement()
    {
        canHandMove = false;
    }


    public void clampHandMovement(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
    {
        handMinX = minX;
        handMaxX = maxX;
        handMinY = minY;
        handMaxY = maxY;
        handMinZ = minZ;
        handMaxZ = maxZ;
    }

    public void removeClampHandMovement()
    {
        handMinX = -10.0f;
        handMaxX = 10.0f;
        handMinY = -0.2f;
        handMaxY = 10.0f;
        handMinZ = -10.0f;
        handMaxZ = 10.0f;
        timeHandsActivated = Time.time;
    }
    
  
    private void UpdateHand()
    {


        //The hand models axes do not match Unity's convention. So we have to compensate here

        // get position
        Vector3 controller_position = new Vector3(-1 * m_controller.Position.z, m_controller.Position.y, m_controller.Position.x);
 
        Vector3 handPosition = SCALE * controller_position;
        Vector3 newPosition = Vector3.Lerp(lastPosition, handPosition, (Time.time - timeHandsActivated)*0.5f);

        newPosition.x = Mathf.Clamp(newPosition.x, handMinX, handMaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, handMinY, handMaxY);
        newPosition.z = Mathf.Clamp(newPosition.z, handMinZ, handMaxZ);

        lastPosition = newPosition;

        if (canHandMove)
        {
            transform.localPosition = newPosition;
        }


        // get rotation
        Quaternion hand_roation_quaternion = m_controller.Rotation;
        Quaternion newQuat = Quaternion.Slerp(lastQuat, hand_roation_quaternion, (Time.time - timeHandsActivated) * 0.5f);


        // only update lastQuat if we are not in the following modes, for slerping purposes
        if (!my_animator.GetBool("Pinch") && !my_animator.GetBool("SmallClamp") && !my_animator.GetBool("RetrieveSmallClamp"))
        {
            lastQuat = newQuat; 
        }


        Vector3 hand_rotation_euler = newQuat.eulerAngles;

        hand_rotation_euler = new Vector3(-hand_rotation_euler.z, hand_rotation_euler.y, hand_rotation_euler.x);

        if (canHandMove)
        {
            transform.localEulerAngles = hand_rotation_euler;
        }        
    }


    // Updates the hand if you are grabbing tubes
    private void UpdatePinch()
    {
        
        // rotation of hand
        if (hand == SixenseHands.LEFT)
        {
            //slerp your way to the new angle
            transform.localEulerAngles = new Vector3(-35, 90, 0);
            

        }
        else if (hand == SixenseHands.RIGHT)
        {
            
            transform.localEulerAngles = new Vector3(35, 270, 0);
       
        }


        if (tubeGrabed) // tube in hand
        {
            transform.position = new Vector3(transform.position.x - offsetTubeGrab.x, transform.position.y - offsetTubeGrab.y, transform.position.z - offsetTubeGrab.z);
            

            if (my_animator.GetFloat("Trigger_Float") <= 0.95f) // release tube
            {
                dontChangeModes = false;
                tubeGrabed = false;
                TubeIKTargets[currentIKTargetIndex].GetComponent<TubeIKTarget>().Release();
                if (TubeIKTargets[currentIKTargetIndex].GetComponent<TubeIKTarget>().placed) // the tube has been placed, so release clamp on tube
                {

                }
                removeClampHandMovement();
            }
        }
        else
        {

            // saves the transform for use if they grab the tube
            Vector3 tempV3 = transform.position;

            // transform.position.z represents the z value that UpdateHand calculated
            // we use it to figure out the iktarget to point at, and overwrite it
            if (transform.position.z - pinchZ > 0.5f && ! (my_animator.GetFloat("Trigger_Float") > 0.95f))
            {
                currentIKTargetIndex = (currentIKTargetIndex + 1) % TubeIKTargets.Length;
                pinchZ = transform.position.z + 0.5f;
            }
            else if (transform.position.z - pinchZ < -0.5f && !(my_animator.GetFloat("Trigger_Float") > 0.95f))
            {
                currentIKTargetIndex = currentIKTargetIndex - 1;
                if (currentIKTargetIndex < 0)
                {
                    currentIKTargetIndex = TubeIKTargets.Length - 1;
                }
                pinchZ = transform.position.z - 0.5f;
            }



            Vector3 nextPosition = TubeIKTargets[currentIKTargetIndex].transform.position;
            nextPosition = Vector3.Lerp(pinchLastPosition, nextPosition, 0.1f);
            pinchLastPosition = nextPosition;

            if (hand == SixenseHands.LEFT)
            {                
                transform.position = new Vector3(nextPosition.x - 0.12f, nextPosition.y + 0.08f, nextPosition.z - 0.2f);
            }
            else if (hand == SixenseHands.RIGHT)
            {
                transform.position = new Vector3(nextPosition.x - 0.12f, nextPosition.y + 0.08f, nextPosition.z + 0.2f);
            }

            
            

            if (my_animator.GetFloat("Trigger_Float") > 0.95f)
            {
                if (TubeIKTargets[currentIKTargetIndex].GetComponent<TubeIKTarget>().grabbed == true) // already grabbed by the other hand
                {
                    return;
                }

                if (TubeIKTargets[currentIKTargetIndex].GetComponent<TubeIKTarget>().MyClampTargetScript.Clamped == false)
                {
                    Debug.Log("Need to clamp a tube before removing from a cannula! Use buttons to change tools");
                }
                else
                {
                    dontChangeModes = true;
                    tubeGrabed = true;
                    offsetTubeGrab = new Vector3(tempV3.x - transform.position.x, tempV3.y - transform.position.y, tempV3.z - transform.position.z);
                    clampHandMovement(offsetTubeGrab.x - 10.0f, offsetTubeGrab.x + 10.0f, offsetTubeGrab.y - 0.5f, offsetTubeGrab.y + 10.0f, offsetTubeGrab.z - 10.0f, offsetTubeGrab.z + 10.0f);

                    TubeIKTargets[currentIKTargetIndex].GetComponent<TubeIKTarget>().Grab(gameObject);
                    
                }
                
            }

            
        }

    }





    /// <summary>
    /// finds next available unclamped tube in positive direction
    /// if none found returns false.
    /// </summary>
    /// <returns></returns>
    private bool nextUnclampedTubePositiveDirection()
    {
        int tempIndex = currentTubeTargetIndex;
        currentTubeTargetIndex = (currentTubeTargetIndex + 1) % TubeClampTargetsScripts.Length;
        while (currentTubeTargetIndex != tempIndex)
        {
            if (TubeClampTargetsScripts[currentTubeTargetIndex].Clamped == false)
            {
                return true;
            }

            currentTubeTargetIndex = (currentTubeTargetIndex + 1) % TubeClampTargetsScripts.Length;
        }

        return false;
    }

    /// <summary>
    /// finds next available unclamped tube in negative direction
    /// if none found returns false.
    /// </summary>
    /// <returns></returns>
    private bool nextUnclampedTubeNegativeDirection()
    {
        int tempIndex = currentTubeTargetIndex;
        currentTubeTargetIndex = currentTubeTargetIndex - 1;
        if (currentTubeTargetIndex < 0)
        {
            currentTubeTargetIndex = TubeClampTargetsScripts.Length - 1;
        }

        while (currentTubeTargetIndex != tempIndex)
        {
            if (TubeClampTargetsScripts[currentTubeTargetIndex].Clamped == false)
            {
                return true;
            }

            currentTubeTargetIndex = currentTubeTargetIndex - 1;
            if (currentTubeTargetIndex < 0)
            {
                currentTubeTargetIndex = TubeClampTargetsScripts.Length - 1;
            }
        }

        return false;
    }


    private void UpdateTubeClamp()
    {

        if (clampingTube)
        {
            
            if (my_animator.GetAnimatorTransitionInfo(0).IsName("Release and Leave Small Clamp -> Idle"))
            {
                
                myHandAnimationsScript.SetAnimationToIdle();
                activateHandMovement();
                dontChangeModes = false;
                clampingTube = false;
            }

        }
        else
        {

            if (TubeClampTargetsScripts[currentTubeTargetIndex].Clamped == true) // move to the next unclamped tube, if any
            {
                if (nextUnclampedTubePositiveDirection() == false) // there isn't an available tube that is unclamped, so move to the next animation state
                {
                    myHandAnimationsScript.ManualUpdateHandAnimationToNextState();
                    return;
                }
            }

            // transform.position.z represents the z value that UpdateHand calculated
            // we use it to figure out the tube we want to point our clamp at
            if (transform.position.z - pinchZ > 0.5f) // enough differnece to move to the next clamp
            {
                nextUnclampedTubePositiveDirection(); // don't care here if there isn't another unclamped tube, we will just stay with our current tube
                pinchZ = transform.position.z + 0.5f;
            }
            else if (transform.position.z - pinchZ < -0.5f)
            {
                nextUnclampedTubeNegativeDirection(); // don't care here if there isn't another unclamped tube, we will just stay with our current tube
                pinchZ = transform.position.z - 0.5f;
            }




            Vector3 nextPostion = TubeClampTargets[currentTubeTargetIndex].transform.position;
            tubeClampLastPosition = Vector3.Lerp(tubeClampLastPosition, nextPostion, 0.1f);
            transform.position = tubeClampLastPosition;


            if (hand == SixenseHands.LEFT)
            {
                transform.localEulerAngles = new Vector3(-35, 90, 0);
  
                
                transform.position = new Vector3(tubeClampLastPosition.x - 0.10f, tubeClampLastPosition.y + 0.14f, tubeClampLastPosition.z - 0.87f);

            }
            else if (hand == SixenseHands.RIGHT)
            {
                transform.localEulerAngles = new Vector3(35, 270, 0);


                transform.position = new Vector3(tubeClampLastPosition.x - 0.10f, tubeClampLastPosition.y + 0.14f, tubeClampLastPosition.z + 0.87f);
                
            }

            

            if (my_animator.GetFloat("Trigger_Float") > 0.99f)
            {
                if (Vector3.Distance(nextPostion, tubeClampLastPosition) > 0.01f) // avoid putting the clamp in a weird spot if the clamp is moving between targets
                {
                    return;
                }

                TubeClampTargetsScripts[currentTubeTargetIndex].Clamp(LeftSmallClamp.transform.position, LeftSmallClamp.transform.rotation, RightSmallClamp.transform.position, RightSmallClamp.transform.rotation, hand);


                deactivateHandMovement();
                dontChangeModes = true;
                clampingTube = true;                
                my_animator.SetTrigger("ClampingTube");                
                LeftSmallClamp.SetActive(false);
                RightSmallClamp.SetActive(false);

            }
       


        }
    }

    /// <summary>
    /// finds next available Clamped tube in positive direction
    /// if none found returns false.
    /// </summary>
    /// <returns></returns>
    private bool nextClampedTubePositiveDirection()
    {
        int tempIndex = currentTubeTargetIndex;
        currentTubeTargetIndex = (currentTubeTargetIndex + 1) % TubeClampTargetsScripts.Length;
        while (currentTubeTargetIndex != tempIndex)
        {
            if (TubeClampTargetsScripts[currentTubeTargetIndex].Clamped == true)
            {
                return true;
            }

            currentTubeTargetIndex = (currentTubeTargetIndex + 1) % TubeClampTargetsScripts.Length;
        }

        return false;
    }

    /// <summary>
    /// finds next available Clamped tube in negative direction
    /// if none found returns false.
    /// </summary>
    /// <returns></returns>
    private bool nextClampedTubeNegativeDirection()
    {
        int tempIndex = currentTubeTargetIndex;
        currentTubeTargetIndex = currentTubeTargetIndex - 1;
        if (currentTubeTargetIndex < 0)
        {
            currentTubeTargetIndex = TubeClampTargetsScripts.Length - 1;
        }

        while (currentTubeTargetIndex != tempIndex)
        {
            if (TubeClampTargetsScripts[currentTubeTargetIndex].Clamped == true)
            {
                return true;
            }

            currentTubeTargetIndex = currentTubeTargetIndex - 1;
            if (currentTubeTargetIndex < 0)
            {
                currentTubeTargetIndex = TubeClampTargetsScripts.Length - 1;
            }
        }

        return false;
    }


    private void UpdateRetrieveSmallClamp()
    {
        if (unclampingTube)
        {
            if (my_animator.GetAnimatorTransitionInfo(0).IsName("UnclampSmallClamp -> Idle"))
            {
                myHandAnimationsScript.SetAnimationToIdle();
                activateHandMovement();
                dontChangeModes = false;
                unclampingTube = false;
                LeftSmallClamp.SetActive(false);
                RightSmallClamp.SetActive(false);
            }
        }
        else
        {
            if (TubeClampTargetsScripts[currentTubeTargetIndex].Clamped == false) // move to the next Clamped tube, if any
            {
                if (nextClampedTubePositiveDirection() == false) // there isn't an available tube that is Clamped, so move to the next animation state
                {
                    myHandAnimationsScript.ManualUpdateHandAnimationToNextState();
                    return;
                }
            }


            // transform.position.z represents the z value that UpdateHand calculated
            // we use it to figure out the tube we want to point our clamp at
            if (transform.position.z - pinchZ > 0.5f) // enough differnece to move to the next clamp
            {
                nextClampedTubePositiveDirection(); // don't care here if there isn't another unclamped tube, we will just stay with our current tube
                pinchZ = transform.position.z + 0.5f;
            }
            else if (transform.position.z - pinchZ < -0.5f)
            {
                nextClampedTubeNegativeDirection(); // don't care here if there isn't another unclamped tube, we will just stay with our current tube
                pinchZ = transform.position.z - 0.5f;
            }


            Vector3 nextPosition = TubeClampTargets[currentTubeTargetIndex].transform.position;
            tubeClampLastPosition = Vector3.Lerp(tubeClampLastPosition, nextPosition, 0.1f);
            
            


            

            if (TubeClampTargetsScripts[currentTubeTargetIndex].handThatClampedMe == SixenseHands.LEFT)
            {
                transform.localEulerAngles = new Vector3(-35, 90, 0);

                transform.position = new Vector3(tubeClampLastPosition.x - 0.10f, tubeClampLastPosition.y + 0.14f, tubeClampLastPosition.z - 0.87f);
         
                
            }
            else if (TubeClampTargetsScripts[currentTubeTargetIndex].handThatClampedMe == SixenseHands.RIGHT)
            {
               
                transform.localEulerAngles = new Vector3(35, 270, 0);

                transform.position = new Vector3(tubeClampLastPosition.x - 0.10f, tubeClampLastPosition.y + 0.14f, tubeClampLastPosition.z + 0.87f);
                
            }


            


            if (my_animator.GetFloat("Trigger_Float") > 0.99f)
            {
                if (Vector3.Distance(nextPosition, tubeClampLastPosition) > 0.01f) // avoid unclamping the clamp in a weird spot if the hand is moving between targets
                {
                    return;
                }


                LeftSmallClamp.SetActive(true);
                RightSmallClamp.SetActive(true);

                TubeClampTargetsScripts[currentTubeTargetIndex].Unclamp();
                deactivateHandMovement();
                dontChangeModes = true;
                unclampingTube = true;
                my_animator.SetTrigger("UnclampingTube");            
            }

        }
    }








}

