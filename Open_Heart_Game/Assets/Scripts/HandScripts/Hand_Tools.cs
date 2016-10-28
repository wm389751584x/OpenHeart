using UnityEngine;
using System.Collections;

/// <summary>
/// Determines if a tool needs to be activated
/// </summary>
public class Hand_Tools : MonoBehaviour {
	public GameObject[] ClampParts = new GameObject[2];
	public GameObject[] ScissorsParts = new GameObject[2];
    public GameObject[] SmallClampParts = new GameObject[2];
	public GameObject Forceps;
	public GameObject Scalpel;

	Animator m_animator;

	int activate_scissors_tag_hash = Animator.StringToHash("ActivateScissors");
	int deactivate_scissors_tag_hash = Animator.StringToHash("DeactivateScissors");
	int activate_clamp_tag_hash = Animator.StringToHash("ActivateClamp");
	int deactivate_clamp_tag_hash = Animator.StringToHash("DeactivateClamp");
	int activate_scalpel_tag_hash = Animator.StringToHash("ActivateScalpel");
	int deactivate_scalpel_tag_hash = Animator.StringToHash("DeactivateScalpel");
	int activate_forceps_tag_hash = Animator.StringToHash("ActivateForceps");
	int deactivate_forceps_tag_hash = Animator.StringToHash("DeactivateForceps");
    int activate_small_clamp_tag_hash = Animator.StringToHash("ActivateSmallClamp");
    int deactivate_small_clamp_tag_hash = Animator.StringToHash("DeactivateSmallClamp");

    bool deactivate_scissors = false;
    bool deactivate_clamp = false;
    bool deactivate_scalpel = false;
    bool deactivate_forceps = false;
    bool deactivate_small_clamp = false;





	// Use this for initialization
	void Start () {
		m_animator = gameObject.GetComponent<Animator>();



	}
	
	// Update is called once per frame
	void Update () {
		if (m_animator.GetCurrentAnimatorStateInfo (0).tagHash == activate_scissors_tag_hash) 
        {
			ScissorsParts[0].SetActive(true);
			ScissorsParts[1].SetActive(true);
		}

        if (m_animator.GetCurrentAnimatorStateInfo (0).tagHash == deactivate_scissors_tag_hash || deactivate_scissors) 
        {
            if (m_animator.GetCurrentAnimatorStateInfo(0).tagHash == deactivate_scissors_tag_hash) // waits until the animation is complete before deactivating scissors
            {
                deactivate_scissors = true;
            }
            else
            {
                deactivate_scissors = false;
                ScissorsParts[0].SetActive(false);
                ScissorsParts[1].SetActive(false);
            }
		}

        if (m_animator.GetCurrentAnimatorStateInfo (0).tagHash == activate_clamp_tag_hash) 
        {
            ClampParts[0].SetActive(true);
            ClampParts[1].SetActive(true);

		} 
        
        if (m_animator.GetCurrentAnimatorStateInfo (0).tagHash == deactivate_clamp_tag_hash || deactivate_clamp) 
        {
            if (m_animator.GetCurrentAnimatorStateInfo(0).tagHash == deactivate_clamp_tag_hash)
            {
                deactivate_clamp = true;
            }
            else
            {
                deactivate_clamp = false;
                ClampParts[0].SetActive(false);
                ClampParts[1].SetActive(false);
            }
		}

        if (m_animator.GetCurrentAnimatorStateInfo(0).tagHash == activate_small_clamp_tag_hash)
        {
            SmallClampParts[0].SetActive(true);
            SmallClampParts[1].SetActive(true);

        }

        if (m_animator.GetCurrentAnimatorStateInfo(0).tagHash == deactivate_small_clamp_tag_hash || deactivate_small_clamp)
        {
            if (m_animator.GetCurrentAnimatorStateInfo(0).tagHash == deactivate_small_clamp_tag_hash)
            {
                deactivate_small_clamp = true;
            }
            else
            {
                deactivate_small_clamp = false;
                SmallClampParts[0].SetActive(false);
                SmallClampParts[1].SetActive(false);
            }
        }

        
        if (m_animator.GetCurrentAnimatorStateInfo (0).tagHash == activate_scalpel_tag_hash) 
        {
			Scalpel.SetActive(true);
		}
        
        if (m_animator.GetCurrentAnimatorStateInfo (0).tagHash == deactivate_scalpel_tag_hash || deactivate_scalpel) 
        {
            if (m_animator.GetCurrentAnimatorStateInfo(0).tagHash == deactivate_scalpel_tag_hash)
            {
                deactivate_scalpel = true;
            }
            else
            {
                deactivate_scalpel = false;
                Scalpel.SetActive(false);
            }
			
		}

        if (m_animator.GetCurrentAnimatorStateInfo(0).tagHash == activate_forceps_tag_hash)
        {
            Forceps.SetActive(true);
        }

        if (m_animator.GetCurrentAnimatorStateInfo(0).tagHash == deactivate_forceps_tag_hash || deactivate_forceps)
        {
            if (m_animator.GetCurrentAnimatorStateInfo(0).tagHash == deactivate_forceps_tag_hash)
            {
                deactivate_forceps = true;
            }
            else
            {
                deactivate_forceps = false;
                Forceps.SetActive(false);
            }

        }
	}
}
