using UnityEngine;
using System.Collections;

/// <summary>
/// Keeps track of the x position of the ik target and moves the tube forwards/backwards, and side to side
/// </summary>
public class FirstBoneOnTube : MonoBehaviour {

    public GameObject TubeIKTarget;
    private float lastTubeIKTargetXPosition;


    public GameObject TubePlacement;

    // for lerping
    float nextZPosition;
    float currentZPosition;

	// Use this for initialization
	void Start () {
        lastTubeIKTargetXPosition = TubeIKTarget.transform.position.x;

        TubePlacement = gameObject; // so we don't get null exceptions.

        nextZPosition = TubePlacement.transform.position.z;
        currentZPosition = nextZPosition;
	}
	
	// Update is called once per frame
	void Update () {

        nextZPosition = TubePlacement.transform.position.z;

        currentZPosition = Mathf.Lerp(currentZPosition, nextZPosition, 0.1f);

        // accounts for the forwards and backwards position of the tube
        gameObject.transform.position =  new Vector3(gameObject.transform.position.x+(TubeIKTarget.transform.position.x - lastTubeIKTargetXPosition),gameObject.transform.position.y, currentZPosition);
        lastTubeIKTargetXPosition = TubeIKTarget.transform.position.x;

        
        
	}
}
