//
//		Path following particle system script
//		Set up 6 empty objects for the particles to follow
//      Copyright (c) Vincent DeLuca 2014.  All rights reserved.
//
//      Edited by Serge Zamarripa

using UnityEngine;
using System.Collections;

public class BubblePathController : MonoBehaviour
{

    //setting up your 6 targets
    /*
    public Transform target1;
    public Transform target2;
    public Transform target3;
    public Transform target4;
    public Transform target5;
    public Transform target6;
    */
    public Transform[] targets;

    ParticleSystem myParticleSystem;

    int numOfTargets;

    int divisor;

    Vector3[] vectors;

    float numTargetsFloat;

    void Start()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
        numOfTargets = targets.Length;
        divisor = numOfTargets * 2; 
        vectors = new Vector3[numOfTargets];
        numTargetsFloat = numOfTargets;
    }

    void Update()
    {
        Trail();
    }

    void Trail()
    {
        ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[myParticleSystem.particleCount + 1];
        int numberOfParticles = myParticleSystem.GetParticles(particleArray);

        /*
        Vector3 D1 = target1.position - transform.position;
        Vector3 D2 = target2.position - target1.position;
        Vector3 D3 = target3.position - target2.position;
        Vector3 D4 = target4.position - target3.position;
        Vector3 D5 = target5.position - target4.position;
        Vector3 D6 = target6.position - target5.position;
        */

        vectors[0] = targets[0].position - transform.position;
        for (int i = 1; i < numOfTargets; i++)
        {
            vectors[i] = targets[i].position - targets[i - 1].position;
        }



        for (int i = 0; i < numberOfParticles; i++)
        {
            int j;
            for (j = 1; j < divisor; j += 2)
            {
                if (particleArray[i].lifetime < ((j * particleArray[i].startLifetime) / divisor))
                {
                    if (j == 1)
                    {
                        particleArray[i].velocity = numTargetsFloat / particleArray[i].startLifetime * vectors[numOfTargets - 1];
                    }
                    else
                    {
                        float t = ((particleArray[i].startLifetime / numOfTargets) - (particleArray[i].lifetime - (((j - 2) * particleArray[i].startLifetime) / divisor))) / (particleArray[i].startLifetime / numOfTargets);
                        particleArray[i].velocity = numTargetsFloat / particleArray[i].startLifetime * Bezier(vectors[numOfTargets - ((j + 1) / 2)], vectors[numOfTargets - (j / 2)], t);
                    }
                    break;
                }
                
            }
            if (j >= divisor)
            {
                particleArray[i].velocity = numTargetsFloat / particleArray[i].startLifetime * vectors[0];
            }
            //setting the velocity of each particle from target to target
            /*
            if (particleArray[i].lifetime < (particleArray[i].startLifetime / 12))
            {
                particleArray[i].velocity = 6f / particleArray[i].startLifetime * D6;
            }
            else if (particleArray[i].lifetime < ((3 * particleArray[i].startLifetime) / 12))
            {
                float t = ((particleArray[i].startLifetime / 6) - (particleArray[i].lifetime - (particleArray[i].startLifetime / 12))) / (particleArray[i].startLifetime / 6);
                particleArray[i].velocity = 6f / particleArray[i].startLifetime * Bezier(D5, D6, t);
            }
            else if (particleArray[i].lifetime < ((5 * particleArray[i].startLifetime) / 12))
            {
                float t = ((particleArray[i].startLifetime / 6) - (particleArray[i].lifetime - ((3 * particleArray[i].startLifetime) / 12))) / (particleArray[i].startLifetime / 6);
                particleArray[i].velocity = 6f / particleArray[i].startLifetime * Bezier(D4, D5, t);
            }
            else if (particleArray[i].lifetime < ((7 * particleArray[i].startLifetime) / 12))
            {
                float t = ((particleArray[i].startLifetime / 6) - (particleArray[i].lifetime - ((5 * particleArray[i].startLifetime) / 12))) / (particleArray[i].startLifetime / 6);
                particleArray[i].velocity = 6f / particleArray[i].startLifetime * Bezier(D3, D4, t);
            }
            else if (particleArray[i].lifetime < ((9 * particleArray[i].startLifetime) / 12))
            {
                float t = ((particleArray[i].startLifetime / 6) - (particleArray[i].lifetime - ((7 * particleArray[i].startLifetime) / 12))) / (particleArray[i].startLifetime / 6);
                particleArray[i].velocity = 6f / particleArray[i].startLifetime * Bezier(D2, D3, t);
            }
            else if (particleArray[i].lifetime < ((11 * particleArray[i].startLifetime) / 12))
            {
                float t = ((particleArray[i].startLifetime / 6) - (particleArray[i].lifetime - ((9 * particleArray[i].startLifetime) / 12))) / (particleArray[i].startLifetime / 6);
                particleArray[i].velocity = 6f / particleArray[i].startLifetime * Bezier(D1, D2, t);
            }
            else
            {
                particleArray[i].velocity = 6f / particleArray[i].startLifetime * D1;
            }
                */

            }

        myParticleSystem.SetParticles(particleArray, numberOfParticles);
    }

    //this is the math to smooth out the path, known as bezier curves
    private Vector3 Bezier(Vector3 P0, Vector3 P2, float t)
    {
        Vector3 P1 = (P0 + P2) / 2f;
        Vector3 B;
        B = (1f - t) * ((1f - t) * P0 + t * P1) + t * ((1f - t) * P1 + t * P2);
        return B;
    }
}
