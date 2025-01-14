using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIHandler : MonoBehaviour
{

    private string API_URL = "localhost:3001/api";
    public string sessionAnswer { get; private set; }
    public string playerAnswer { get; private set; }
    public string answerAnswer { get; private set; }
    public string questionAnswer { get; private set; }

    public bool sessionDataIsLoaded { get; private set; }
    public bool playerDataIsLoaded { get; private set; }
    public bool answerDataIsLoaded { get; private set; }
    public bool questionDataIsLoaded { get; private set; }




    //---------------------------------------------------NETWORK USING CLASSES--------------------------------------------------------------------


    // session tools
    public void MakeNewSession()
    {
        sessionAnswer = null;
        string ending = "/session";
        ConnectToAPI("GET", ending, "session");

    }

    public void UpdateSessionPhase(int sessionId, string sessionJSON)
    {
        sessionAnswer = null;
        string ending = "/session/" + sessionId;
        ConnectToAPI("PUT", ending, "session", sessionJSON);

    }

    public void DeleteCurrentSession(int sessionId)
    {
        sessionAnswer = null;
        string ending = "/session/" + sessionId;
        ConnectToAPI("DELETE", ending, "session");

    }

    //player tools
    public void GetPlayers(int sessionId)
    {
        playerAnswer = null;
        string ending = "/player/" + sessionId;
        ConnectToAPI("GET", ending, "player");
    }

    public void UpdatePlayers(int playerid, string data)
    {
        Debug.Log("player data: " + data);
        playerAnswer = null;
        string ending = "/player/" + playerid;
        ConnectToAPI("PUT", ending, "player", data);

    }


    //answer tools
    public void GetFirstAnswers(int sessionId, int questionId)
    {
        answerAnswer = null;
        string ending = "/answerfirst/" + sessionId + "/" + questionId;
        ConnectToAPI("GET", ending, "answer");

    }

    public void GetSecondAnswers(int sessionId, int questionId)
    {
        answerAnswer = null;
        string ending = "/answersecond/" + sessionId + "/" + questionId;
        ConnectToAPI("GET", ending, "answer");

    }

    //question tools
    public void GetAllQuestions()
    {
        answerAnswer = null;
        string ending = "/questions";
        ConnectToAPI("GET", ending, "question");

    }


    // questionsession tools
    public void UpdateSessionQuestion(string data, int questionId)
    {
        questionAnswer = null;
        string ending = "/sessionquestion/" + Singleton.Instance.sessionhandler.session.id + "/" + questionId;
        ConnectToAPI("PUT", ending, "question", data);
        Debug.LogWarning("data " + data);
        Debug.LogWarning("connected to sessionquestion update with the address: " + API_URL + ending);
    }

    public void InsertGameQuestion(string data)
    {
        Debug.Log("Inserting game questions to API");
        questionAnswer = null;
        string ending = "/sessionquestion/";
        ConnectToAPI("POST", ending, data);

    }

    // network tools
    private void ConnectToAPI(string httpRequestType, string ending, string datatype, string data = null)
    {
        string url = API_URL + ending;
        StartCoroutine(ProcessRequest(httpRequestType, url, datatype, data));
    }

    private IEnumerator ProcessRequest(string httpRequestType, string uri, string datatype, string data)
    {
        switch (datatype)
        {
            case "session":
                sessionDataIsLoaded = false;
                break;
            case "player":
                playerDataIsLoaded = false;
                break;
            case "answer":
                answerDataIsLoaded = false;
                break;
            case "question":
                questionDataIsLoaded = false;
                break;
        }
   
        Debug.Log(uri);
        if (String.Equals(httpRequestType, "GET"))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {

                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    switch (datatype)
                    {
                        case "session":
                            sessionAnswer = request.error;
                            break;
                        case "player":
                            playerAnswer = request.error;
                            break;
                        case "answer":
                            answerAnswer = request.error;
                            break;
                        case "question":
                            questionAnswer = request.error;
                            break;
                    }

                }
                else
                {
                    switch (datatype)
                    {
                        case "session":
                            sessionAnswer = request.downloadHandler.text;
                            break;
                        case "player":
                            playerAnswer = request.downloadHandler.text;
                            break;
                        case "answer":
                            answerAnswer = request.downloadHandler.text;
                            break;
                        case "question":
                            questionAnswer = request.downloadHandler.text;
                            break;

                    }

                }
            }   
        }
        else if (String.Equals(httpRequestType, "POST"))
        {
            using (UnityWebRequest request = UnityWebRequest.Post(uri, data))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    switch (datatype)
                    {
                        case "session":
                            sessionAnswer = request.error;
                            break;
                        case "player":
                            playerAnswer = request.error;
                            break;
                        case "answer":
                            answerAnswer = request.error;
                            break;
                        case "question":
                            questionAnswer = request.error;
                            break;
                    }

                    }
                else
                {
                    switch (datatype)
                    {
                        case "session":
                            sessionAnswer = request.downloadHandler.text;
                            break;
                        case "player":
                            playerAnswer = request.downloadHandler.text;
                            break;
                        case "answer":
                            answerAnswer = request.downloadHandler.text;
                            break;
                        case "question":
                            questionAnswer = request.downloadHandler.text;
                            break;

                    }
                    }

            }
        }
        else if (String.Equals(httpRequestType, "DELETE"))
        {
            using (UnityWebRequest request = UnityWebRequest.Delete(uri))
            {

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    switch (datatype)
                    {
                        case "session":
                            sessionAnswer = request.error;
                            break;
                        case "player":
                            playerAnswer = request.error;
                            break;
                        case "answer":
                            answerAnswer = request.error;
                            break;
                        case "question":
                            questionAnswer = request.error;
                            break;
                    }
                    }
                else
                {
                    switch (datatype)
                    {
                        case "session":
                            sessionAnswer = request.downloadHandler.text;
                            break;
                        case "player":
                            playerAnswer = request.downloadHandler.text;
                            break;
                        case "answer":
                            answerAnswer = request.downloadHandler.text;
                            break;
                        case "question":
                            questionAnswer = request.downloadHandler.text;
                            break;

                    }
                    }
            }

        }
        else if (String.Equals(httpRequestType, "PUT"))
        {
            byte[] dataInBytes = System.Text.Encoding.UTF8.GetBytes(data);
            using (UnityWebRequest request = UnityWebRequest.Put(uri, dataInBytes))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    switch (datatype)
                    {
                        case "session":
                            sessionAnswer = request.error;
                            break;
                        case "player":
                            playerAnswer = request.error;
                            break;
                        case "answer":
                            answerAnswer = request.error;
                            break;
                        case "question":
                            questionAnswer = request.error;
                            break;
                    }

                    }
                else
                {
                    switch (datatype)
                    {
                        case "session":
                            sessionAnswer = request.downloadHandler.text;
                            break;
                        case "player":
                            playerAnswer = request.downloadHandler.text;
                            break;
                        case "answer":
                            answerAnswer = request.downloadHandler.text;
                            break;
                        case "question":
                            questionAnswer = request.downloadHandler.text;
                            break;

                    }
                }

            }
        }
        switch (datatype)
        {
            case "session":
                sessionDataIsLoaded = true;
                break;
            case "player":
                playerDataIsLoaded = true;
                break;
            case "answer":
                answerDataIsLoaded = true;
                break;
            case "question":
                questionDataIsLoaded = true;
                break;
        }
        Debug.Log("answer data has loaded in the very end: " + answerDataIsLoaded);

    }



    /*
    
    // -----------------------------------------TEST CLASSES---------------------------------------------------

   
      
    // session tools
    public void MakeNewSession()
    {
        answer =  Singleton.Instance.sessionJSON.text;

    }

    public void UpdateSessionPhase(int sessionId, string phase)
    {

    }

    public void DeleteCurrentSession(int sessionId)
    {
      
    }

    //player tools
    public string GetPlayers(int sessionId)
    {
        return Singleton.Instance.playersJSON.text;
    }


    //answer tools
    public string GetFirstAnswers(int sessionId, int questionId)
    {
        if(questionId == 1)
        {
            return Singleton.Instance.answersFirstJSON_1.text;
        }
        else if (questionId == 2)
        {
            return Singleton.Instance.answersFirstJSON_2.text;
        }
        else if (questionId == 3)
        {
            return Singleton.Instance.answersFirstJSON_3.text;
        }
        else
        {
            return Singleton.Instance.answersFirstJSON_4.text;
        }

    }

    public string GetSecondAnswers(int sessionId, int questionId)
    {
        if (questionId == 1)
        {
            return Singleton.Instance.AnswersSecondJSON_1.text;
        }
        else
        {
            return Singleton.Instance.AnswersSecondJSON_2.text;
        }
    }

    //question tools
    public string getAllQuestions()
    {
        return Singleton.Instance.questionsJSON.text;
    }

   
    public void UpdatePlayers(int playerid, string playerjson)
    {

    }
   */
}

