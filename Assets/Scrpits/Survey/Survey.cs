using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Survey : MonoBehaviour
{

    public List<Question> Questions;
    private GameObject canvasTemplate;
    private Transform contentContainer;
    private Transform buttonNext;

    
    

    private Question currentQuestion;
    private int currentQuestionIndx = 0;

    public delegate void Callback();
    Callback experimentCallback = () => { };

    // Start is called before the first frame update
    void Start()
    {
        canvasTemplate = GameObject.Find("SurveyCanvas");
        contentContainer = canvasTemplate.transform.FindChildRecursive("CanvasContent");
        buttonNext = contentContainer.transform.Find("ButtonNext");

        for (int i = 0; i < Questions.Count; i++)
        {
            Questions[i].Init(contentContainer);
        }

        buttonNext.transform.SetAsLastSibling();
        buttonNext.GetComponent<Button>().onClick.AddListener(onNextClick);
        canvasTemplate.SetActive(false);



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void Init()
    //{

    //}


    void onNextClick()
    {
        var chosenAnswers = currentQuestion.GetAnswers();
        Debug.Log("NextButtonClicked \nAnswers:");
        for (int i=0; i<chosenAnswers.Count; i++)
        {
            Debug.Log(chosenAnswers[i] + "\n");
        }
        
        // save answers here
        currentQuestion.Reset();
        currentQuestion.Show(false);

        // last question
        // wrapup and callback
        if (currentQuestionIndx == Questions.Count - 1) 
        {
            buttonNext.transform.Find("ButtonText").GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
            canvasTemplate.SetActive(false);
            experimentCallback();
            return;
        }
        // second last question
        else if (currentQuestionIndx == Questions.Count - 2) 
        {
            buttonNext.transform.Find("ButtonText").GetComponent<TMPro.TextMeshProUGUI>().text = "Finish";
        }
        //else
        currentQuestionIndx += 1;
        currentQuestion = Questions[currentQuestionIndx];
        currentQuestion.Show(true);
    }


    public void Run(Callback callback)
    {
        canvasTemplate.SetActive(true);
        experimentCallback = callback;
        currentQuestionIndx = 0;
        currentQuestion = Questions[currentQuestionIndx];
        currentQuestion.Show(true);
    }

}
