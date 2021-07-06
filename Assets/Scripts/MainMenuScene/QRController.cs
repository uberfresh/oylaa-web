using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using BarcodeScanner;
using BarcodeScanner.Scanner;
using TMPro;
using BarcodeScanner.Webcam;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class QRController : MonoBehaviour
{
    /*     QR CONTROLLER
       <-------------------> 
     This script  creating QR object on screen,

    */

    //Declarartions
 
    [Header("QR Code Components")]
    public RawImage Image;
    public AudioSource Audio;
    private IScanner BarcodeScanner;
    private float RestartTime;
    [Space(5)]

    [Header("Send Coin Panel Inputs")]
    public TMP_InputField targetRefText;
    public TMP_InputField targetInfoText;
    public TMP_InputField coinAmount;
    public TextMeshProUGUI errorText;

    [Space(5)]
    [Header("Other")]
    public AppController appController;
    public GameObject sendCoinPanel;
    public GameObject okPanel;
    string USER_ID,tRef;




    //Close rotate screen on awake. 
    private void Awake()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }

    #region ScanQR
    //Start scanner on start.
    private void Start()
    {
        CreateScanner();   
    }

    public void CreateScanner()
    {
        //Checking userid and define.
        if (PlayerPrefs.HasKey("USER_ID"))
            USER_ID = PlayerPrefs.GetString("USER_ID");

        // Creating new scanner object and play.
        BarcodeScanner = new Scanner();
        BarcodeScanner.Camera.Play();
        // Creating camera texture on Rawimage if scanner ready.
        BarcodeScanner.OnReady += (sender, arg) =>
        {
            //Getting device angles,scale and setting to texture of image.
            Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
            Image.transform.localScale = BarcodeScanner.Camera.GetScale();
            Image.texture = BarcodeScanner.Camera.Texture;
            
            //Keeping aspect ratio of Image
            var rect = Image.GetComponent<RectTransform>();
            var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
            rect.localScale = rect.localScale * 1.5f;

            RestartTime = Time.realtimeSinceStartup;

        };

      
    }
    
    private void Update()
    {     
        if (BarcodeScanner != null)
        {
            BarcodeScanner.Update();
        }

        // Check if the Scanner need to be started or restarted
        if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
        {
            StartScanner();
            RestartTime = 0;
        }


    }

    
    private void StartScanner()
    {   //Catching value of QR Code
        BarcodeScanner.Scan((barCodeType, barCodeValue) =>
        {
            Debug.Log(barCodeValue);
            BarcodeScanner.Stop();
         
                if (barCodeValue.Length == 8)
                {
                    tRef = barCodeValue;
                    StartCoroutine(GetTargetInfo());
                    RestartTime += Time.realtimeSinceStartup + 1f;

                    //Feedback
                    Audio.Play();
                    #if UNITY_ANDROID || UNITY_IOS
                    Handheld.Vibrate();
                    #endif
                }
                

            else if (barCodeValue.Contains("~"))
             {
           
                string[] data = barCodeValue.Split('~');
                if(PlayerPrefs.HasKey("BUSSINES_ID") || PlayerPrefs.HasKey("SURVEY_ID"))
				{
                    PlayerPrefs.DeleteKey("BUSSINES_ID");
                    PlayerPrefs.DeleteKey("SURVEY_ID");
				}

                PlayerPrefs.SetString("BUSSINES_ID", data[0]);
                PlayerPrefs.SetString("SURVEY_ID", data[1]);

                StartCoroutine(StopCamera(() =>
                {
                    SceneManager.LoadScene("Survey_Scene");
                }));
            }

            else
            {
                
                Debug.Log(barCodeValue);
                if (PlayerPrefs.HasKey("BUSSINES_ID") || PlayerPrefs.HasKey("BALANCE"))
                {
                    
                    PlayerPrefs.DeleteKey("BUSSINES_ID");
                    PlayerPrefs.DeleteKey("BALANCE");
                }

                balance = int.Parse(appController.currentCoinAmount.text.Trim());
                Debug.Log(balance);

                PlayerPrefs.SetString("BUSSINES_ID", barCodeValue);
                PlayerPrefs.SetInt("BALANCE", balance);
                StartCoroutine(StopCamera(() =>
                {
                    SceneManager.LoadScene("Campaign_Scene");
                }));

              
            }


        });
    }
	#endregion

	#region Database 
	
	private string uri = "https://oylaa.online/mobil/";
    public IEnumerator GetTargetInfo()
    {   //Creating new form with Networking Library
        WWWForm form = new WWWForm();
        //Adding filed on form
        form.AddField("user_id",tRef);

        //Getting target id from database with php script.
        using (UnityWebRequest www = UnityWebRequest.Post(uri + "/targetinfo.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError) Debug.Log(www.error + ':' + www.downloadHandler.text);

            else
            {
                Debug.Log("Succes" + USER_ID + www.downloadHandler.text);
                //Activating coin send panel
                sendCoinPanel.SetActive(true);
                targetRefText.text = tRef;
                targetInfoText.text =  www.downloadHandler.text.Trim();             
            }

        }
    }
    private int balance, amount;
    public void sendCoin()
    {
        //Cheking inputs and balance
        if(!String.IsNullOrEmpty(targetRefText.text) && !String.IsNullOrEmpty(coinAmount.text))
        { 

            balance = int.Parse(appController.currentCoinAmount.text.Trim());
            tRef = targetRefText.text;
            amount = int.Parse(coinAmount.text);
        
            if (amount <= balance)
            {
                StartCoroutine(SendCoinCoroutine());
            }
            else
            {
              errorText.text="En fazla" + balance.ToString() + " Oylaa Coin gönderebilirsiniz!";
            }

        }
        else
            errorText.text = "Lütfen gerekli alanları doldurunuz!";





    }
    IEnumerator SendCoinCoroutine()
    {

        // If everything ok send coin to target user.
        // Php script adding values to tbl_user_transaction
        // tbl_user_transaction has trigger
        WWWForm form = new WWWForm();
        form.AddField("user_id", USER_ID);
        form.AddField("target_id", tRef);
        form.AddField("coin_amount",amount);


        using (UnityWebRequest www = UnityWebRequest.Post(uri + "/sendcoin.php", form))
        {
            //Using sendwebrequest to send form
            yield return www.SendWebRequest();
             
            if (www.isNetworkError) Debug.Log(www.error + ':' + www.downloadHandler.text);

            else
            {
                Debug.Log(www.downloadHandler.text);
                string data = www.downloadHandler.text.Trim();
                //Error catching from php code.
                switch (data)
                {
                    case "0":
                        errorText.text = "Sunucu hatası! Lütfen daha sonra tekrar deneyiniz.";
                        break;

                    case "1":
                        okPanel.SetActive(true);
                        appController.StartCoroutine(appController.Balance());
                        Debug.Log("Başarılı:" + tRef + "'e" + amount + "coin gönderildi!");
                        break;

                    case "2":
                        errorText.text = "Alıcı referans numarası yanlış!";
                        break;

                    case "3":
                        errorText.text = "Kendinize coin gönderemezsiniz!";
                        break;

                    default:
                        break;
                }


            }

        } 
    }


    #endregion
    #region Functions
    public void CloseSendCoinPanel()
    {
        okPanel.SetActive(false);
        sendCoinPanel.SetActive(false);
        appController.PanelChange(0);
    }

    public void ManualInput()
    {
         sendCoinPanel.SetActive(true);
       
    }
    public void MaxAmountToText()
    {
        coinAmount.text = appController.currentCoinAmount.text.Trim();
    }


    public IEnumerator StopCamera(Action callback)
    {
        // Stop Scanning
        Image = null;
        BarcodeScanner.Destroy();
        BarcodeScanner = null;

        // Wait a bit
        yield return new WaitForSeconds(0.1f);

        callback.Invoke();
    }
    #endregion
}
