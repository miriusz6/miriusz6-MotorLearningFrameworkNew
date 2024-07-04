using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionSlider : Question
{
    //new public QuestionType QuestionType = QuestionType.SingleChoice;

    private Slider slider;

    private GameObject sliderOption;
    private GameObject sliderLabels;
    private GameObject startLabel;
    private GameObject middleLabel;
    private GameObject endLabel;
    private GameObject questionBox;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }


    public override List<string> GetAnswers()
    {
        int sliderValue = (int)slider.value;
        return new List<string> { PossibleAnswers[sliderValue] };
    }

    public override bool IsAnswered()
    {
        return true;
    }

    public override List<Transform> GetUIContent()
    {
        return new List<Transform>() { questionBox.transform, sliderOption.transform };
    }

    public override void Reset()
    {
        slider.value = 0;
    }

    public override void Init(Transform Container)
    {
        

        GameObject sliderOptionTemplate = GameObject.Find("OptionSliderTemplate").gameObject;
        GameObject questionBoxTemplate = GameObject.Find("QuestionTemplate").gameObject;
        questionBox = GameObject.Instantiate(questionBoxTemplate);
        questionBox.transform.Find("QuestionText").GetComponent<TMPro.TextMeshProUGUI>().text = questionText;

        sliderOption = GameObject.Instantiate(sliderOptionTemplate);
        slider = sliderOption.transform.Find("OptionSlider").GetComponent<Slider>();
        sliderLabels = sliderOption.transform.Find("OptionLabels").gameObject;

        startLabel = sliderLabels.transform.Find("StartLabel").gameObject;
        middleLabel = sliderLabels.transform.Find("MiddleLabel").gameObject;
        endLabel = sliderLabels.transform.Find("EndLabel").gameObject;

        slider.maxValue = PossibleAnswers.Count - 1;

        startLabel.GetComponent<TMPro.TextMeshProUGUI>().text = PossibleAnswers[0];


        for (int i = 1; i < PossibleAnswers.Count - 1; i++)
        {
            GameObject new_label = GameObject.Instantiate(middleLabel);
            new_label.GetComponent<TMPro.TextMeshProUGUI>().text = PossibleAnswers[i];
            new_label.transform.SetParent(sliderLabels.transform);
            new_label.transform.localPosition = new Vector3();
            new_label.transform.localScale = new Vector3(1, 1, 1);
            new_label.transform.SetAsLastSibling();
        }

        endLabel.GetComponent<TMPro.TextMeshProUGUI>().text = PossibleAnswers[PossibleAnswers.Count - 1];
        endLabel.transform.SetAsLastSibling();

        Destroy(middleLabel);

        //sliderOptionTemplate.SetActive(false);
        Show(false);

        questionBox.transform.SetParent(Container);
        sliderOption.transform.SetParent(Container);

    }

    public override void Show(bool visible)
    {
        if (!visible) { Reset(); }
        sliderOption.SetActive(visible);
        questionBox.SetActive(visible);
    }
}
