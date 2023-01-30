using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ChooseObject : MonoBehaviour
{
    private ProgrammManager ProgrammManagerScript;

    private Button button;

    public GameObject ChosenObject;

    // Start is called before the first frame update
    void Start()
    {
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>();
        Assert.IsNotNull(ProgrammManagerScript);

        button = GetComponent<Button>();
        button.onClick.AddListener(ChooseObjectFunc);
    }

    void ChooseObjectFunc()
    {
        ProgrammManagerScript.ObjToSpawn = ChosenObject;
        ProgrammManagerScript.ChooseObject = true;
        ProgrammManagerScript.ScrollView.SetActive(false);
    }
}
