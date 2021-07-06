using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Android;
using UnityEngine.Networking;
public class LoginController : MonoBehaviour
{
    //Declarations

    public LoginSceneAnimations Anims;
    public Toggle rememberMe;
   

    public TMP_InputField user_name, user_password;
    string uri = "https://oylaa.online/mobil/";

    public GameObject resetPassPanel, resetPassPanelNext, resetPassPanelNew;
    public TMP_InputField resetPassUname,resetAuthCode,resetPassText;
    public GameObject loadingPanel;


    //Send user data to php script.
    /* 0: Wrong username or password
     * 1:OK!
       2:Network conn error
    */
    IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("user_name", user_name.text);
        form.AddField("user_password", user_password.text);

        using (UnityWebRequest www = UnityWebRequest.Post(uri + "register/login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error + ':' + www.downloadHandler.text);
              
                Anims.WarningPanelAnim("Bağlantı hatası lütfen daha sonra tekrar deneyin!", false);

            }
            else
            {
                Anims.WarningPanelAnim("Başarılı giriş!", true);
                Debug.Log("Succes" + www.downloadHandler.text);
                if (www.downloadHandler.text.Length > 1)
                {
                    var data = www.downloadHandler.text.Split(';');


                    if (data[0] == "1")
                    {


                        if (rememberMe.isOn)
                             PlayerPrefs.SetInt("REMEMBER", 1);

                        PlayerPrefs.SetString("USER_ID", data[1]);
                        SceneManager.LoadSceneAsync("Main_Scene");

                    }

                }
                else if (www.downloadHandler.text == "0") Anims.WarningPanelAnim("Kullanıcı adı veya şifre yanlış!!", false);
                else Anims.WarningPanelAnim("Sunucu Hatası!", false);

            }

        }

    }
    //Login Button
      public void MainMenu()
    {
        StartCoroutine(Login());
    }

	#region ResetPassword
	
    //Resetpass button
	public void OpenResetPassPanel()
	{
        resetPassPanel.SetActive(true);
	}


    //Reset Pass
    public void resetPassword()
    {
        if (!string.IsNullOrEmpty(resetPassUname.text))
        {
            StartCoroutine(ResetEmailAuth());
        }
        else
        {
            Anims.WarningPanelAnim("E posta alanını boş bırakmayınız!",false);
        }

    }

    string authCode;
    //Create auth code and send to email
    IEnumerator ResetEmailAuth()
    {
        authCode = "";
        loadingPanel.SetActive(true);

        Debug.Log("email");

        for (int i = 0; i < 4; i++)
        {
            authCode += UnityEngine.Random.Range(0, 9).ToString();
        }


        WWWForm form = new WWWForm();
        form.AddField("user_name", resetPassUname.text);
        form.AddField("authCode", authCode);

        using (UnityWebRequest www = UnityWebRequest.Post(uri + "/mail/resetPassAuth.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
             
                Anims.WarningPanelAnim("Sunucu hatası!", false);
                resetPassPanel.SetActive(false);
            }
            else
            {
                if(www.downloadHandler.text == "2")
				{
                    Anims.WarningPanelAnim("Kullanıcı adınız yanlış olabilir!", false);
                    resetPassPanel.SetActive(false);
                }
                
                else if (www.downloadHandler.text == "1")
                    resetPassPanelNext.SetActive(true);
                
                else if (www.downloadHandler.text == "0")
				{
                    Anims.WarningPanelAnim("Sunucu hatası", false);
                    resetPassPanel.SetActive(false);
                }

                
            }
            loadingPanel.SetActive(false);
            Debug.Log(www.error + ":" + www.downloadHandler.text);
        }

    }

    //Get auth code from user
    public void GetAuthCode()
	{
        if(resetAuthCode.text == authCode )
		{
            resetPassPanelNew.SetActive(true);
		}
        else
		{
            Anims.WarningPanelAnim("Yanlış kod!!", false);
        }


	}

    public void NewPassword()
	{
        StartCoroutine(UpdatePassword());
	}


    //Update password with new
    IEnumerator UpdatePassword()
	{

        WWWForm form = new WWWForm();
        form.AddField("user_pass", resetPassText.text);
        form.AddField("user_name", resetPassUname.text);

        using (UnityWebRequest www = UnityWebRequest.Post(uri + "register/resetPass.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
              
                Anims.WarningPanelAnim("Sunucu hatası!", false);
                resetPassPanel.SetActive(false);
            }
            else
            {
                
                 if (www.downloadHandler.text == "1")
				{
                    resetPassPanel.SetActive(false);
                    Anims.WarningPanelAnim("Şifreniz başarılı bir şekilde değiştirildi!", true);

                }

                else if (www.downloadHandler.text == "0")
                {
                    Anims.WarningPanelAnim("Sunucu hatası", false);
                    resetPassPanel.SetActive(false);
                }


            }

            Debug.Log(www.error + ":" + www.downloadHandler.text);
        }


    }
    public void ClosePassResetPanel()
	{

        resetAuthCode.text = "";
        resetPassText.text = "";
        resetPassUname.text = "";

        if(resetPassPanelNew.activeSelf)
        resetPassPanelNew.SetActive(false);
       
        if (resetPassPanelNext.activeSelf) 
        resetPassPanelNext.SetActive(false);
        
        resetPassPanel.SetActive(false);
	}


    #endregion

}
