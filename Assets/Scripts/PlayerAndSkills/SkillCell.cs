using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillCell : MonoBehaviour
{
    public Image skillImage;
    public Image skillNameImage;
    public TextMeshProUGUI skillNameText;
    public Skill skillProperties;
    public float opacity = 1;
    public void SetSkill(Skill targetSkill){
        skillProperties = targetSkill;
        skillImage.sprite = skillProperties.icon;
        if (skillNameImage != null)
        {
            skillNameImage.sprite = skillProperties.nameBackground;
        }
        if( skillNameText != null) skillNameText.text = skillProperties.skillName;
    }

    public void SetOpacity(float newOpacity){
        Color oldColor = skillImage.color;
        oldColor.a = newOpacity;
        skillImage.color = oldColor;
        opacity = newOpacity;
    }
}
