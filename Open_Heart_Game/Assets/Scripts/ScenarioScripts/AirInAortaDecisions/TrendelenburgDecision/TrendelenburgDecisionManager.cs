using UnityEngine;
using System.Collections;

public class TrendelenburgDecisionManager : DecisionManagerAbstractClass {

    public GameObject table;
    private Animator tableAnimator;

    private TrendelenburgDecisionManager thisScript;
    private float timeBecameDecisionManager = 0.0f;
    private float timeUntilFailure = 10.0f;
    private bool timeSet = false;

    void Awake()
    {
        thisScript = gameObject.GetComponent<TrendelenburgDecisionManager>();
    }

	// Use this for initialization
	void Start () 
    {
        tableAnimator = table.GetComponent<Animator>();
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isActiveDecisionManager)
        {
            if (!timeSet)
            {
                timeSet = true;
                timeBecameDecisionManager = Time.time;
            }

            if (Time.time - timeBecameDecisionManager  > timeUntilFailure)
            {
                print("failed");
                Decision(Decisions.Incorrect);
            }

            if (Table.trendelenburg)
            {
                tableAnimator.SetBool("Trendelenburg",true);
                Decision(Decisions.Correct);
            }
                
        }
	}



    protected override void ActivateDecisionManagerScript()
    {
        thisScript.enabled = true;
    }

    protected override void DeactivateDecisionManagerScript()
    {
        thisScript.enabled = false;
    }
}
