using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellTriggerEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public void OnPointerEnter(PointerEventData eventData){
        SkillsManager.DescriptionActive = true;
        Skill describeSkill = transform.GetComponentInParent<SkillCell>().skillProperties;
        SkillsManager.SetSkillDescription(describeSkill);
    }

    public void OnPointerExit(PointerEventData eventData){
        SkillsManager.DescriptionActive = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (transform.GetComponentInParent<SkillCell>().opacity == 1f)
        {
            SkillsManager.DescriptionActive = false;
            SkillsManager.dragSkill = true;
            SkillsManager.actualDragSkill = transform.GetComponentInParent<SkillCell>().skillProperties;
        }
    }

}
