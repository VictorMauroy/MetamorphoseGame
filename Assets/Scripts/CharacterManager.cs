using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterManager : MonoBehaviour {

	private static GameObject _player;
	public static GameObject Player
	{
		get
		{
			return _player;
		}
	}

	[Header("Movement")]
	public CharacterController cc;
	public float walkSpeed;
	public KeyCode forward = KeyCode.Z;
	public KeyCode back = KeyCode.S;
	public KeyCode left = KeyCode.Q;
	public KeyCode right = KeyCode.D;
	public KeyCode run = KeyCode.LeftShift;
	public KeyCode jump = KeyCode.Space;
	public KeyCode interactKey = KeyCode.Mouse0;
	float verticalVelocity;
	float runMultiplier;
	public float runSpeedAdd;
	public float jumpForce;
	public float gravityForce;

	[Header("Look")]
	public Transform cameraPivot;
	public float sensitivity;
	public float maxAngle;
	
	[Header("Interact")]
	public float bindDistance;
	public Image bindInteractionImage;
	public Image pullInteractionImage;
	
	[Header("Animations")]
	public Animator humanAnimator;
	public GameObject humanObject;
	Vector3 humanBasePosition;
	float endJumpAnimDelay;

	void Start()
	{
		_player = this.gameObject;
		humanBasePosition = humanObject.transform.localPosition;
		endJumpAnimDelay = 0f;
	}
	
	// Update is called once per frame
	void Update () {

		#region "Camera and Look"
		if (SkillsManager.BindMenuOpen)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		} else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			cameraPivot.localRotation = Quaternion.AngleAxis(
				-Input.GetAxis("Mouse Y")*sensitivity,
				Vector3.right
			) * cameraPivot.localRotation;
			float vAngle = Quaternion.Angle(
				Quaternion.identity,
				cameraPivot.localRotation
			);
			if (vAngle>maxAngle){
				cameraPivot.localRotation = Quaternion.RotateTowards(
					cameraPivot.localRotation,
					Quaternion.identity,
					vAngle - maxAngle
				);
			}
			transform.rotation = Quaternion.AngleAxis(
				Input.GetAxis("Mouse X")*sensitivity,
				transform.up
			) * transform.rotation;
		}
		

		#endregion

		#region "Run and Jump"

		if (Input.GetKey(run) )
		{
			runMultiplier = runSpeedAdd;
			humanAnimator.SetBool("Running", true);
		} else
		{
			runMultiplier = 1f;
			humanAnimator.SetBool("Running", false);
		}

		#region Interact
		
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			if (hit.transform.tag == "DuplicableMonster" && !SkillsManager.BindMenuOpen)
			{
				print("I'm looking at " + hit.transform.name);
				Vector3 offset = hit.transform.position - transform.position;
				
				if (offset.sqrMagnitude < bindDistance * bindDistance)
				{
					bindInteractionImage.gameObject.SetActive(true);
					//Set de la position de l'image d'interaction afin qu'elle soit positionné à côté du monstre 
					bindInteractionImage.rectTransform.position = Camera.main.WorldToScreenPoint(hit.transform.position);
					if (Input.GetKeyDown(interactKey))
					{
						GetComponent<PlayerSkills>().Bind(hit.transform.gameObject);
					}
				}
				pullInteractionImage.gameObject.SetActive(false);	
			} else if(hit.transform.tag == "Crate" && !SkillsManager.BindMenuOpen)
			{
				bindInteractionImage.gameObject.SetActive(false);
				Vector3 offset = hit.transform.position - transform.position;
				if (offset.sqrMagnitude < bindDistance-0.1f * bindDistance-0.1f){
					pullInteractionImage.gameObject.SetActive(true);
				}
				
			} 
			else
			{
				bindInteractionImage.gameObject.SetActive(false);
				pullInteractionImage.gameObject.SetActive(false);
			}
		}

		humanObject.transform.localPosition = humanBasePosition;

		#endregion


		Vector3 move = Vector3.zero;

		// Mouvement avant
		if (Input.GetKey(forward)){
			move+=transform.forward;

			if (Input.GetKey(right))
			{
				humanObject.transform.localRotation = Quaternion.AngleAxis(45f, Vector3.up);	
			}
			
			if(Input.GetKey(left))
			{
				humanObject.transform.localRotation = Quaternion.AngleAxis(-45f, Vector3.up);	
			} 
			
			if (!(Input.GetKey(left) || Input.GetKey(right)))
			{
				humanObject.transform.localRotation = Quaternion.AngleAxis(0f, Vector3.up);	
			}
		}

		// Mouvement arrière
		if (Input.GetKey(back)){
			move-=transform.forward;
			
			
			if (Input.GetKey(right))
			{
				humanObject.transform.localRotation = Quaternion.AngleAxis(135f, Vector3.up);	
			}
			
			if(Input.GetKey(left))
			{
				humanObject.transform.localRotation = Quaternion.AngleAxis(-135f, Vector3.up);	
			} 
			
			if (!(Input.GetKey(left) || Input.GetKey(right)))
			{
				humanObject.transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
			}
		}

		if ((Input.GetKey(back) || Input.GetKey(forward) || Input.GetKey(right) || Input.GetKey(left)))
		{
			humanAnimator.SetBool("MovingForward", true);
		}
		else
		{
		humanAnimator.SetBool("MovingForward", false);
		}

		// Tourner à droite
		if (Input.GetKey(right)){
			move+=transform.right;
			if (!(Input.GetKey(forward) || Input.GetKey(back)) )
			{
				humanObject.transform.localRotation = Quaternion.AngleAxis(90f, Vector3.up);	
			}
		}

		//Tourner à gauche
		if (Input.GetKey(left)){
			move-=transform.right;
			if (! (Input.GetKey(forward) || Input.GetKey(back)) )
			{
				humanObject.transform.localRotation = Quaternion.AngleAxis(-90f, Vector3.up);	
			}
		}

		if(endJumpAnimDelay>0f) endJumpAnimDelay -= Time.deltaTime;

		move.Normalize();
		if (cc.collisionFlags.OnGround() && Input.GetKeyDown(jump)){
			
			verticalVelocity = jumpForce;
			humanAnimator.SetBool("Jumping", true);
			endJumpAnimDelay = 0.3f;
		}
		CollisionFlags cFlags = cc.Move(
			(move * walkSpeed * runMultiplier+Vector3.up * verticalVelocity) * Time.deltaTime
		);
		if (cFlags.OnGround()){
			verticalVelocity = -0.1f;
			if (endJumpAnimDelay <= 0f)
			{
				humanAnimator.SetBool("Jumping", false);
			}
		} else {
			verticalVelocity += Physics.gravity.y*gravityForce*Time.deltaTime;
		}  

		#endregion

	}

}
