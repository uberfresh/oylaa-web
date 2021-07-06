using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

using UnityEngine.SceneManagement;
public class SurveyManager : MonoBehaviour
{
    // Start is called before the first frame update
    int currentPage = 0;
    string bussines_id, survey_id;
    void Start()
    {
        
        bussines_id = PlayerPrefs.GetString("BUSSINES_ID");
        survey_id = PlayerPrefs.GetString("SURVEY_ID");
        StartCoroutine(GetQuestions());
    }

    public Transform questionContainer;
    public GameObject questionPrefab;
    GameObject questionObject;

    float objposX = -10f;
    string[] questions;
    string uri = "https://oylaa.online/mobil/survey/";
    // Getting question data

    List<string> q_ids = new List<string>();
    private int nameIndex = 0;
    IEnumerator GetQuestions()
    {
        WWWForm form = new WWWForm();
        form.AddField("bussines_id",bussines_id);
        form.AddField("survey_id", survey_id);
        
        // Used uwr GET function
        using (UnityWebRequest www = UnityWebRequest.Post(uri+ "get_questions.php", form))
        {
            
            yield return www.SendWebRequest();
            questions = www.downloadHandler.text.Split('~');
            Debug.Log(www.downloadHandler.text);
            Array.Resize(ref questions, questions.Length - 1);
            
            for (int i = 1; i < questions.Length; i += 2)
                q_ids.Add(questions[i]);
            
            for(int i=0; i<questions.Length; i+=2)
			{
                questionObject = Instantiate(questionPrefab, questionContainer);
                questionObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = questions[i];
                questionObject.name = nameIndex.ToString(); 
                questionObject.transform.position = new Vector2(questionObject.transform.position.x+objposX,questionObject.transform.position.y);
                objposX += 2000f;
                nameIndex++;
			}
        }
        StartCoroutine(GetQuestionTypes());
    }
    
    

    private char[] q_types;
    private string  question_type;
    /*
     Getting question types.
     0: Radiobutton
     1: Checkbox
     2: Open-ended
     
     */
    IEnumerator GetQuestionTypes()
	{
        WWWForm form = new WWWForm();
        form.AddField("bussines_id", bussines_id);
        form.AddField("survey_id", survey_id);
        using (UnityWebRequest www = UnityWebRequest.Post(uri+ "get_questiontypes.php",form))
		{
            yield return  www.SendWebRequest();
            question_type = www.downloadHandler.text;
            q_types =question_type.ToCharArray();
            Debug.Log(question_type);
		}
       
        StartCoroutine(GetAnswers());
    }

    public GameObject radioGroupPrefab,radioButtonPrefab;
    public GameObject checkGroupPrefab,checkButtonPrefab;
    public GameObject openEndedPrefab, openEndedContainerPrefab;
    GameObject answerObject,buttonObject;
    string[] answer,answers;

    // Getting quesiton answers by types.
    IEnumerator GetAnswers()
	{
        WWWForm form = new WWWForm();
        form.AddField("bussines_id", bussines_id);
        form.AddField("survey_id", survey_id);

       
        using (UnityWebRequest www = UnityWebRequest.Post(uri+ "get_answers.php",form))
		{
            yield return www.SendWebRequest();
            Debug.Log(www.downloadHandler.text);
            answers = www.downloadHandler.text.Split('#');
            for (int i = 0; i < answers.Length; i++)
            {
                answer = answers[i].Split('~');
                
                switch (q_types[i])
                {
                    case '0':
                        AddAnswers(radioGroupPrefab,radioButtonPrefab,i);
                        break;
                    case '1':
                        AddAnswers(checkGroupPrefab,checkButtonPrefab,i);
                        break;
                    case '2':
                        AddAnswers(openEndedContainerPrefab,openEndedPrefab,i);
                        break;

                }

			}
		}


    }
    //Adding answers to question panel.
    public void AddAnswers(GameObject groupPrefab,GameObject buttonPrefab,int index)
	{
        groupPrefab.name = "group" + index;
        answerObject = Instantiate(groupPrefab, GameObject.Find(index.ToString()).transform.GetChild(0).transform);
        for (int j = 0; j < answer.Length; j++)
        {
            buttonObject = Instantiate(buttonPrefab, answerObject.transform);
            buttonObject.name = "q." + index + "a." + j; 
           
            buttonObject.transform.localPosition = new Vector2(buttonObject.transform.localPosition.x, buttonObject.transform.localPosition.y - 200 * j);
            if (q_types[index] != '2' )
            {
                buttonObject.transform.GetChild(1).GetComponent<Text>().text = answer[j].Trim();
                buttonObject.transform.GetComponent<Toggle>().isOn = false;
                if (q_types[index] == '0')
                buttonObject.transform.GetComponent<Toggle>().group = GameObject.Find("group" + index + "(Clone)").GetComponent<ToggleGroup>();
            }
		



        }
        
    }
    //Pagination
    public void Next()
	{
        
        if(currentPage < nameIndex-1)
		{
            currentPage++;
            for (int i = 0; i < nameIndex; i++)
            { GameObject panel = GameObject.Find(i.ToString());
               panel.transform.localPosition = new Vector2(panel.transform.localPosition.x - 2000f , panel.transform.localPosition.y);
            }
        }
		else
		{
            GetUserResponse();
        }


     
    }

