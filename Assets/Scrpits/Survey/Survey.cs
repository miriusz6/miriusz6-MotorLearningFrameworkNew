using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public delegate void ExperimentCallback(List<string> answers);
    ExperimentCallback experimentCallback;


    private List<string> answers = new List<string>();

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

        GameObject.Find("SurveyTemplates").SetActive(false);

        buttonNext.transform.SetAsLastSibling();
        buttonNext.GetComponent<Button>().onClick.AddListener(onNextClick);
        canvasTemplate.SetActive(false);



    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private List<string> readQuestionAnswers()
    {
        List<string> chosenAnswers = currentQuestion.GetAnswers();
        List<string> ret = new List<string> ();

        string questionID = Questions.FindIndex(x => x == currentQuestion).ToString();
        for (int i = 0; i < chosenAnswers.Count; i++)
        {
           var answer_txt = chosenAnswers[i];
            string answer = ( questionID + "," + // question ID
                currentQuestion.QuestionType.ToString() + "," + // question type
                currentQuestion.questionText + "," + // question text
                currentQuestion.PossibleAnswers.FindIndex( x => x == answer_txt).ToString() +
                "," + answer_txt); // answer text
            ret.Add(answer); 
        }
        return ret;
    }



    void onNextClick()
    {
        var chosenAnswers = currentQuestion.GetAnswers();
        answers.AddRange(readQuestionAnswers());
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
            experimentCallback(answers);
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


    public void Run(ExperimentCallback callback)
    {
        answers = new List<string>();
        canvasTemplate.SetActive(true);
        experimentCallback = callback;
        currentQuestionIndx = 0;
        currentQuestion = Questions[currentQuestionIndx];
        currentQuestion.Show(true);
    }

}
