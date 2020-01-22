using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class SkillsManager : MonoBehaviour
{
    [Header("All the skills")]
    public Skill[] skillDictionnary;
    
    [Header("Skills objects")] 
    public GameObject skillBar;
    public GameObject bindMenu;
    Vector3 beginPosBindMenu;
    public GameObject playerSkillCells;
    public Image monsterImage;
    public GameObject monsterSkillItemPrefab;
    private static bool _descriptionActive;
    public static bool DescriptionActive
    {
        get
        {
            return _descriptionActive;
        }
        set
        {
            _descriptionActive = value;
        }
    }
    private static Skill _skillToDescribe;
    public static Skill SkillToDescribe
    {
        get
        {
            return _skillToDescribe;
        }
        set
        {
            _skillToDescribe = value;
        }
    }
    public GameObject skillDescriptionCell;
    public static bool dragSkill;
    
    [Header("Player Skills")]
    public GameObject[] leftPlayerSkills;
    Vector3[] basePosLeftPlSkill;
    public Image dragSkillIcon;
    public static Skill actualDragSkill;
    public GameObject[] raycastedPlayerSkills;

    [Header("Skills data")]
    private static bool _bindMenuOpen;
    public static bool BindMenuOpen
    {
        get
        {
            return _bindMenuOpen;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        bindMenu.SetActive(false);
        beginPosBindMenu = bindMenu.transform.position;
        bindMenu.transform.DOMoveY(500, 0.5f, true);
        bindMenu.transform.GetChild(0).GetComponent<Image>().DOFade(0.3f,0.5f);
        basePosLeftPlSkill = new Vector3[2];
        for (int i = 0; i < leftPlayerSkills.Length; i++)
        {
            basePosLeftPlSkill[i] = leftPlayerSkills[i].transform.position;    
        }
        dragSkill = false;
        dragSkillIcon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _bindMenuOpen)
        {
            CloseBindMenu();
        }

        if (_descriptionActive)
        {
            skillDescriptionCell.transform.DOScale(Vector3.one, 0.2f);
            skillDescriptionCell.SetActive(true);
            if (_skillToDescribe != null)
            {
                skillDescriptionCell.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _skillToDescribe.skillName;
                skillDescriptionCell.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _skillToDescribe.description;    
            }
        }
        else
        {
            skillDescriptionCell.transform.DOScale(Vector3.zero, 0.2f);
            //.OnComplete(()=>skillDescriptionCell.SetActive(false));
            
        }

        foreach (Transform skillBarObject in skillBar.transform)
        {
            if (skillBarObject.GetComponent<SkillCell>().skillProperties == skillDictionnary[3])
            {
                int nbSlimeSkill = 0;
                foreach (Skill playerS in PlayerSkills.ActualSkills)
                {
                    if (playerS.origin == SkillType.Slime)
                    {
                        nbSlimeSkill++;
                    }
                }
                if (nbSlimeSkill >=2)
                {
                    skillBarObject.GetComponent<SkillCell>().SetOpacity(1f);
                } 
                else
                {
                    skillBarObject.GetComponent<SkillCell>().SetOpacity(0.5f);
                }
            }
        }

        if (Input.GetMouseButton(0) && dragSkill)
        {
            dragSkillIcon.gameObject.SetActive(true);
            dragSkillIcon.sprite = actualDragSkill.icon;
            dragSkillIcon.transform.position = Input.mousePosition;
        }
        else
        {
            /* 
                Conditions pour les skills combinés
                Conditions pour les skills verouillés
            */
            //Si on ne maintient pas le bouton gauche de la souris, dragSkill redevient faux
            dragSkill = false;
            bool alreadyPossessed = false;
            foreach (Skill pSkills in PlayerSkills.ActualSkills)
            {
                if (actualDragSkill == pSkills)
                {
                    alreadyPossessed = true;
                }
            }
            if (dragSkillIcon.gameObject.activeSelf && alreadyPossessed == false)
            {
                PointerEventData m_PointerEventData = new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>());
                m_PointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                GetComponent<GraphicRaycaster>().Raycast(m_PointerEventData, results);
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject == raycastedPlayerSkills[0])
                    {
                        PlayerSkills.ActualSkills[0] = actualDragSkill;
                        raycastedPlayerSkills[0].GetComponentInParent<SkillCell>().SetSkill(actualDragSkill);
                    } 
                    else if (result.gameObject == raycastedPlayerSkills[1])
                    {
                        PlayerSkills.ActualSkills[1] = actualDragSkill;
                        raycastedPlayerSkills[1].GetComponentInParent<SkillCell>().SetSkill(actualDragSkill);
                    } 
                    else if (result.gameObject == raycastedPlayerSkills[2])
                    {
                        PlayerSkills.ActualSkills[2] = actualDragSkill;
                        raycastedPlayerSkills[2].GetComponentInParent<SkillCell>().SetSkill(actualDragSkill);
                    }
                }
            }
            dragSkillIcon.gameObject.SetActive(false);
        }
    }

    public void OpenBindMenu(EntityType monsterType){
        // On fait disparaître la barre de skill vers la gauche
        foreach (GameObject skillItem in leftPlayerSkills)
        {
            skillItem.transform.DOMoveX(-120f, 0.5f, true);
        }

        //Apparition du menu "Bind"
        _bindMenuOpen = true;
        bindMenu.SetActive(true);
        bindMenu.transform.DOMove(beginPosBindMenu,0.5f);
        bindMenu.transform.GetChild(0).GetComponent<Image>().DOFade(0.3f,0.5f);
        
        //On affiche les skills du Player dans le bas du menu "Bind"
        for (int i = 0; i < playerSkillCells.transform.childCount; i++)
        {
            playerSkillCells.transform.GetChild(i).GetComponent<SkillCell>().SetSkill(PlayerSkills.ActualSkills[i]);
        }

        //On affiche les skills du monstre cible dans le haut du menu "Bind"
        //Faire un tri afin d'idenfier les skills rare, via "monsterType". Détecter aussi les skills à combinaison en comparant les skills
        // de "monsterType" et du player. 
        //Faire aussi un lock sur les skills ne pouvant être obtenus. (Skill "YOU", combinaisons manquante, skill déjà possédés)
        
        switch (monsterType)
        {
            case EntityType.Slime :
                //On récupère les skills de type "Slime" dans notre dictionnaire
                List<Skill> slimeSkills = new List<Skill>();
                foreach (Skill skillItem in skillDictionnary)
                {
                    if (skillItem.origin == SkillType.Slime)
                    {
                        slimeSkills.Add(skillItem);
                    }
                }
                //On recréé tous les skills de type "Slime" dans le bind Menu
                foreach (Skill slimeSkill in slimeSkills)
                {
                    GameObject slimePower = Instantiate(monsterSkillItemPrefab);
                    slimePower.transform.SetParent(skillBar.transform);
                    slimePower.GetComponent<SkillCell>().SetSkill(slimeSkill);
                }
                
            break;
        }
    }

    public void CloseBindMenu(){
        _bindMenuOpen = false;
        bindMenu.transform.DOMoveY(500,0.5f, true).SetEase(Ease.InSine).OnComplete(()=> bindMenu.SetActive(false));
        bindMenu.transform.GetChild(0).GetComponent<Image>().DOFade(0,0.5f); //Fondu pour le background de BindMenu (position child 0)
        for (int i = 0; i < leftPlayerSkills.Length; i++)
        {
            leftPlayerSkills[i].transform.DOMove(basePosLeftPlSkill[i],0.5f, true);
        }
        foreach (Transform skillBarChild in skillBar.transform)
        {
            Destroy(skillBarChild.gameObject);
        }
    }

    public static void SetSkillDescription(Skill describeSkill){
        _skillToDescribe = describeSkill;
    }
     
}
