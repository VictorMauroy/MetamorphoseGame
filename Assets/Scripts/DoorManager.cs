using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public enum DoorType {
    TimeDoor, //Type de porte se refermant un certain temps après avoir été ouverte
    NeverCloseDoor, //Type de porte restant constamment ouverte dès lors que l'on a activé l'interrupteur
    ClassicDoor // Type de porte classique, reste ouverte dès lors que toutes les dalles sont activées, mais se ferme ou reste fermer dans le cas contraire
}

public class DoorManager : MonoBehaviour
{
    [Header("Type of Door")]
    public DoorType doorType;
    
    public bool open;
    Vector3 origin;
	Vector3 velocity;

    [Header("Door properties")]
    public float smoothTime;
	public Vector3 openOffset;
    public Vector3 openRotation;
    public float openTime;
    float time;
    public int nbSlabRequired;
    public int nbSlabPressed;
    public string doorName;
    bool stateReached;
    public bool isImportedMesh;
    Vector3 undoImportedMeshRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        SlabInteraction.OnSlabPressed += AtSlabPressed;
        SlabInteraction.OnSlabRelease += AtSlabReleased;
        open = false;
        stateReached = false;
        undoImportedMeshRotation = new Vector3(-90,0,0);
    }

    void OnDestroy()
    {
        SlabInteraction.OnSlabPressed -= AtSlabPressed;
        SlabInteraction.OnSlabRelease -= AtSlabReleased;
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
        
        if (open && !stateReached)
        {
            stateReached = true;
            Debug.Log("opening: " + gameObject.name);
            if (isImportedMesh)
            {
                transform.DORotate(undoImportedMeshRotation + openRotation, smoothTime);    
            } else
            {
                transform.DORotate(openRotation, smoothTime);
            }
            
        } 
        
        if(!open && !stateReached)
        {
            stateReached = true;
            if (isImportedMesh)
            {
                transform.DORotate(undoImportedMeshRotation, smoothTime);    
            } else
            {
                transform.DORotate(Vector3.zero, smoothTime);
            }
        }
        

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
        foreach (string doorNameValue in slab.GetComponent<SlabInteraction>().targetedDoor)
        {
            if (doorNameValue == doorName)
            {
                Debug.Log(slab.name + " enter on: " + gameObject.name);
                switch (doorType)
                {
                    case DoorType.TimeDoor :
                        nbSlabPressed ++; 
                        if (nbSlabPressed == nbSlabRequired)
                        {
                            stateReached = false;
                            time = openTime;
                        }
                    break;

                    case DoorType.NeverCloseDoor :
                        nbSlabPressed ++;
                        if (nbSlabPressed == nbSlabRequired)
                        {
                            open = true; 
                            stateReached = false;
                        }
                    break;

                    case DoorType.ClassicDoor :
                        nbSlabPressed ++; 
                        if (nbSlabPressed == nbSlabRequired)
                        {
                            open = true; 
                            stateReached = false;
                        }
                        
                    break;
                }    
            }    
        }
        
    }

    void AtSlabReleased(GameObject slab){
        foreach (string doorNameValue in slab.GetComponent<SlabInteraction>().targetedDoor)
        {
            Debug.Log("end interaction with me");
            if (doorNameValue == doorName)
            {
                Debug.Log(slab.name + " exit");
                switch (doorType)
                {
                    case DoorType.ClassicDoor :
                        stateReached = false;
                        nbSlabPressed --;
                        if (nbSlabPressed != nbSlabRequired)
                        {
                            open = false; 
                        }
                        
                    break;

                    case DoorType.NeverCloseDoor :
                        stateReached = false;
                        nbSlabPressed --;
                    break;

                    case DoorType.TimeDoor :
                        stateReached = false;
                        nbSlabPressed --;
                        
                    break;
                }    
            }
        }
        
    }
}
