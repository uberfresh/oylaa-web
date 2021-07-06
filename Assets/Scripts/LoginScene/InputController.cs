using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class InputController : MonoBehaviour
{

   //Declarations.
    private TMP_InputField inputField;
    private Image inputFieldImage;
    private bool keepOldTextInField;
    private string oldEditText,editText;

    public Color grey, yellow;


    //Add event listeners to input on start.
    void Start()
    {       //Colors alpha value
            grey.a = 1f;
            yellow.a = 1f;
             
            
            inputFieldImage = gameObject.GetComponent<Image>();
            inputFieldImage.color = grey;     

            inputField = gameObject.GetComponent<TMP_InputField>();    
            inputField.onEndEdit.AddListener(EndEdit);
            inputField.onValueChanged.AddListener(Editing);
            inputField.onTouchScreenKeyboardStatusChanged.AddListener(ReportChangeStatus);
       
    }

    //Catch cancel input on keyboard.
    private void ReportChangeStatus(TouchScreenKeyboard.Status newStatus)
    {
        if (newStatus == TouchScreenKeyboard.Status.Canceled)
            keepOldTextInField = true;  
    }

    
    // Get current text and save old edit text
    private void Editing(string currentText)
    {

        inputFieldImage.color = yellow;
        oldEditText = editText;
        editText = currentText;
     
    }
    //When the editing done, write the text to inputfield. 
    private void EndEdit(string currentText)
    {
        if (keepOldTextInField && !string.IsNullOrEmpty(oldEditText))
        {
            //IMPORTANT ORDER
            editText = oldEditText;
            inputField.text = oldEditText;

            keepOldTextInField = false;
        }
        inputFieldImage.color = grey;
    }
    public void ClearInput()
    {
        inputField.text = "";
    }
    public Image scratch;
    public void PasswordVisibility()
    {
        if (scratch.enabled)
        {
            scratch.enabled = false;
            inputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            scratch.enabled = true;
            inputField.contentType = TMP_InputField.ContentType.Password;
        }
        //Focus password input
        inputField.Select();
        inputField.ActivateInputField(); 
    }
}
