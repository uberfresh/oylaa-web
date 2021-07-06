using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using ZXing;
using ZXing.QrCode;
using UnityEngine.Networking;

public class AppController : MonoBehaviour
{
    //Declarations
    [Header("Navigation Bar")]
    public Button[] navButtons;
    public TextMeshProUGUI[] navButtonTexts;
    public Image navBar;
    public Color32 yellow, grey;
    [Space(5)]

    [Header("Panels")]
    public GameObject homePanel;
    public GameObject walletPanel;
    public GameObject qrPanel;
    public GameObject oppPanel;
    [Space(5)]

    [Header("Wallet Panel")]
    public RawImage userWalletQr;
    public TextMeshProUGUI refID,currentCoinAmount;
    [Space(5)]

    private string USER_ID;

     // Make application frame 60
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {   //Navbar animation
        navBar.GetComponent<RectTransform>().DOLocalMoveY(0f, 0.5f);
        navButtons[0].GetComponent<Image>().color = yellow;
        
        if (PlayerPrefs.HasKey("USER_ID"))
        USER_ID = PlayerPrefs.GetString("USER_ID");
        // Create QR code for user.
        CreateQR();
        // Get user balance from database
        StartCoroutine(GetLastComment());
        StartCoroutine(Balance());
        StartCoroutine(GetLastSurveys());
        StartCoroutine(GetCampaigns());
       #if PLATFORM_ANDROID || PLATFORM_IOS
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
       #endif

    }

  
    // Panel change animations 
    public void PanelChange(int buttonNum)
    {

        switch (buttonNum)
        {

            case 0:
                homePanel.GetComponent<RectTransform>().DOLocalMoveX(0f, 0.5f);
                oppPanel.GetComponent<RectTransform>().DOLocalMoveX(2000f, 0.5f);
                walletPanel.GetComponent<RectTransform>().DOLocalMoveX(4000f, 0.5f);
                qrPanel.SetActive(false);
                qrPanel.GetComponent<RectTransform>().DOLocalMoveY(-2920f, 0.5f);
                navBar.GetComponent<RectTransform>().DOLocalMoveY(0f, 0.5f);
                break;
            case 1:
                homePanel.GetComponent<RectTransform>().DOLocalMoveX(-2000f, 0.5f);
                oppPanel.GetComponent<RectTransform>().DOLocalMoveX(0f, 0.5f);
                walletPanel.GetComponent<RectTransform>().DOLocalMoveX(4000f, 0.5f);                 
                navBar.GetComponent<RectTransform>().DOLocalMoveY(0f, 0.5f);
                break;
            case 2:
                homePanel.GetComponent<RectTransform>().DOLocalMoveX(-2000f, 0.5f);
                walletPanel.GetComponent<RectTransform>().DOLocalMoveX(4000f, 0.5f);
                oppPanel.GetComponent<RectTransform>().DOLocalMoveX(2000f, 0.5f);
                qrPanel.SetActive(true);
                qrPanel.GetComponent<RectTransform>().DOLocalMoveY(0f, 0.5f);
                navBar.GetComponent<RectTransform>().DOLocalMoveY(-400f, 0.5f);
                #if PLATFORM_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                {
                    Permission.RequestUserPermission(Permission.Camera);
                }
               #endif
                break;
            case 3:
                homePanel.GetComponent<RectTransform>().DOLocalMoveX(-2000f, 0.5f);
                oppPanel.GetComponent<RectTransform>().DOLocalMoveX(2000f, 0.5f);
                walletPanel.GetComponent<RectTransform>().DOLocalMoveX(0f, 0.5f); 
                navBar.GetComponent<RectTransform>().DOLocalMoveY(0f, 0.5f);
                break;

            default:
                break;
        }
        //Change navbar button colors when click navbutton
        if (buttonNum<4)
        {
            for (int i = 0; i < navButtons.Length; i++)
            {
                navButtons[i].GetComponent<Image>().color = navButtonTexts[i].color = grey;
            
            }
         
            navButtons[buttonNum].GetComponent<Image>().color = navButtonTexts[buttonNum].color = yellow;
  
        }
      
    }

   
    #region QR GENERATE
    //Generate user QR with ZXing library.

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }
  
    
    public void CreateQR()
    {
        if(!String.IsNullOrEmpty(USER_ID))
        {
            string user_id = USER_ID;
            refID.text = "REFERANS NUMARASI: \n " + user_id;

            Texture2D myQR = generateQR(user_id);
            userWalletQr.GetComponent<RawImage>().texture = myQR;
        }

    }
    #endregion
    string uri = "https://oylaa.online/mobil/";
    [Header("Last Surveys")]
    public GameObject LSPrefab ;
    public Transform LSContent;
    IEnumerator GetLastSurveys()

    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", USER_ID);

        using (UnityWebRequest www = UnityWebRequest.Post(uri + "get_lastsurveys.php", form))
        {
            yield return www.SendWebRequest();

            if (!www.isNetworkError)
            {   if(www.downloadHandler.text != "0")
                { 
                    string[] data = www.downloadHandler.text.Split('~');
				    for (int i = 0; i < data.Length-1; i++)
				    {
                        GameObject obj = Instantiate(LSPrefab, LSContent);
                    
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data[i];

                    }

                }
                else
				{
                    GameObject.Find("NoLastSurvey").transform.GetComponent<TextMeshProUGUI>().text = "Henüz yapmış olduğunuz bir anket yok :(";
                }

            }


        }
    }

    [Header("Campaigns")]
    public GameObject CampaignPanelPrefab;
    public Transform PanelTransform;
    //Getting campaigns for campaigns page.
    public IEnumerator GetCampaigns()
	{
        using (UnityWebRequest www = UnityWebRequest.Get(uri+"get_campaigns.php"))
		{
            yield return www.SendWebRequest();

            if (www.downloadHandler.text == "0")
            {
                GameObject obj = Instantiate(CampaignPanelPrefab, PanelTransform);
                obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Kampanya bulunamadı!";
            }

            else
            {
                Campaigns(www.downloadHandler.text);
            }

            Debug.Log(www.downloadHandler.text);

        }

    }

    public TextMeshProUGUI commentText, dateText, companyText;
    IEnumerator GetLastComment()
	{
        WWWForm form = new WWWForm();

        form.AddField("user_id", USER_ID);

        using (UnityWebRequest www = UnityWebRequest.Post(uri + "get_lastcomment.php", form))
        {
            yield return www.SendWebRequest();

            string[] data = www.downloadHandler.text.Split('~');
            commentText.text = data[0];
            dateText.text = data[1];
            companyText.text = data[2];


        }

    }
 
    void Campaigns(string c)
	{
        string[] placesCamps = c.Split('#');
        Array.Resize(ref placesCamps, placesCamps.Length - 1);
        for(int i=0; i<placesCamps.Length; i++)
		{
                string[] camp = placesCamps[i].Split('~');
            
                GameObject obj = Instantiate(CampaignPanelPrefab, PanelTransform);
               
                obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = camp[3];
                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = camp[0];
                obj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = camp[2]+ " coin";
                obj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text ="İndirim yüzdesi: "+ camp[1] + "%";
               
        }


	}


    //Get OylaaCoin amount and print to text.
    public IEnumerator Balance()
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", USER_ID);
      

        using (UnityWebRequest www = UnityWebRequest.Post(uri + "balance.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError) {

                currentCoinAmount.text = "N/A";
                Debug.Log(www.error + ':' + www.downloadHandler.text);
            } 

            else
            {
                Debug.Log("Succes"+USER_ID + www.downloadHandler.text);
                currentCoinAmount.text = www.downloadHandler.text;
                
            }

        }

    }

    //Scene changes
    public void LoginScreen()
    {
        if (PlayerPrefs.HasKey("REMEMBER") || PlayerPrefs.HasKey("USER_ID"))
            PlayerPrefs.DeleteAll();

        SceneManager.LoadSceneAsync("Login_Scene");
    }

}
