using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    Rigidbody rb;

    public Vector3 halfExtents;
    public LayerMask collisionMask;
    public float rise;
    public bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        grounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!grounded )
        {
            //Passe parfois à travers le sol après avoir été poussé
            RaycastHit hit;
            if (Physics.BoxCast(transform.position, halfExtents, Vector3.down, out hit, transform.rotation, Mathf.Infinity, collisionMask)){
                if(hit.distance <= Time.deltaTime)
                {
                    transform.position += Vector3.down * hit.distance;
                    Debug.Log("Decreased by hit.distance");
                    grounded = true;
                }
                else
                {
                    transform.position += Vector3.down * Time.deltaTime;
                    Debug.Log("Decreased by Time.deltaTime");
                }
            } 
            else {
                transform.position += Vector3.down * Time.deltaTime;
            }
        }
        Debug.Log(grounded);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" /*&& SkillManager.ActualForm == "Human" */)
        {
            Vector3 offset = this.transform.position - other.transform.position;

            Vector3 moveTo = Vector3.zero;
            if (Mathf.Abs(offset.x) > Mathf.Abs(offset.z))
            {
                if (offset.x > 0)
                {
                    //right
                    moveTo += Vector3.right * Time.deltaTime;
                } 
                else
                {
                    //left
                    moveTo += Vector3.left * Time.deltaTime;
                }
            }
            else
            {
                if (offset.z > 0)
                {
                    //forward
                    moveTo += Vector3.forward * Time.deltaTime;
                } 
                else
                {
                    //back
                    moveTo += Vector3.back * Time.deltaTime;
                }
            }

            moveTo += Vector3.up * rise;
            
            if (Physics.BoxCast(transform.position, halfExtents, moveTo.normalized, transform.rotation, moveTo.magnitude, collisionMask))
            {
                
            } 
            else 
            {
                transform.position += moveTo;
                RaycastHit hit;
                if (Physics.BoxCast(transform.position, halfExtents, Vector3.down, out hit, transform.rotation, rise+Mathf.Epsilon, collisionMask)){
                    transform.position += Vector3.down * hit.distance;
                    grounded = true;
                } else {
                    transform.position += Vector3.down * rise;
                    grounded = false;
                }
            }
        }
    }

}
