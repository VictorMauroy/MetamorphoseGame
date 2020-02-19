using System.Collections;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < detectDistance*2f && Input.GetMouseButton(0))
        {
            boxCollider.material = activeCratePhysics;
            if (!offset_Set)
            {
                offset = transform.position - player.transform.position;
                offset_Set = true;
            }
            Debug.DrawRay(transform.position, -offset, Color.red, 0.5f);
            if (Input.GetKey(KeyCode.S))
            {
                transform.position = player.transform.position + offset; 
            }
        } 
        else if(Vector3.Distance(transform.position, player.transform.position) < detectDistance)
        {
            /*offset_Set = false;
            boxCollider.material = activeCratePhysics;
            Vector3 offset = this.transform.position - player.transform.position;
            if (Mathf.Abs(offset.x) > Mathf.Abs(offset.z))
            {
                if (offset.x > 0)
                {
                    //right
                    moveDir = Vector3.right*Time.deltaTime;
                } 
                else
                {
                    //left
                    moveDir = Vector3.left*Time.deltaTime;
                }
                moved = true;
            }
            else
            {
                if (offset.z > 0)
                {
                    //forward
                    moveDir = Vector3.forward*Time.deltaTime;
                } 
                else
                {
                    //back
                    moveDir = Vector3.back*Time.deltaTime;
                }
                moved = true;
            }

            if (Physics.BoxCast(transform.position, halfExtents, moveDir, transform.rotation, moveDir.magnitude, collisionMask) && moved)
            {

            } else
            {
                rb.AddForce(moveDir);
                rb.AddForce(Vector3.up * Time.deltaTime * 5f);
                moved = false;
            } */
            rb.AddForce(Vector3.up * (Time.deltaTime * 20f));
        }
        else
        {
            boxCollider.material = inactiveCratePhysics;
            offset_Set = false;
        }
    }

}
