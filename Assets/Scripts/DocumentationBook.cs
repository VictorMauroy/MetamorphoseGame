using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DocumentationBook : MonoBehaviour
{
    public Image targetBookPage;
    Image bookBlackBox;
    public float moveOffset = 500f;
    float blackBoxFadeValue = 0.6f;
    float delayBtwClick;
    public float baseDelayBtwClick = 3f;
    bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        bookBlackBox = targetBookPage.transform.parent.GetComponent<Image>();
        bookBlackBox.gameObject.SetActive(false);
        bookBlackBox.DOFade(0f, 0.1f);
        targetBookPage.transform.localPosition += new Vector3(0, moveOffset, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (delayBtwClick > 0f)
        {
            delayBtwClick -= Time.deltaTime;
        }

        if (activated && Input.GetMouseButtonDown(0) && delayBtwClick <= 0f)
        {
            BookDesactivation();
        }
    }

    public void BookActivation()
    {
        if (!activated)
        {
            CharacterManager.canMove = false;
            CharacterManager.canRotateCamera = false;

            bookBlackBox.gameObject.SetActive(true);
            bookBlackBox.DOFade(blackBoxFadeValue, 1.5f);
            targetBookPage.transform.DOLocalMoveY(0f, 2f).SetEase(Ease.InSine);
            
            activated = true;
            delayBtwClick = baseDelayBtwClick;
        }

    }

    public void BookDesactivation()
    {
        bookBlackBox.DOFade(0f, 1.5f);
        targetBookPage.transform.DOLocalMoveY(moveOffset, 1f).SetEase(Ease.OutSine).OnComplete(()=> DesactivationComplete());
        activated = false;
    }

    public void DesactivationComplete()
    {
        bookBlackBox.gameObject.SetActive(false);
        CharacterManager.canMove = true;
        CharacterManager.canRotateCamera = true;
    }
}
