using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePos = CharacterManager.Player.transform.position - transform.position;

        transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up); //Appliquer une légère correction à la rotation
    }
}
