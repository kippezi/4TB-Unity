using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestionHandler : MonoBehaviour
{
    // -----------------------------Classes to handle JSON data---------------------------
    [System.Serializable]
    public class Question
    {
        public int id;
        public string alternativeA;
        public string alternativeB;

    }
    [System.Serializable]
    public class QuestionList
    {
        public Question[] question;
    }

    // -----------------------Actualy Question Handler-------------------------------------

    [SerializeField] private TextAsset questionsFile;
    public QuestionList localQuestions;
    private APIHandler apihandler;
    public List<Question> gameQuestions;
    public Question currentQuestion;

    private int currentQuestionIndex;
    void Start()
    {
        apihandler = Singleton.Instance.apihandler;
        GetLocalQuestions();
        SetGameQuestions(4);

        currentQuestionIndex = 0;
        currentQuestion = gameQuestions[0];
    } 


    public void GetLocalQuestions()
    {
        string questionsString = questionsFile.ToString();
        localQuestions =  JsonUtility.FromJson<QuestionList>(questionsString);
        
    }

    public int GetTotalQuestionAmount()
    {
        int totalQuestionAmount = gameQuestions.Count;
        return totalQuestionAmount;
    }

    public Question GetCurrentQuestion()
    {
        return gameQuestions[currentQuestionIndex];
    }

    public void NextQuestion()
    {
        int lastIndex = GetTotalQuestionAmount() - 1;
        if(currentQuestionIndex < lastIndex)
        {
            currentQuestionIndex++;
        }else
        {
            Debug.Log("It's the last question! No more questions available");
        }

        currentQuestion = gameQuestions[currentQuestionIndex];
    }

    //DELETE LATER MAYBE this finds the question alternative if the answers are A/B, maybe consider changing them to the actual sentence? 
   
    public void CommunicateGameQuestionsToAPI()
    {
        string questionIds = "";
        int sessionId = Singleton.Instance.sessionhandler.session.id;
        foreach (Question question in gameQuestions)
        {
            questionIds += question.id + ", ";
        }
        questionIds = questionIds.Remove(questionIds.Length - 1, 1);
        string JSONToSend = "\"questions\": {\"sessionid\": " + sessionId + "," + "\"ids\": [" + questionIds + "]}}";

        apihandler.InsertGameQuestion(JSONToSend);
    }
    public string FindCurrentQuestionAlternative()
    {
        string firstQuestionAnswerLetter = Singleton.Instance.gamesituationhandler.FindFirstQuestionRightAnswer();

        Debug.Log(firstQuestionAnswerLetter);
        if (firstQuestionAnswerLetter == "A")
        {
            return currentQuestion.alternativeA;
        }
        else
        {
            return currentQuestion.alternativeB;
        }
    }

    public void UpdateCurrentQuestionToAPI(bool isFirstQuestion)
    {
        if (!isFirstQuestion)
        {
            Question questionbefore = gameQuestions[currentQuestionIndex - 1];
            string previousQuestionData = "{\"iscurrentquestion\": 0}";
            apihandler.UpdateSessionQuestion(previousQuestionData, questionbefore.id);
        }

        Question currentQuestion = gameQuestions[currentQuestionIndex];
        string currentQuestionData = "{\"iscurrentquestion\": 1}";
        apihandler.UpdateSessionQuestion(currentQuestionData, currentQuestion.id);

    }

    private void FindNewQuestions()
    {
        // FINAL VERSION CODE

    }

    private void SetGameQuestions(int amount)
    {
        // FINAL VERSION CODE

        /* LOPULLISEEN VERSIOON
        List<Question> cards = localQuestions.question.ToList();
        List<Question> pickedCards = new List<Question>();
        
        for (int i = 1; i <= amount; i++)
        {
            int cardIndexToPick = Random.Range(0, cards.Count);
            Question pickedQuestion = cards[cardIndexToPick];
            pickedCards.Add(pickedQuestion);
            cards.RemoveAt(cardIndexToPick);
        }
        gameQuestions = pickedCards;
        */
        List<Question> cards = localQuestions.question.ToList();

        // TESTING PURPOSES
        List<Question> orderedQuestionList = cards.OrderBy(card => card.id).ToList();
        gameQuestions = orderedQuestionList;

    }
}
