using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DoorType {
    TimeDoor, //Type de porte se refermant un certain temps après avoir été ouverte
    NeverCloseDoor, //Type de porte restant constamment ouverte dès lors que l'on a activé l'interrupteur
    ClassicDoor // Type de porte classique, reste ouverte dès lors que toutes les dalles sont activées, mais se ferme ou reste fermer dans le cas contraire
}

public class DoorManager : MonoBehaviour
{
    [Header("Type of Door")]
    public DoorType doorType;
    
    bool open;
    Vector3 origin;
	Vector3 velocity;

    [Header("Door properties")]
    public float smoothTime;
	public Vector3 openOffset;
    public float openTime;
    float time;
    public int nbSlabRequired;
    int nbSlabPressed;
    public string doorName;
    
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        SlabInteraction.OnSlabPressed += AtSlabPressed;
        if (doorType == DoorType.ClassicDoor) SlabInteraction.OnSlabRelease += AtSlabReleased;
        
    }

    void OnDestroy()
    {
        SlabInteraction.OnSlabPressed -= AtSlabPressed;
        if (doorType == DoorType.ClassicDoor) SlabInteraction.OnSlabRelease -= AtSlabReleased;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(
			transform.position,
			origin + (open?openOffset:Vector3.zero),
			ref velocity,
			smoothTime
		);

        if (doorType == DoorType.TimeDoor){
            if(time > 0f)
            {
                time -= Time.deltaTime;
                open = true;
            }
            if (time <= 0f )
            {
                open = false;
            }
        }
    }

    void AtSlabPressed(GameObject slab){
        if (slab.GetComponent<SlabInteraction>().targetedDoor == doorName)
        {
            switch (doorType)
            {
                case DoorType.TimeDoor :
                    time = openTime;
                break;

                case DoorType.NeverCloseDoor :
                    open = true;
                break;

                case DoorType.ClassicDoor :
                    
                    if(!slab.GetComponent<SlabInteraction>().used)
                    {
                        slab.GetComponent<SlabInteraction>().used = true;
                        nbSlabPressed ++; //Il faut l'undo dans AtSlabReleased
                    }

                    if (nbSlabPressed == nbSlabRequired)
                    {
                        open = true; 
                    }
                    
                break;
            }    
        }
        
    }

    void AtSlabReleased(GameObject slab){
        if (slab.GetComponent<SlabInteraction>().targetedDoor == doorName)
        {
            switch (doorType)
            {
                case DoorType.ClassicDoor :
                    
                    if(slab.GetComponent<SlabInteraction>().used)
                    {
                        slab.GetComponent<SlabInteraction>().used = false;
                        nbSlabPressed --;
                    }

                    if (nbSlabPressed != nbSlabRequired)
                    {
                        open = false; 
                    }
                    
                break;
            }    
        }
    }
}
