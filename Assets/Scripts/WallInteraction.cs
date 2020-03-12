using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInteraction : MonoBehaviour
{
    Material SlimeMaterial;
    public bool touchedBySlime;
    Renderer rend;
    Material baseMaterial;
    float changeTime = 0f;
    public GameObject slimeActivatedSprite;
    public Transform[] ClimbPosition;
    [HideInInspector]
    public Transform[] activeClimbPositions = new Transform[2]; 

    // Start is called before the first frame update
    void Start()
    {
        touchedBySlime = false;
        rend = GetComponent<Renderer>();
        baseMaterial = rend.material;
        slimeActivatedSprite.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (touchedBySlime)
        {
            slimeActivatedSprite.SetActive(true);
            slimeActivatedSprite.transform.rotation *= Quaternion.AngleAxis(0.5f, Vector3.up);

            for (int i = 0; i < ClimbPosition.Length; i += 2)
            {
                if (Vector3.Distance(CharacterManager.Player.transform.position, ClimbPosition[i].transform.position) < 2f)
                {
                    activeClimbPositions[0] = ClimbPosition[i];
                    activeClimbPositions[1] = ClimbPosition[i + 1];
                    CharacterManager.Player.GetComponent<CharacterManager>().climbWall = gameObject;
                }
            }

        } else
        {
            slimeActivatedSprite.SetActive(false);
        }

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
        touchedBySlime = true;
    }
}
