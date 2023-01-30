using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ChooseObject : MonoBehaviour
{
    private ProgrammManager _programmManagerScript;

    private Button _button;

    public GameObject chosenObject;

    // Start is called before the first frame update
    void Start()
    {
        _programmManagerScript = FindObjectOfType<ProgrammManager>();
        Assert.IsNotNull(_programmManagerScript);

        _button = GetComponent<Button>();
        _button.onClick.AddListener(ChooseObjectFunc);
    }

    void ChooseObjectFunc()
    {
        _programmManagerScript.objToSpawn = chosenObject;
        _programmManagerScript.chooseObject = true;
        _programmManagerScript.scrollView.SetActive(false);
    }
}
