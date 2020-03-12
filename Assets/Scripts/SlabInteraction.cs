using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void SlabPressedEvent(GameObject slab);
public delegate void SlabReleaseEvent(GameObject slab);

public class SlabInteraction : MonoBehaviour
{
    public static SlabPressedEvent OnSlabPressed;
    public static SlabReleaseEvent OnSlabRelease;
    public bool activate;
    public bool used;
    public string[] targetedDoor;
    Vector3 origin;
	Vector3 velocity;
    public float smoothTime;
	public Vector3 openOffset;

    // Start is called before the first frame update
    void Start()
    {
        activate = false;
        used = false;
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(
			transform.position,
			origin + (activate?openOffset:Vector3.zero),
			ref velocity,
			smoothTime
		);
    }

    void OnTriggerStay(Collider other)
    {
        if ((other.tag == "Player" || other.tag == "Crate") && used == false) //Ajouter également les divers blocs ou éléments pouvant activer la dalle
        {
            activate = true;
            if (OnSlabPressed != null) OnSlabPressed(this.gameObject);
            Debug.Log("Pressed");
            used = true;
        }    
        
    }

    void OnTriggerExit(Collider other)
    {
        if ( (other.tag == "Player" || other.tag == "Crate") && used == true )
        {
            activate = false;
            if(OnSlabRelease != null) OnSlabRelease(this.gameObject);
            used = false;
        }
    }
}
