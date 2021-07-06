using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class LoginSceneAnimations : MonoBehaviour
{
    public GameObject loginPanel, registerPanel, topCircles, bottomCircles,warningPanel;
    bool isLoginPage = true;
  
    public TextMeshProUGUI warningText;
    public GameObject warningImg,okImg;
    // Circle animations when starting.
    // Used DOTWeen Library for anims.
    public void StartAnim()
    {
        topCircles.GetComponent<RectTransform>().DOScale(1, 1f);
        bottomCircles.GetComponent<RectTransform>().DOScale(1, 1.5f);
    }


    //Panel slide animations.
    public void PanelSlide()
    {
       

        if (!isLoginPage)
        {
            topCircles.GetComponent<RectTransform>().DOScale(1, 1f);
            bottomCircles.GetComponent<RectTransform>().DOMoveX(1080f, 0.5f);
            loginPanel.GetComponent<RectTransform>().DOLocalMoveX(0, 0.5f);
            registerPanel.GetComponent<RectTransform>().DOLocalMoveX(2000, 0.5f);
            isLoginPage = true;
            
        }
        else
        {
            topCircles.GetComponent<RectTransform>().DOScale(0, 1f);
            bottomCircles.GetComponent<RectTransform>().DOMoveX(-220f, 0.5f);
            loginPanel.GetComponent<RectTransform>().DOLocalMoveX(-2000, 0.5f);
            registerPanel.GetComponent<RectTransform>().DOLocalMoveX(0, 0.5f);
            isLoginPage = false;
        }

     

    }
    //Warning panel
    public void WarningPanelAnim(string wText,bool isOk)
    {     
        #if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
        #endif
        if (isOk)
        {
            warningImg.SetActive(false);
            okImg.SetActive(true);

        }
        else
        {
            okImg.SetActive(false);
            warningImg.SetActive(true);
           
        }

        Vector2 warningPanelOrginalLocation = warningPanel.GetComponent<RectTransform>().transform.position;
        warningText.text = wText;
        warningPanel.GetComponent<RectTransform>().DOMoveY(87.5f, 0.5f);
        warningPanel.GetComponent<RectTransform>().DOMove(warningPanelOrginalLocation, 1f).SetDelay(1.5f);
    }

}
