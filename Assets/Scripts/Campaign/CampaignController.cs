using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class CampaignController : MonoBehaviour
{
    string business_id;
    int balance;
    void Start()
    {
        business_id = PlayerPrefs.GetString("BUSSINES_ID");
        balance = PlayerPrefs.GetInt("BALANCE");
        Debug.Log(balance);
        StartCoroutine(GetCampaigns());
    }
    string uri = "https://oylaa.online/mobil/campaign/";
    IEnumerator GetCampaigns()
	{
     
        WWWForm form = new WWWForm();
        form.AddField("business_id", business_id);

        using (UnityWebRequest www = UnityWebRequest.Post(uri + "get_campagins_business.php", form))
		{
            yield return www.SendWebRequest();
            Debug.Log(www.downloadHandler.text);
            FetchCampaigns(www.downloadHandler.text);
            

		}

    }
    public TextMeshProUGUI b_name;
    public GameObject radioButton;
    public Transform panel;
    string[] camps;
    void FetchCampaigns(string campaigns)
	{
        camps = campaigns.Split('#');
		b_name.text = camps[camps.Length-1];
        Array.Resize(ref camps, camps.Length - 1);

        for(int i=0; i<camps.Length; i++)
		{
            string[] data = camps[i].Split('~');
            GameObject obj = Instantiate(radioButton, panel);
            obj.GetComponent<Toggle>().group = GameObject.Find("RadioGroup").GetComponent<ToggleGroup>();
            obj.transform.GetChild(1).GetComponent<Text>().text = data[0] + " " + data[1] + "% \n " +
                "Coin: " + data[2];

            obj.transform.GetChild(3).GetComponent<Text>().text = data[2];
            obj.transform.GetChild(2).GetComponent<Text>().text = data[3];
            obj.name = i.ToString();
            obj.transform.localPosition = new Vector2(obj.transform.localPosition.x, obj.transform.localPosition.y - 200 * i);
            
        }



    }

   public  void PostCamp()
	{
        for(int i=0; i<camps.Length; i++)
		{
            if (GameObject.Find(i.ToString()).GetComponent<Toggle>().isOn == true)
			{
                string id = GameObject.Find(i.ToString()).transform.GetChild(2).GetComponent<Text>().text;
                string coin = GameObject.Find(i.ToString()).transform.GetChild(3).GetComponent<Text>().text;
                StartCoroutine(PostCampaign(id,coin));
			}
		}


	}
    public GameObject AlertPanel,  Ok;
    public TextMeshProUGUI warningText;
    
    IEnumerator PostCampaign(string id,string coin)
	{
        Debug.Log(balance.ToString() + ">" + coin);
       if(balance>=int.Parse(coin))
        { 
            WWWForm form = new WWWForm();
            form.AddField("business_id", business_id);
            form.AddField("campaign_id", id);
            form.AddField("coin", coin);
            form.AddField("user_id", PlayerPrefs.GetString("USER_ID"));
            using (UnityWebRequest www = UnityWebRequest.Post(uri + "post_campagins_business.php", form))
            {
            
                yield return www.SendWebRequest();
                Debug.Log(www.downloadHandler.text);
                AlertPanel.SetActive(true);
                PlayerPrefs.DeleteKey("BALANCE");
                PlayerPrefs.DeleteKey("BUSINESS_ID");
                
   
             

            }
        }
        else
		{
            warningText.text = "Yeterli coininiz yok! Mevcut Coin: " + balance.ToString();
		}

    }

    public void Menu()
    {
        SceneManager.LoadScene("Main_Scene");
    }
}
