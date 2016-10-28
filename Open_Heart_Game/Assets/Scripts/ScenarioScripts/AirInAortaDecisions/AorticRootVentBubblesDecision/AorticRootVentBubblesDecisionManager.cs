using UnityEngine;
using System.Collections;

public class AorticRootVentBubblesDecisionManager : DecisionManagerAbstractClass {

    AorticRootVentBubblesDecisionManager thisScript;

    public GameObject ParticleSystem;

    float timeActivated= 0.0f;

    float howLongToShow = 5.0f;

    void Awake()
    {
        thisScript = gameObject.GetComponent<AorticRootVentBubblesDecisionManager>();

    }

   
    protected override void ActivateDecisionManagerScript()
    {
        thisScript.enabled = true;
        ParticleSystem.SetActive(true);
        timeActivated = Time.time;
    }

    protected override void DeactivateDecisionManagerScript()
    {
        thisScript.enabled = false;
        ParticleSystem.SetActive(false);
        
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - timeActivated > howLongToShow)
        {
            Decision(Decisions.Correct);
        }
	}
}