    public void Previous()
	{
        if (currentPage > 0)
        {
            currentPage--;
            for (int i = 0; i < nameIndex; i++)
            {
                GameObject panel = GameObject.Find(i.ToString());
                panel.transform.localPosition = new Vector2(panel.transform.localPosition.x + 2000f, panel.transform.localPosition.y);
            }
           
        }
    }
  

  //Getting user answers and posting to database. 
    public void GetUserResponse()
	{
        
        for (int i=0; i< q_types.Length; i++)
		{
            MergeAnswers(i);           
		}
        StartCoroutine(IncCoin());
        Instantiate(prizePanel, questionContainer);
        StartCoroutine(LoadMain());
    }
    string data;
    void MergeAnswers(int index)
	{

      answer = answers[index].Split('~');
       
        if (q_types[index] == '0')
        {
            for (int j = 0; j < answer.Length; j++)
            {
                string answerNumber = "q." + index.ToString() + "a." + j.ToString();
                if (GameObject.Find(answerNumber).GetComponent<Toggle>().isOn == true)
                {
                    StartCoroutine(PostAnswers(q_ids[index], question_type[index].ToString(), (j + 1).ToString()));
                }

            }


        }
        
        else if (q_types[index] == '1')
        {
           
            for (int j = 0; j < answer.Length; j++)
            {
                string answerNumber = "q." + index.ToString() + "a." + j.ToString();
                if (GameObject.Find(answerNumber).GetComponent<Toggle>().isOn == true)
                    data += (j+1).ToString() + '~';

             
            }
			data = data.Substring(0, data.Length-1);

            StartCoroutine(PostAnswers(q_ids[index], question_type[index].ToString(), data));
            data = "";
        }

        else if (q_types[index] == '2')
        {
			string answerNumber = "q." + index.ToString() + "a.0";
			string a = GameObject.Find(answerNumber).transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text;
            StartCoroutine(PostAnswers(q_ids[index], question_type[index].ToString(), a));        
        }
       
       
	}


    public GameObject prizePanel;
    IEnumerator PostAnswers(string q_id,string q_type,string ans)
    {
        

        WWWForm form = new WWWForm();

        form.AddField("bussines_id", bussines_id);
        form.AddField("survey_id", survey_id);
        form.AddField("chosen_answer", ans);
        form.AddField("user_id", PlayerPrefs.GetString("USER_ID"));
        form.AddField("q_types", q_type);
        form.AddField("q_id", q_id);

        using (UnityWebRequest www = UnityWebRequest.Post(uri+ "post_answers.php", form))
        {
            yield return www.SendWebRequest();    
        }
      
    }

    //Loading main menu after polling.
   IEnumerator LoadMain()
   {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("Main_Scene");
      
   }
  // Adding 150 coin to user for polling.
  IEnumerator IncCoin()
	{
        WWWForm form = new WWWForm();

        form.AddField("user_id", PlayerPrefs.GetString("USER_ID"));
        using (UnityWebRequest www = UnityWebRequest.Post(uri + "inc_coin.php",form))
        {
            yield return www.SendWebRequest();
        }
    }

}
