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
    public float pullPushSpeed;
    public float jumpForce;
    public float gravityForce;
    [HideInInspector]
    public float skillSpeedIncrease;
	public static bool canMove;
	public static bool canRotateCamera;

    [Header("Look")]
    public Transform cameraPivot;
    public float sensitivity;
    public float maxAngle;

    [Header("Interact")]
    public float bindDistance;
    public Image bindInteractionImage;
    public Image pullInteractionImage;
    public Image climbInteractionImage;
    bool climbActivated;
    [HideInInspector]
    public GameObject climbWall;
	
	[Header("Animations")]
	public Animator humanAnimator;
	public GameObject humanObject;
	Vector3 humanBasePosition;
	float endJumpAnimDelay;
	public bool pushing;
	public bool pulling;
	public bool canRun;
    public bool climbing = false;
	public bool specialAnimation; //Permet de bloquer les mouvements lors de certaines animations

	private void Awake()
	{
		_player = this.gameObject;
	}

	void Start()
	{
		humanBasePosition = humanObject.transform.localPosition;
		endJumpAnimDelay = 0f;
		canRun = true;
		specialAnimation = false;
        skillSpeedIncrease = 0f;
		canMove = true;
		canRotateCamera = true;
    }
	
	// Update is called once per frame
	void Update () {

		#region "Camera and Look"
		if (SkillsManager.BindMenuOpen)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		} else if(canRotateCamera)
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

		if (Input.GetKey(run) && canRun && !specialAnimation && canMove)
		{
			humanAnimator.SetBool("Running", true);
		} else
		{
			humanAnimator.SetBool("Running", false);
		}

		if (pulling || pushing)
		{
			runMultiplier = pullPushSpeed;
		} 
		else if (Input.GetKey(run))
		{
			runMultiplier = runSpeedAdd;
		} 
		else
		{
			runMultiplier = 1f;
		}
		#endregion

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
			}
			else
			{
				bindInteractionImage.gameObject.SetActive(false);
			}
		}

		humanObject.transform.localPosition = humanBasePosition;

        #endregion

        #region Climbing
        if(climbWall != null)
        {
            if (Vector3.Distance(climbWall.GetComponent<WallInteraction>().activeClimbPositions[0].transform.position, transform.position) < 2f)
            {
                climbActivated = true;
                climbInteractionImage.gameObject.SetActive(true);
            }
            else
            {
                climbActivated = false;
                climbInteractionImage.gameObject.SetActive(false);
            }

            if (climbActivated && !pushing && !pulling && !climbing && Input.GetMouseButtonDown(0))
            {
                climbing = true;
                Climb();
                humanAnimator.SetTrigger("Climbing");
            }

        }
        #endregion

        Vector3 move = Vector3.zero;

		// Mouvement avant
		if (Input.GetKey(forward) && !specialAnimation && canMove)
		{
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
		if (Input.GetKey(back) && !specialAnimation && canMove)
		{
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

		if ((Input.GetKey(back) || Input.GetKey(forward) || Input.GetKey(right) || Input.GetKey(left)) && (!pulling && !pushing) && !specialAnimation && canMove)
		{
			humanAnimator.SetBool("MovingForward", true);
		}
		else
		{
			humanAnimator.SetBool("MovingForward", false);
		}

		if (pushing && !specialAnimation && canMove)
		{
			humanAnimator.SetBool("Pushing", true);
		} else
		{
			humanAnimator.SetBool("Pushing", false);
		}

		if (pulling && !specialAnimation && canMove)
		{
			humanAnimator.SetBool("Pulling", true);
			humanObject.transform.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
		} else
		{
			humanAnimator.SetBool("Pulling", false);
		}

		// Tourner à droite
		if (Input.GetKey(right) && !specialAnimation && canMove)
		{
			move+=transform.right;
			if (!(Input.GetKey(forward) || Input.GetKey(back)) )
			{
				humanObject.transform.localRotation = Quaternion.AngleAxis(90f, Vector3.up);	
			}
		}

		//Tourner à gauche
		if (Input.GetKey(left) && !specialAnimation && canMove)
		{
			move-=transform.right;
			if (! (Input.GetKey(forward) || Input.GetKey(back)) )
			{
				humanObject.transform.localRotation = Quaternion.AngleAxis(-90f, Vector3.up);	
			}
		}

		if(endJumpAnimDelay>0f) endJumpAnimDelay -= Time.deltaTime;

		if (canMove)
		{
			move.Normalize();
			if (cc.collisionFlags.OnGround() && Input.GetKeyDown(jump))
			{
				verticalVelocity = jumpForce;
				humanAnimator.SetBool("Jumping", true);
				endJumpAnimDelay = 0.3f;
			}
			CollisionFlags cFlags = cc.Move(
				(move * (walkSpeed + skillSpeedIncrease) * runMultiplier + Vector3.up * verticalVelocity) * Time.deltaTime
			);
			if (cFlags.OnGround())
			{
				verticalVelocity = -0.1f;
				if (endJumpAnimDelay <= 0f)
				{
					humanAnimator.SetBool("Jumping", false);
				}
			}
			else
			{
				verticalVelocity += Physics.gravity.y * gravityForce * Time.deltaTime;
			}
		}
		

	}

    public void Climb()
    {
        transform.DOMove(
            climbWall.GetComponent<WallInteraction>().activeClimbPositions[0].position, 0.5f, false)
                .OnComplete(() => transform.DOMove(climbWall.GetComponent<WallInteraction>().activeClimbPositions[1].position, 3f, false)
                    .SetEase(Ease.OutSine )
                        .OnComplete(() => climbing = false));
    }

}
