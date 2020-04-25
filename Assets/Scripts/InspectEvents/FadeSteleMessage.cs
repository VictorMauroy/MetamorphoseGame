using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeSteleMessage : MonoBehaviour
{
    public Image messageImage;
    bool activated = false;
    public float blackScreenMaxOpacity;
    float delayBtwClick;
    public float baseDelayBtwClick = 3f;

    void Start()
    {
        messageImage.gameObject.SetActive(false);
        messageImage.DOFade(0, 0.5f);
        messageImage.transform.GetChild(0).GetComponent<Image>().DOFade(0, 0.5f);
    }

    void Update()
    {
        if(delayBtwClick > 0f)
        {
            delayBtwClick -= Time.deltaTime;
        }

        if(activated && Input.GetMouseButtonDown(0) && delayBtwClick <= 0f)
        {
            MessageDesactivation();
        }
    }

    public void messageActivation()
    {
        if (!activated)
        {
            CharacterManager.canMove = false;
            CharacterManager.canRotateCamera = false;
            messageImage.gameObject.SetActive(true);
            messageImage.DOFade(blackScreenMaxOpacity, 1.5f);
            messageImage.transform.GetChild(0).GetComponent<Image>().DOFade(1, 3f);
            activated = true;
            delayBtwClick = baseDelayBtwClick;
        }
        
    }

    public void MessageDesactivation()
    {
        messageImage.DOFade(0, 2f);
        messageImage.transform.GetChild(0).GetComponent<Image>().DOFade(0, 1f).OnComplete(()=>DesactivationComplete());
        activated = false;
    }

    public void DesactivationComplete()
    {
        messageImage.gameObject.SetActive(false);
        CharacterManager.canMove = true;
        CharacterManager.canRotateCamera = true;
    }
}
