using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSee : MonoBehaviour
{
    public Transform target;
    public Transform eyes; //Transform déterminant la direction dans laquelle regarde l'entité.
    public float halfAngle; //Moitié de l'angle. 60 degrés va dire qu'il y aura 60° vers la droite et la gauche en détection.
    public float maxDist; //Distance max à laquelle nous pouvons détecter la cible
    public LayerMask mask; //Masque décrivant ce qu'est un obstacle à la détection de la cible.
    public bool targetFinded; //Vrai si la cible est visible pour l'entité
    //public float detectAroundDist;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Débug une ligne : Rouge si la cible n'est pas dans l'angle de vue devant la cible. 
        //Bleu si elle y est mais trop éloignée. Vert si tout est réuni pour la détection.
        Debug.DrawLine(
            eyes.position,
            target.position,
            IsInViewCone() ? 
                (isNotCovered() ? Color.green : Color.blue) :
                Color.red
        );
        
        //Si la cible est visible, assez proche et dans un certain angle de vue devant l'entité, alors elle est détectée.
        if (IsInViewCone() && isNotCovered())
        {
            targetFinded = true;
        } else
        {
            targetFinded = false;
        }
        /*
        if (Vector3.Distance(target.transform.position, transform.position) < detectAroundDist)
        {
            if (Physics.Raycast(eyes.position, target.transform.position - eyes.position, Mathf.Infinity, mask))
            {
                targetFinded = true;
            }
        }*/
    }

    /// <summary>
    /// Fonction déterminant si la cible est assez proche et visible par l'entité (aucun obstacle entre eux).
    /// </summary>
    /// <returns>Vrai si le joueur est visible, faux sinon</returns>
    bool isNotCovered(){

        if(Vector3.Distance(target.position, transform.position ) < maxDist){
            return !Physics.Raycast(
                eyes.position,
                target.position - eyes.position,
                (target.position - eyes.position).magnitude,
                mask
            );
        } 
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Fonction déterminant si la cible est dans un certain angle de vue devant l'entité.
    /// </summary>
    /// <returns>Vrai si la cible est à l'intérieur de l'angle de vue, faux sinon.</returns>
    bool IsInViewCone(){
        return Vector3.Angle(
            eyes.forward,
            target.position - eyes.position
        ) < halfAngle;
    }
}
