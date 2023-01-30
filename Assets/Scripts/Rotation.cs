using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class Rotation : MonoBehaviour
{
    private Button _button;
    private ProgrammManager _programmManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        _programmManagerScript = FindObjectOfType<ProgrammManager>();
        _button.onClick.AddListener(RotationFunction);

        Assert.IsNotNull(_button);
        Assert.IsNotNull(_programmManagerScript);
    }

    void RotationFunction()
    {
        if (_programmManagerScript.Rotation)
        {
            _programmManagerScript.Rotation = false;

            // Change the color of the button
            _button.GetComponent<Image>().color = Color.red;

        }
        else
        {
            _programmManagerScript.Rotation = true;

            // Change the color of the button
            _button.GetComponent<Image>().color = Color.green;
        }
    }
}
