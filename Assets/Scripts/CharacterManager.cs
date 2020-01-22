using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	void Start()
	{
		_player = this.gameObject;
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

		if (Input.GetKey(run))
		{
			runMultiplier = runSpeedAdd;
		} else
		{
			runMultiplier = 1f;
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
			} else
			{
				bindInteractionImage.gameObject.SetActive(false);
			}
		}

		#endregion

		// Movement
		Vector3 move = Vector3.zero;
		if (Input.GetKey(forward)){
			move+=transform.forward;
		}
		if (Input.GetKey(back)){
			move-=transform.forward;
		}
		if (Input.GetKey(right)){
			move+=transform.right;
		}
		if (Input.GetKey(left)){
			move-=transform.right;
		}
		move.Normalize();
		if (cc.collisionFlags.OnGround() && Input.GetKeyDown(jump)){
			verticalVelocity = jumpForce;
		}
		CollisionFlags cFlags = cc.Move(
			(move * walkSpeed * runMultiplier+Vector3.up * verticalVelocity) * Time.deltaTime
		);
		if (cFlags.OnGround()){
			verticalVelocity = -0.1f;
		} else {
			verticalVelocity += Physics.gravity.y*gravityForce*Time.deltaTime;
		}

		#endregion

	}

}
