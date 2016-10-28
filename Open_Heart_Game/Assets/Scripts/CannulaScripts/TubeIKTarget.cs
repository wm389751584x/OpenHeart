using UnityEngine;
using System.Collections;

public class TubeIKTarget : MonoBehaviour {

    // if a hand has grabbed the tube
    public bool grabbed = false;
    // the hand that grabbed the tube
    public GameObject hand;

    public bool placed;

    public GameObject FirstBoneOnTube;

    public GameObject MyClampTarget;
    public TubeClampTarget MyClampTargetScript;

    private FirstBoneOnTube FirstBoneOnTubeScript;

    public GameObject CurrentTubePlacement;

    private SixenseHands whichHandGrabbedMe;
  

	// Use this for initialization
	void Start () {
        grabbed = false;
       
        if (FirstBoneOnTube != null)
        {
            FirstBoneOnTubeScript = FirstBoneOnTube.GetComponent<FirstBoneOnTube>();
        }
        else
        {
            Debug.Log("firstbone null");
        }

        if (MyClampTarget != null)
        {
            MyClampTargetScript = MyClampTarget.GetComponent<TubeClampTarget>();
        }
        else
        {
            Debug.Log("myclamptarget null");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (grabbed && hand != null)
        {
            if (whichHandGrabbedMe == SixenseHands.LEFT)
            {
                gameObject.transform.position = new Vector3(hand.transform.position.x + 0.12f, hand.transform.position.y - 0.08f, hand.transform.position.z + 0.2f);
            }
            else if (whichHandGrabbedMe == SixenseHands.RIGHT)
            {
                gameObject.transform.position = new Vector3(hand.transform.position.x + 0.12f, hand.transform.position.y - 0.08f, hand.transform.position.z - 0.2f);
            }
        }
 
	}


    // set which tubeplacement we entered
    void OnTriggerEnter(Collider other)
    {
        if (!grabbed)
        {
            return;
        }

        if (other.tag.Equals("TubePlacement"))
        {


            if (!other.GetComponent<TubePlacement>().hasTube)
            {

                CurrentTubePlacement = other.gameObject;

                if(FirstBoneOnTubeScript != null)
                {
                    FirstBoneOnTubeScript.TubePlacement = other.gameObject;
                }
            }
        }
    }


    // set the tube placement to nothing
    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("TubePlacement"))
        {
            CurrentTubePlacement = null;
        }
    }


    // called when a hand grabs the tube
    public void Grab(GameObject grabHand)
    {

        if (CurrentTubePlacement != null)
        {
            CurrentTubePlacement.GetComponent<TubePlacement>().DisconnectTube();
        }
        placed = false;
        grabbed = true;
        hand = grabHand;

        whichHandGrabbedMe = hand.GetComponent<HandMovement>().hand;
        
    }

    // called when a hand releases the tube
    public void Release()
    {
        grabbed = false;
        hand = null;

        if (CurrentTubePlacement != null && CurrentTubePlacement.GetComponent<TubePlacement>().hasTube == false)
        {
            placed = true;
            
            gameObject.transform.position = CurrentTubePlacement.transform.position;
            CurrentTubePlacement.GetComponent<TubePlacement>().ConnectTube(gameObject);

        }
    }
   

}

