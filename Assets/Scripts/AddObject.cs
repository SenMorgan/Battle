using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class AddObject : MonoBehaviour
{
    private Button _button;
    private ProgrammManager _programmManagerScript;


    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        _programmManagerScript = FindObjectOfType<ProgrammManager>();
        _button.onClick.AddListener(AddObjectFunction);

        Assert.IsNotNull(_button);
        Assert.IsNotNull(_programmManagerScript);
    }

    void AddObjectFunction()
    {
        _programmManagerScript.scrollView.SetActive(true);
    }
}
