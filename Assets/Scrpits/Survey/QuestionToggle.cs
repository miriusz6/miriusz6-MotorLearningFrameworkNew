using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionToggle : Question
{

    private GameObject toggleOptionTemplate;
    private List<Toggle> toggles = new List<Toggle>{ };

    private List<Transform> uiContent = new List<Transform>() { };
    private GameObject questionBox;

    void InstantiateOptions()
    {
        for (int i = 0; i < PossibleAnswers.Count; i++)
        {
            GameObject option = GameObject.Instantiate(toggleOptionTemplate);
            option.transform.Find("OptionContent").GetComponent<TMPro.TextMeshProUGUI>().text = PossibleAnswers[i];
            option.transform.SetParent(toggleOptionTemplate.transform);

            Toggle optionToggle = option.transform.Find("OptionToggle").GetComponent<Toggle>();
            //optionToggle.onValueChanged.AddListener(
            //    delegate { onToggleChange(optionToggle);  }
            //);
            toggles.Add(optionToggle);
            uiContent.Add(option.transform);

        }
        

        Show(false);

    }

        // Start is called before the first frame update
    void Start()
    {
        //toggle.onValueChanged.AddListener

        toggleOptionTemplate = GameObject.Find("OptionToggleTemplate");
        toggleOptionTemplate.SetActive(false);


        GameObject questionBoxTemplate = GameObject.Find("QuestionTemplate").gameObject;
        questionBox = GameObject.Instantiate(questionBoxTemplate);
        questionBox.transform.Find("QuestionText").GetComponent<TMPro.TextMeshProUGUI>().text = questionText;
        uiContent.Add(questionBox.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onToggleChange(Toggle toggeled)
    {

    }


    public override List<string> GetAnswers()
    {
        List<string> answeres = new List<string> { };
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].isOn) {
                answeres.Add(PossibleAnswers[i]); 
            }
        }
        return answeres;

    }

    public override bool IsAnswered()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            if ( toggles[i].isOn) { return true; }
        }
        return false;
    }

    public override List<Transform> GetUIContent()
    {
        return uiContent;
    }

    public override void Reset()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].isOn = false;
        }
    }
    
    public override void Init(Transform Container)
    {
        InstantiateOptions();
        for (int i = 0; i < uiContent.Count; i++)
        {
            uiContent[i].SetParent(Container);
        }
    }

    public override void Show(bool visible)
    {
        if (!visible) { Reset(); }
        for (int i = 0; i < uiContent.Count; i++)
        {
            uiContent[i].gameObject.SetActive(visible);
        }
    }
}

