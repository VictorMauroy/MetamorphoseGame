using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInteraction : MonoBehaviour
{
    Material SlimeMaterial;
    bool touchedBySlime;
    Renderer rend;
    Material baseMaterial;
    float changeTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        touchedBySlime = false;
        rend = GetComponent<Renderer>();
        baseMaterial = rend.material;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (touchedBySlime && changeTime < 1f)
        {
            changeTime += Time.deltaTime;
            Debug.Log(changeTime);
            rend.material.Lerp(baseMaterial, SlimeMaterial, changeTime);
        } */
    }

    public void SwitchMaterial( Material newMaterial){
        //SlimeMaterial = newMaterial;
        rend.material = newMaterial;
        //touchedBySlime = true;
    }
}
