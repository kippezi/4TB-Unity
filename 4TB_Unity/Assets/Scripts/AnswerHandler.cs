using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using Question = QuestionHandler.Question;


public class AnswerHandler : MonoBehaviour
{
    [System.Serializable]
    public class AnswerFirst
    {
        public int id;
        public string answer;
        public int questionid;
        public int playerid;

    }
    [System.Serializable]
    public class AnswerFirstList
    {
        public AnswerFirst[] answerfirst;

    }

    [System.Serializable]
    public class AnswerSecond
    {
       public int id;
       public int answer;
       public int questionid;
       public int playerid;
    }
    [System.Serializable]
    public class AnswerSecondList
    {
        public AnswerSecond[] answersecond;

    }

    APIHandler apihandler; 
    SessionHandler sessionhandler;
    QuestionHandler questionhandler;

    public List<AnswerFirst> answersFirstList { get; private set; }
    public List<AnswerSecond> answersSecondList { get; private set; }

    Coroutine findAnswers;
    // Start is called before the first frame update
    void Start()
    {
        apihandler = Singleton.Instance.apihandler;
        sessionhandler = Singleton.Instance.sessionhandler;
        questionhandler = Singleton.Instance.questionhandler;

        answersFirstList = new List<AnswerFirst>();
        answersSecondList = new List<AnswerSecond>();
    }

    private IEnumerator FindAnswersFirst()
    {
        while (true)
        {
            QuestionHandler.Question currentQuestion = questionhandler.GetCurrentQuestion();
            int currentQuestionId = currentQuestion.id;
            int sessionId = sessionhandler.session.id;
            Debug.Log("sending data request for answers");
            apihandler.GetFirstAnswers(sessionId, currentQuestionId);
            yield return new WaitUntil(() => apihandler.answerDataIsLoaded);

            Debug.Log("answer data has loaded: " + apihandler.answerDataIsLoaded);
            string answerString = apihandler.answerAnswer;
            AnswerFirst[] allAnswers = JsonUtility.FromJson<AnswerFirstList>(answerString).answerfirst;

            // if this is the first time going through the list
            if (answersFirstList.Capacity == 0)
            {
                answersFirstList = allAnswers.ToList();

            }
            else
            {
                AddNewAnswersFirst(allAnswers);
            }

            yield return new WaitForSecondsRealtime(1);
        }
        
    }

    private IEnumerator FindAnswersSecond()
    {
        while (true)
        {
            QuestionHandler.Question currentQuestion = questionhandler.GetCurrentQuestion();
            int currentQuestionId = currentQuestion.id;
            int sessionId = sessionhandler.session.id;
            apihandler.GetSecondAnswers(sessionId, currentQuestionId);
            yield return new WaitUntil(() => apihandler.answerDataIsLoaded);

            Debug.Log("answer data has loaded: " + apihandler.answerDataIsLoaded);
            string answerString = apihandler.answerAnswer;
            Debug.Log("answer data: " + answerString);
            AnswerSecond[] allAnswers = JsonUtility.FromJson<AnswerSecondList>(answerString).answersecond;

            // if this is the first time going through the list
            if (answersSecondList.Capacity == 0)
            {
                answersSecondList = allAnswers.ToList();
            }
            else
            {
                AddNewAnswersSecond(allAnswers);
            }

            yield return new WaitForSecondsRealtime(1);
        }

    }

   public void GetAnswers(string phase)
    {
        if (phase == "first")
        {
            findAnswers = StartCoroutine(FindAnswersFirst());
        }
        else
        {
            findAnswers = StartCoroutine(FindAnswersSecond());
        }
        
    }

    /*
    private AnswerFirst[] AnswerFirstJSONToAnswer(string jsonString)
    {
        AnswerFirstList newList = JsonUtility.FromJson<AnswerFirstList>(jsonString);
        Debug.Log(jsonString);
        return newList.answerfirst;
    }

    private AnswerSecond[] AnswerSecondJSONToAnswer(string jsonString)
    {
        AnswerSecondList newList = JsonUtility.FromJson<AnswerSecondList>(jsonString);
        Debug.Log(jsonString);
        return newList.answersecond;
    }

    */

    // Finds NEW answers and adds them to the list
    private void AddNewAnswersFirst(AnswerFirst[] newAnswers)
    {
        List<AnswerFirst> originalList = new List<AnswerFirst>(answersFirstList);
 

        for (int i = 0; i < newAnswers.Length; i++)
        {
            // look if the answer is in the new list already exist in the old list
            bool wasFound = false;
            for (int j = 0; j < originalList.Capacity; j++)
            {
                if (originalList[j].id == newAnswers[i].id)
                {
                    wasFound = true;
                    break;
                }
            }
            // if the new answer wasn't found in the old list, add it as a player
            if (!wasFound)
            {
                answersFirstList.Add(newAnswers[i]);
            }
        }
    }

    private void AddNewAnswersSecond(AnswerSecond[] newAnswers)
    {
        List<AnswerSecond> originalList = new List<AnswerSecond>(answersSecondList);
        
        for (int i = 0; i < newAnswers.Length; i++)
        {
            // look if the answer is in the new list already exist in the old list
            bool wasFound = false;
            for (int j = 0; j < originalList.Capacity; j++)
            {
                if (originalList[j].id == newAnswers[i].id)
                {
                    wasFound = true;
                    break;
                }
            }
            // if the new answer wasn't found in the old list, add it as a playerr
            if (!wasFound)
            {
                answersSecondList.Add(newAnswers[i]);
            }
        }

    }

    public void CancelAnswerFinding()
    {
        StopCoroutine(findAnswers);
    }

    public void ResetRoundParameters()
    {
        answersFirstList = new List<AnswerFirst>();
        answersSecondList = new List<AnswerSecond>();
    }
}
