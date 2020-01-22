using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
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
    public static Skill[] ActualSkills{
        get
        {
            return _actualSkills;
        }
        set 
        {
            _actualSkills = value;
        }
    }
    private static Skill[] _actualSkills = new Skill[3];
    public SkillsManager skillsManager;
    Skill[] skillDictionnary;
    public GameObject[] skillCells;
    public Skill baseSkill;

    // Start is called before the first frame update
    void Start()
    {
        this.skillDictionnary = skillsManager.GetComponent<SkillsManager>().skillDictionnary;
        for (int i = 0; i < _actualSkills.Length; i++)
        {
            _actualSkills[i] = baseSkill;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < skillCells.Length; i++)
        {
            skillCells[i].GetComponent<SkillCell>().SetSkill(_actualSkills[i]);
        }
    }
    
    public void Bind(GameObject targetMonster)
    {
        EntityType monsterType = targetMonster.GetComponent<CharacterProperties>()._monsterType;
        skillsManager.OpenBindMenu(monsterType);
    }
}
