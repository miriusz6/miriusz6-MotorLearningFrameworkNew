using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Question : MonoBehaviour
{
    // Start is called before the first frame update

    public QuestionType QuestionType;

    public int questionTextSize = 20;
    public Color questionTextColor = Color.black;
    public string questionText = "Question text here";

    public List<string> PossibleAnswers;
    public int AnswerTextSize = 20;
    public Color AnswerTextColor = Color.black;



    // private Toggle 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract List<Transform> GetUIContent();

    public abstract bool IsAnswered();

    public abstract List<string> GetAnswers();

    public abstract void Reset();

    public abstract void Init(Transform Container);

    public abstract void Show(bool visible);
}
