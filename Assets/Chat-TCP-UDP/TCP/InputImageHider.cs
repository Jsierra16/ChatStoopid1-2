using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DualInputFieldImageHider : MonoBehaviour
{
    public TMP_InputField inputField1;
    public TMP_InputField inputField2;
    public GameObject imageToHide;

    private void Start()
    {
        if (inputField1 != null)
            inputField1.onValueChanged.AddListener(delegate { CheckInputs(); });

        if (inputField2 != null)
            inputField2.onValueChanged.AddListener(delegate { CheckInputs(); });

        CheckInputs(); 
    }

    private void CheckInputs()
    {
        if (inputField1 == null || inputField2 == null || imageToHide == null) return;

        bool hasText1 = !string.IsNullOrWhiteSpace(inputField1.text);
        bool hasText2 = !string.IsNullOrWhiteSpace(inputField2.text);

        imageToHide.SetActive(!(hasText1 && hasText2)); 
    }
}
