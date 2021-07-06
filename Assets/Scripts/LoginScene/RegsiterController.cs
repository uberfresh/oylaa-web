using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System;



[Serializable]
public class RegsiterController : MonoBehaviour
{
    //Declarations
    public TMP_InputField[] registerData; 
    public TMP_InputField registerDataPass2;
    public TextMeshProUGUI gender;
    string[] fields = { "user_name","user_mail","user_password","user_age"};

    public GameObject emailAuthPanel;
    public TMP_InputField authText;
    public GameObject loadingPanel;

    public LoginSceneAnimations Anims;
    //public InputController inputController;

    private void Start()
    {
        //...Char limits...//
        // USERNAME //
        registerData[0].characterLimit = 16;
        // PASSWORDS //
        registerData[2].characterLimit = 20;
        registerDataPass2.characterLimit = 20;
        // BIRTH YEAR //
        registerData[3].characterLimit = 4;
    }

    
    // 0:ok!
    // 1:All fields must be filled.
    // 2:Userinput must  not less or not equal than 4.
    // 3:Password must be greater than 4.
    // 4:Paswwords must be match.
    // 5:Birth age must be correct and user must greater than 13.

    public void Send()
    {
        int warningState = 0;
        //User-friendly controls
        Debug.Log("send");
        bool isNull=false;
        foreach (var data in registerData)
        {   
            if (String.IsNullOrEmpty(data.text))
            {
                warningState = 1; 
                isNull = true;
            }
        }
        if (!isNull) 
        {
            if (registerData[0].text.Length <= 4) warningState = 2;
            else if (registerData[2].text.Length <= 4) warningState = 3;
            else if (registerData[2].text != registerDataPass2.text) warningState = 4;
            else if (int.Parse(registerData[3].text) > DateTime.Now.Year - 13 || int.Parse(registerData[3].text) < 1900) warningState = 5;  
            
        }

       if(warningState>0)
        {
            
            switch (warningState)
            {
                case 1:
                    Anims.WarningPanelAnim("Lütfen tüm alanları doldurunuz!", false);
                    break;
                case 2:
                    Anims.WarningPanelAnim("Kullanıcı adı 4 karakterden büyük olmalıdır!", false);
                    break;
                case 3:
                    Anims.WarningPanelAnim("Şifre 4 karakterden büyük olmalıdır!", false);
                    break;
                case 4:
                    Anims.WarningPanelAnim("Şifreler eşleşmelidir!", false);
                    break;
                case 5:
                    Anims.WarningPanelAnim("Geçerli bir tarih giriniz.\n 13 yaşından küçükler üye olamaz!", false);
                    break;
                default:
                    break;
            }
        
        }
        else if (warningState==0) StartCoroutine(EmailAuth());
    }

  
    //Register to Database with PHP Form
    /* 
     * 0:Database error
     * 1:Ok!
     * 2:Mail in use
     * 3:Mail not valid
     * 4:Username in use  
     * 5:Auth can not send
     */
    string uri = "https://oylaa.online/mobil/";
    string authCode;
    IEnumerator EmailAuth()
    {
        authCode = "";
        loadingPanel.SetActive(true);
        
        Debug.Log("email");

		for (int i = 0; i < 4; i++)
		{
            authCode += UnityEngine.Random.Range(0, 9).ToString();
		}
        

        WWWForm form = new WWWForm();
        form.AddField("user_name", registerData[0].text);
        form.AddField("user_mail", registerData[1].text);
        Debug.Log(registerData[1].text);
        form.AddField("authCode", authCode);
        
        using (UnityWebRequest www = UnityWebRequest.Post(uri + "mail/emailauth.php", form))
		{
            yield return www.SendWebRequest();
 
            if (www.isNetworkError)
			{
                Debug.Log(www.error + ":" + www.downloadHandler.text);
			}   
            else
            {
                switch (www.downloadHandler.text)
                {
                    case "0":
                        Anims.WarningPanelAnim("Sunucu hatası!", false);
                        break;

                    case "1":
                        loadingPanel.SetActive(false);
                        emailAuthPanel.SetActive(true);
                        Debug.Log(www.downloadHandler.text);
                        break;
                    case "2":
                        Anims.WarningPanelAnim("Bu email zaten alınmış!", false);
                        break;
                    case "3":
                        Anims.WarningPanelAnim("Geçersiz email!", false);
                        break;
                    case "4":
                        Anims.WarningPanelAnim("Bu kullanıcı adı zaten alınmış!", false);
                        break;
                    case "5":
                        Anims.WarningPanelAnim("E posta gönderilemedi!", false);
                        break;
                    default:
                        break;

                }
                loadingPanel.SetActive(false);
                Debug.Log( www.downloadHandler.text);

            }
		}

	}

    public void register()
	{

       if(authText.text == authCode.ToString())
		{

            StartCoroutine(Register());
            emailAuthPanel.SetActive(false);
		}
       else
		{
            Anims.WarningPanelAnim("Yanlış kod girdiniz!", false);
		}
	}

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();

        for (int i = 0; i < fields.Length; i++)
        form.AddField(fields[i], registerData[i].text);
        form.AddField("user_gender", gender.text);

        using (UnityWebRequest www = UnityWebRequest.Post(uri +"register/register.php" ,form))
        {
            yield return www.SendWebRequest();
            Debug.Log("sa:" + www.downloadHandler.text);
            if (www.isNetworkError)
            {
                Anims.WarningPanelAnim("Bağlantı hatası lütfen daha sonra tekrar deneyiniz!", false);
                Debug.Log(www.error + ':'+ www.downloadHandler.text);
            }
            else
            {
                if (www.downloadHandler.text == "0" || www.downloadHandler.text == "")
				{
                    Anims.WarningPanelAnim("Sunucu hatası!", false);
                }
                else if (www.downloadHandler.text == "1")
                {
                    Anims.WarningPanelAnim("Hesabınız başarılı bir şekilde oluşturuldu!", true);
                    Anims.PanelSlide();
                  //  inputController.ClearInput();
                    Debug.Log("Success!" + ':' + www.downloadHandler.text);
                }
              
               
            }

        }
    }

    public void CloseAuthPanel()
	{
        emailAuthPanel.SetActive(false);
	}

}
