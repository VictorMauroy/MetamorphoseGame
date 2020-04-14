using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SlabInteraction))]
public class TorchInteract : MonoBehaviour
{
    SlabInteraction slab;
    public GameObject[] torchLights = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {
        slab = GetComponent<SlabInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (slab.activate)
        {
            foreach(GameObject light in torchLights)
            {
                light.SetActive(true);
            }
        } 
        else
        {
            foreach (GameObject light in torchLights)
            {
                light.SetActive(false);
            }
        }
    }
}
