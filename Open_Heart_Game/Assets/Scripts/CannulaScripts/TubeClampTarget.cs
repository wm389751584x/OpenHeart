using UnityEngine;
using System.Collections;

public class TubeClampTarget : MonoBehaviour {

    public bool Clamped;

    // the individual pieces of the clamps
    public GameObject leftClamp;
    public GameObject rightClamp;


    public SixenseHands handThatClampedMe;




	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
    
	}

    public void Clamp(Vector3 leftClampPosition, Quaternion leftClampRotation, Vector3 rightClampPosition, Quaternion rightClampRotation, SixenseHands handThatIsClamping)
    {

        handThatClampedMe = handThatIsClamping;

        Clamped = true;

        leftClamp.SetActive(true);
        rightClamp.SetActive(true);




 
        
        leftClamp.transform.position = leftClampPosition;
        leftClamp.transform.rotation = leftClampRotation;

        rightClamp.transform.position = rightClampPosition;
        rightClamp.transform.rotation = rightClampRotation;
        


        
    }

    public void Unclamp()
    {
        Clamped = false;

        leftClamp.SetActive(false);
        rightClamp.SetActive(false);
    }

}
