using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OldSkillManager : MonoBehaviour
{
    private static string _actualForm; //Actual form of our character, allow us to know what action we can do
    public static string ActualForm {
        get
        {
            return _actualForm;
        }
        set
        {
            if (value != "")
            {
                _actualForm = value;
            }
        }
    }

    private static bool _skillAllowed; //If skill are allowed or not, disable if a dialogBox is opened or animation is playing  
    public static bool SkillAllowed {
        get
        {
            return _skillAllowed;
        }
        set 
        {
            _skillAllowed = value;
        }
    }
    
    [Header("Slime Skills")]
    public float slimeThrowForce;
    public GameObject slimeSubstance;
    [Header("Human Weapon")]
    public Image chargeImage;
    float chargeTime;
    public float chargeSpeed;
    CharacterManager charManager;
    public TMPro.TMP_Text projectileNumberText;
    int projectileNumber;
    public GameObject[] projectilesprefabs;
    public GameObject fronde;

    // Start is called before the first frame update
    void Start()
    {
        _actualForm = "Human";
        if (chargeSpeed == 0f) Debug.Log("SkillManager.cs : float chargeSpeed not set to a correct value");
        _skillAllowed = true;
        if (GetComponent<CharacterManager>() != null)
        {
            charManager = GetComponent<CharacterManager>();
        }
        projectileNumber = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (_skillAllowed)
        {
            switch (_actualForm)
            {
                case "Slime" :
                    if (Input.GetMouseButtonDown(1))
                    {
                        /* //Lancer de glu
                        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            Debug.DrawRay(ray.origin, ray.direction*20f, Color.red, 2f);
                            
                        } */
                        ThrowSlimeBall();
                    }
                break;

                #region "Human Weapon"
                case "Human" :
                    //Chargement de la jauge de puissance de la fronde
                    if (Input.GetMouseButton(1))
                    {
                        charManager.walkSpeed = 5f;
                        if (chargeTime < 1f)
                        {
                            chargeTime += Time.deltaTime * chargeSpeed;
                        } 
                        else
                        {
                            chargeTime = 1f;
                        }
                    } else
                    {
                        charManager.walkSpeed = 10f;
                        if (chargeTime > 0f)
                        {
                            chargeTime -= Time.deltaTime * chargeSpeed * 2f; 
                        } 
                        else
                        {
                            chargeTime = 0f;
                        }
                    }
                    chargeImage.fillAmount = chargeTime;

                    //Défini le type de projectile selon le scroll de la souris
                    if (Input.mouseScrollDelta.y == 1 )
                    {
                        if (projectileNumber < 1)
                        {
                            projectileNumber++;
                        }
                        if (projectileNumber > 1)
                        {
                            projectileNumber = 1;
                        }
                    }
                    else if (Input.mouseScrollDelta.y == -1)
                    {
                        if (projectileNumber > -1)
                        {
                            projectileNumber --;
                        } 
                        if (projectileNumber < -1)
                        {
                            projectileNumber = -1;
                        }
                    }

                    projectileNumberText.text = projectileNumber.ToString();

                    //Lancer d'une munition si la fronde possède un temps de charge supérieur à 0
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (chargeTime > 0f)
                        {
                            GameObject bullet = Instantiate(projectilesprefabs[projectileNumber+1], fronde.transform.position + transform.forward*2f, Quaternion.identity);
                            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
                            bulletRB.AddForce((transform.forward + transform.up * 0.5f) * chargeTime * 20f, ForceMode.VelocityChange);
                        }
                    }
                    
                    
                break;
                #endregion

                case "Golem" :

                break;

                case "Ondine" :

                break;
            }
        }
        
    }

    public void ThrowSlimeBall(){
        GameObject slimeBall = Instantiate(slimeSubstance, transform.position + transform.forward*2f, transform.rotation);
        Rigidbody slimeRB = slimeBall.GetComponent<Rigidbody>();
        slimeRB.AddForce((transform.forward + transform.up) * slimeThrowForce, ForceMode.VelocityChange);
    }
}
