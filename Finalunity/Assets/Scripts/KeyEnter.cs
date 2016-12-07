using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeyEnter : MonoBehaviour
{

    public string inputName;
    Button buttonMe;
    InputField inputk;
    // Use this for initialization
    void Start()
    {
        buttonMe = GetComponent<Button>();
        inputk = GetComponent<InputField>();
    }

    void Update()
    {
        if (Input.GetButtonDown(inputName))
        {
            inputk.ActivateInputField();
          //  buttonMe.onClick.Invoke();
        }


    }
}