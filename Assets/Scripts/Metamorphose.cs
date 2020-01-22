using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metamorphose : MonoBehaviour
{
    CharacterController cc;
    float runSpeedAdd;
	float jumpForce;
    float walkSpeed;

	[Header("Copy Properties")]
	public KeyCode possessKey = KeyCode.E;
	public float copyDistance;
	public GameObject copiedMonster;
	public KeyCode metamorph = KeyCode.Tab;
	public GameObject[] switchForms;
    CharacterController baseController;


	void Start()
	{
        baseController = gameObject.GetComponent<CharacterController>(); //Faire en sorte qu'il prenne ses propriétés sans devenir le CharacterController
        cc = gameObject.GetComponent<CharacterController>();
		ExecuteCopy(this.gameObject);
        walkSpeed = GetComponent<CharacterManager>().walkSpeed;
        runSpeedAdd = GetComponent<CharacterManager>().runSpeedAdd;
        jumpForce = GetComponent<CharacterManager>().jumpForce;
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(possessKey))
		{
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				print("I'm looking at " + hit.transform.name);
				Vector3 offset = hit.transform.position - transform.position;
				if (offset.sqrMagnitude < copyDistance * copyDistance && hit.transform.tag == "DuplicableMonster")
				{
					//Vérifier que le monstre possède les spécificités recquises pour être contrôlé
					Debug.Log("Monster successfully copied !");
					copiedMonster = hit.transform.gameObject;
				}
			}
		}
		
		//Métamorphose
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (switchForms[0].activeSelf == false)
			{
				//On désactive toutes les formes de monstre
				foreach (GameObject forms in switchForms)
				{
					forms.SetActive(false);
				}
                cc = baseController;
                ExecuteCopy(switchForms[0]);
				switchForms[0].SetActive(true); //On réactive la forme humaine
				OldSkillManager.ActualForm = "Human"; //On set à nouveau les skills à ceux d'un humain
			} 
			else 
			{
				//Si le joueur a copié un monstre, on lui permet de se métamorphoser en ce dernier
				if (copiedMonster != null)
				{
					ExecuteCopy(copiedMonster);
					foreach (GameObject forms in switchForms)
					{
						forms.SetActive(false);
					}
					switch (copiedMonster.GetComponent<CharacterProperties>()._monsterType)
					{
						case EntityType.Slime :
							switchForms[1].SetActive(true);
							OldSkillManager.ActualForm = "Slime";
						break;

						/*
						
							Ajouter les autres types de monstres
						
						 */
					}	
				}
			}
			
		}
    }

    public void ExecuteCopy(GameObject controlledObject) {
		
		CharacterController controlledController = controlledObject.GetComponent<CharacterController>();

		if (controlledController != null)
		{
			cc.height = controlledController.height;
			cc.radius = controlledController.radius;
			cc.center = controlledController.center;
		}
		else
		{
			Debug.LogError("Can't find component CharacterController on : " + controlledObject);
		}

		if (controlledObject.GetComponent<CharacterProperties>() != null)
		{
			CharacterProperties properties = controlledObject.GetComponent<CharacterProperties>();
			this.jumpForce = properties.jumpForce;
			this.walkSpeed = properties.walkSpeed;
			this.runSpeedAdd = properties.runSpeedAdd;
		} 
		else
		{
			Debug.LogError("Can't find component CharacterProperties on : " + controlledObject);
		}
		
	}
}
