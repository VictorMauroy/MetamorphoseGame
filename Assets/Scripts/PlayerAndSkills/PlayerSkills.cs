using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerSkills : MonoBehaviour
{
    private static string _actualForm; //Actual character form, allow us to know what action we can do
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
    public static Skill[] ActualSkills { get; set; } = new Skill[3];

    public SkillsManager skillsManager;
    Skill[] skillDictionnary;
    public GameObject[] skillCells;
    public Skill baseSkill;
    
    [Header("Skills Parameters")]
    float[] keyDelay;
    float cooldownSkillA;
    float cooldownSkillE;
    float cooldownSkillR;
    public GameObject characterObject;
    bool minimised;
    public Image[] cooldownImages;
    public Animator humanAnimator;
    float slimeSpeedActivatedTime = 0;

    [Header("Slime")]
    public float slimeThrowForce;
    public GameObject slimeSubstance;

    [Header("Particles")]
    public ParticleSystem shrinkageParticlesMinimise;
    public ParticleSystem shrinkageParticlesMaximise;
    public GameObject[] slimeSpeedParticles = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {
        keyDelay = new float[3];
        skillDictionnary = skillsManager.GetComponent<SkillsManager>().skillDictionnary;
        for (int i = 0; i < ActualSkills.Length; i++)
        {
            ActualSkills[i] = baseSkill;
        }
        minimised = false;
        shrinkageParticlesMinimise.Stop();
        shrinkageParticlesMaximise.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < skillCells.Length; i++)
        {
            skillCells[i].GetComponent<SkillCell>().SetSkill(ActualSkills[i]);
        }

        //Player Skills activation
        if (Input.GetKeyDown(KeyCode.A) && cooldownSkillA <= 0f)
        {
            SkillActivation(ActualSkills[0], KeyCode.A);
        }

        if (Input.GetKeyDown(KeyCode.E) && cooldownSkillE <= 0f)
        {
            SkillActivation(ActualSkills[1], KeyCode.E);
        }

        if (Input.GetKeyDown(KeyCode.R) && cooldownSkillR <= 0f)
        {
            SkillActivation(ActualSkills[2], KeyCode.R);
        }

        if(cooldownSkillA > 0f) 
        {
            cooldownSkillA -= Time.deltaTime;
            cooldownImages[0].fillAmount = cooldownSkillA/keyDelay[0];
        }  else
        {
            cooldownImages[0].fillAmount = 0f;
        }
        if(cooldownSkillE > 0f)
        {
            cooldownSkillE -= Time.deltaTime;
            cooldownImages[1].fillAmount = cooldownSkillE/keyDelay[1];
        } else
        {
            cooldownImages[1].fillAmount = 0f;
        }
        if(cooldownSkillR > 0f)
        {
            cooldownSkillR -= Time.deltaTime;
            cooldownImages[2].fillAmount = cooldownSkillR/keyDelay[2];
        } else
        {
            cooldownImages[2].fillAmount = 0f;
        }

        if(slimeSpeedActivatedTime > 0)
        {
            slimeSpeedActivatedTime -= Time.deltaTime;
            foreach(GameObject speedParticles in slimeSpeedParticles)
            {
                speedParticles.SetActive(true);
            }
        }
        

        if(slimeSpeedActivatedTime <= 0f /* && Ajouter les autres float qui peuvent modifier la speed */ )
        {
            foreach (GameObject speedParticles in slimeSpeedParticles)
            {
                speedParticles.SetActive(false);
            }
            GetComponent<CharacterManager>().skillSpeedIncrease = 0f;
        }
    }
    
    public void Bind(GameObject targetMonster)
    {
        EntityType monsterType = targetMonster.GetComponent<CharacterProperties>()._monsterType;
        skillsManager.OpenBindMenu(monsterType);
    }

    public void SkillActivation(Skill skillToUse, KeyCode keyToUpdate)
    {
        //On recherche l'index de notre skill
        int skillIndex = 0;
        for (int i = 0; i < skillDictionnary.Length; i++)
        {
            if (skillToUse == skillDictionnary[i])
            {
                skillIndex = i;
            }
        }

        float nextSkillDelay = 0f;

        //On agit en fonction de cet index
        switch (skillIndex)
        {
            //Slime Ball
            case 0 :
                if (humanAnimator.GetComponent<AnimationsFunctions>().throwingBall == false)
                {
                    humanAnimator.SetTrigger("ThrowSpell");
                    humanAnimator.GetComponent<AnimationsFunctions>().done = false;
                    nextSkillDelay = 2.5f;
                }
            break;

            //ElasticBody
            case 1 :
                
                if (minimised)
                {
                    shrinkageParticlesMaximise.Play();
                    characterObject.transform.DOScale(1.5f, 0.3f);
                    characterObject.transform.localPosition = Vector3.zero;
                    GetComponent<CharacterController>().height = 2f;
                    GetComponent<BoxCollider>().size = new Vector3(1.5f, 1f, 1.5f);
                    minimised = false;
                } else
                {
                    shrinkageParticlesMinimise.Play();
                    characterObject.transform.DOScale(0.6f, 0.3f);
                    characterObject.transform.localPosition = new Vector3(0f, -0.15f, 0f);
                    GetComponent<CharacterController>().height = .7f;
                    GetComponent<BoxCollider>().size = new Vector3(1.5f, 2f, 1.5f);
                    minimised = true;
                }
                
                nextSkillDelay = 1f;
            break;

            //Slime Speed
            case 2 :
                slimeSpeedActivatedTime = 10f;
                nextSkillDelay = 14f;
                GetComponent<CharacterManager>().skillSpeedIncrease = 6f;
            break;

            //SlimeMetamorphose
            case 3 :

            break;

            //No Skill
            case 4 :

            break;

            case 5 :

            break;

            case 6 :

            break;

            case 7 :

            break;

            case 8 :

            break;
        }

        switch (keyToUpdate)
        {
            case KeyCode.A :
                keyDelay[0] = nextSkillDelay;
                cooldownSkillA = nextSkillDelay;
            break;

            case KeyCode.E :
                keyDelay[1] = nextSkillDelay;
                cooldownSkillE = nextSkillDelay;
            break;

            case KeyCode.R :
                keyDelay[2] = nextSkillDelay;
                cooldownSkillR = nextSkillDelay;
            break;
        }
    }

    public void ThrowSlimeBall(){
        GameObject slimeBall = Instantiate(slimeSubstance, transform.position + transform.forward*2f + transform.right/1.2f, transform.rotation);
        Rigidbody slimeRB = slimeBall.GetComponent<Rigidbody>();
        slimeRB.AddForce((transform.forward + transform.up) * slimeThrowForce, ForceMode.VelocityChange);
    }
}
