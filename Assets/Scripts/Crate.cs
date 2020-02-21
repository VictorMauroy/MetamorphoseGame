﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Crate : MonoBehaviour
{
    public GameObject player;
    Rigidbody rb;
    BoxCollider boxCollider;
    public float detectDistance;
    public Vector3 halfExtents;
    public LayerMask collisionMask;
    Vector3 moveDir;
    bool moved;
    public PhysicMaterial activeCratePhysics;
    public PhysicMaterial inactiveCratePhysics;
    bool pulled;
    bool offset_Set;
    Vector3 offset;
    public float detectAngle;
    CharacterManager playerManager;
    public bool playerTriggering;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        moveDir = Vector3.zero;
        moved = false;
        boxCollider.material = inactiveCratePhysics;
        pulled = false;
        offset_Set = false;
        playerManager = player.GetComponent<CharacterManager>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | 
                RigidbodyConstraints.FreezePositionX | 
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY |
                RigidbodyConstraints.FreezeRotationZ;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Angle(player.transform.forward, transform.position - player.transform.position) < detectAngle && playerTriggering)
        {
            playerManager.pullInteractionImage.gameObject.SetActive(true);
            Debug.Log("Finded");
            if (!Input.GetKey(playerManager.left) && !Input.GetKey(playerManager.right) && Input.GetMouseButton(0))
            {
                if (Input.GetKey(playerManager.back) && !Input.GetKey(playerManager.forward))
                {
                    playerManager.canRun = false;
                    playerManager.pulling = true;
                    Debug.Log("Pulling");
                    rb.constraints = RigidbodyConstraints.FreezeRotationX |
                    RigidbodyConstraints.FreezeRotationY |
                    RigidbodyConstraints.FreezeRotationZ;
                    boxCollider.material = activeCratePhysics;
                    if (!offset_Set)
                    {
                        offset = transform.position - player.transform.position;
                        offset_Set = true;
                    }
                    transform.position = player.transform.position + offset;
                } else
                {
                    playerManager.pulling = false;
                }

                if (!Input.GetKey(playerManager.back) && Input.GetKey(playerManager.forward))
                {
                    playerManager.canRun = false;
                    playerManager.pushing = true;
                    Debug.Log("Pushing");
                    rb.constraints = RigidbodyConstraints.FreezeRotationX |
                    RigidbodyConstraints.FreezeRotationY |
                    RigidbodyConstraints.FreezeRotationZ;
                } else
                {
                    playerManager.pushing = false;
                }
            } else
            {
                playerManager.canRun = true;
                playerManager.pulling = false;
                playerManager.pushing = false;
            }
            
        }
        else
        {
            boxCollider.material = inactiveCratePhysics;
            offset_Set = false;
            rb.constraints = RigidbodyConstraints.FreezePositionZ | 
                RigidbodyConstraints.FreezePositionX | 
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY |
                RigidbodyConstraints.FreezeRotationZ;

            playerManager.canRun = true;
            playerManager.pulling = false;
            playerManager.pushing = false;
            playerManager.pullInteractionImage.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            playerTriggering = true;
        }    
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerTriggering = false;
            playerManager.pullInteractionImage.gameObject.SetActive(false);
        }
    }

}
