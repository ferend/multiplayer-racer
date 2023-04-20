using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumericInputField : MonoBehaviour
{
    private TMP_InputField inputField;

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(string value)
    {
        // Check if the entered value is a number
        if (!float.TryParse(value, out float result))
        {
            // Remove non-numeric characters from the input field
            inputField.text = new string(value.Where(char.IsDigit).ToArray());
        }
    }
}
