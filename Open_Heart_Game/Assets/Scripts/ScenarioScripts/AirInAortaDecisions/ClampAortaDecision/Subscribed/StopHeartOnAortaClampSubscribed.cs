using UnityEngine;
using System.Collections;

public class StopHeartOnAortaClampSubscribed : SubscribedAbstractClass {

    public GameObject beatingHeart;
    public GameObject clampedHeart;
    private Animator clampedHeartAnimator;
    private StopHeartOnAortaClampSubscribed thisScript;

    void Start()
    {
        clampedHeartAnimator = clampedHeart.GetComponent<Animator>();
        thisScript = gameObject.GetComponent<StopHeartOnAortaClampSubscribed>();
    }

    public override void subscribedMethod(DecisionManagerAbstractClass.Decisions decision)
    {
        beatingHeart.SetActive(false);
        clampedHeart.SetActive(true);
        clampedHeartAnimator.SetBool("Clamped", true);
        thisScript.enabled = false;
    }
}
