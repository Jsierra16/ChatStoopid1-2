using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DualInputFieldImageHider : MonoBehaviour
{
    // --- REFERENCIAS DE UI ---
    public TMP_InputField inputField1; // Primer campo de texto
    public TMP_InputField inputField2; // Segundo campo de texto
    public GameObject imageToHide;     // Imagen que se ocultará o mostrará

    private void Start()
    {
        // --- CONFIGURAR EVENTOS DE CAMBIO DE TEXTO ---
        // Cada vez que el usuario escriba algo en inputField1, se revisarán ambos campos
        if (inputField1 != null)
            inputField1.onValueChanged.AddListener(delegate { CheckInputs(); });

        // Cada vez que el usuario escriba algo en inputField2, se revisarán ambos campos
        if (inputField2 != null)
            inputField2.onValueChanged.AddListener(delegate { CheckInputs(); });

        // Verificación inicial al iniciar la escena
        CheckInputs(); 
    }

    // --- MÉTODO DE VERIFICACIÓN ---
    // Revisa si ambos campos de texto tienen contenido
    private void CheckInputs()
    {
        // Evita errores si alguna referencia no está asignada
        if (inputField1 == null || inputField2 == null || imageToHide == null) return;

        // Verifica si los campos no están vacíos o con espacios
        bool hasText1 = !string.IsNullOrWhiteSpace(inputField1.text);
        bool hasText2 = !string.IsNullOrWhiteSpace(inputField2.text);

        // Oculta la imagen solo si ambos campos tienen texto
        // Si al menos uno está vacío, la imagen permanece visible
        imageToHide.SetActive(!(hasText1 && hasText2)); 
    }
}
