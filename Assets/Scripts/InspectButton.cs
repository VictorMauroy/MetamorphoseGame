using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class InspectButton : MonoBehaviour
{
    [Header("Button elements")]
    Collider detectCollider;
    public Image interactImage;
    public TextMeshProUGUI interactText;
    [Header("Button data")]
    public string interactMessage;
    public Sprite interactSprite;
    public float detectArea;
    Transform player;
    [Header("If the object require a min and maximum rotation.")]
    /*public bool useRotationCaps;
    public float maxRotation;
    public float minRotation;*/
    [Header("Add an event on Player Click")]
    public UnityEvent clickEvent;
    

    // Start is called before the first frame update
    void Start()
    {
        interactText.text = interactMessage;
        interactImage.sprite = interactSprite;
        interactImage.gameObject.SetActive(false);
        detectCollider = GetComponent<Collider>();
        player = CharacterManager.Player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {   
        //On vérifie si l'objet est assez proche pour qu'on affiche le message
        if(Vector3.Distance(player.position, transform.position) < detectArea)
        {
            // Faire en sorte que le message s'affiche devant le player
            transform.LookAt(Camera.main.transform);
            Vector3 eulerLook = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.AngleAxis(180f + eulerLook.y, Vector3.up);
            interactImage.gameObject.SetActive(true);

            if (Input.GetMouseButtonDown(0) && clickEvent != null)
            {
                clickEvent.Invoke();
            }

        }
    }

    private void OnMouseExit()
    {
        interactImage.gameObject.SetActive(false);
    }
}