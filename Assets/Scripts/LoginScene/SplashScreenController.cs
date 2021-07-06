using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
public class SplashScreenController : MonoBehaviour
{
    public Camera cam;
    public Image logo ;
    public TextMeshProUGUI oylaText;
    public LoginSceneAnimations Anim;
  
    public GameObject connCheckPanel, splashPanel;
    
    // Setting app aspect and framerate
    private void Awake()
    {
        Application.targetFrameRate = 60;
        cam.aspect = 16f / 9f;
     
    }   
    void Start()
    {
        logo.GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1f), 1.5f).SetEase(Ease.OutBack);
        oylaText.GetComponent<RectTransform>().DOLocalMoveX(0f, 1.25f).SetEase(Ease.OutBack).OnComplete(() => CheckInternetConn()); 

    }
    public void CheckInternetConn()
    {
        splashPanel.GetComponent<RectTransform>().DOLocalMoveX(-2000f, 1.5f);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            connCheckPanel.SetActive(true);
        }
        else
        {
            if(connCheckPanel != null)
            Destroy(connCheckPanel);
            
            if (PlayerPrefs.HasKey("REMEMBER") && PlayerPrefs.GetInt("REMEMBER") == 1) 
                SceneManager.LoadSceneAsync("Main_Scene");
            else
                StartCoroutine(PanelDestroy());

        }
    }
    //Destroy panel after splashscreen anim end.
    IEnumerator PanelDestroy()
    {      
        yield return new WaitForSeconds(0.5f);
        Anim.StartAnim();
        Destroy(splashPanel,2f);
    }

}
