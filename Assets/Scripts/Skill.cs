using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType{
    Slime,
    Golem,
    Ondine,
    SacredFirefly,
    MiniWizard,
    Combined
}

[CreateAssetMenu(fileName ="New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public Sprite nameBackground;
    public string description;
    public Sprite icon;
    public SkillType origin;
}
