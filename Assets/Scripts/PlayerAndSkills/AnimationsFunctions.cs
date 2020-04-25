using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationsFunctions : MonoBehaviour
{
    public CharacterManager player;
    public GameObject baseFormObject;
    public bool specialAnimations;
    public int step;
    public bool throwingBall;
    public bool done;
    Quaternion baseFormRotationSave;
    public ParticleSystem handSlimeParticles;

    void Start()
    {
        handSlimeParticles.Stop();    
    }

    // Update is called once per frame
    void Update()
    {
        player.specialAnimation = this.specialAnimations;

        if (throwingBall && step == 2 && !done && specialAnimations)
        {
            player.gameObject.GetComponent<PlayerSkills>().ThrowSlimeBall();
            done = true;
            Debug.Log("Ball throwed");
            baseFormObject.transform.DORotateQuaternion(new Quaternion(0f,0f,0f,0f), 1f);
        }

        
        if (specialAnimations && throwingBall && !done)
        {
            baseFormObject.transform.DORotateQuaternion(baseFormRotationSave * Quaternion.AngleAxis(35f, Vector3.up), 1f);
            handSlimeParticles.Play();
        } else if(!specialAnimations && !throwingBall)
        {
            baseFormObject.transform.rotation = new Quaternion(0f,0f,0f,0f);
            handSlimeParticles.Stop();
        } /*else
        {
            handSlimeParticles.Stop();
        } */

        if (!specialAnimations)
        {
            baseFormRotationSave = baseFormObject.transform.rotation;
        }
    }
}
