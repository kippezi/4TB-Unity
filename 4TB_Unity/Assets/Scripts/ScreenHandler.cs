using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerResult = GameSituationHandler.PlayerResult;

public class ScreenHandler : MonoBehaviour
{
    /*
    [SerializeField] private Sprite screenJoin;
    [SerializeField] private Sprite screenFirstQuestion;
    [SerializeField] private Sprite screenSecondQuestion;
    [SerializeField] private Sprite screenFinalResults;
    [SerializeField] private Sprite screenMidResults;
    [SerializeField] private Sprite screenNextPlayer;
    [SerializeField] private Sprite[] screens;
    */

    private GameObject screenJoinObject;
    private GameObject screenFirstQuestionObject;
    private GameObject screenSecondQuestionObject;
    private GameObject screenMidResultsObject;
    private GameObject screenNextPlayerObject;
    private GameObject screenMidAnswersObject;
    private GameObject screenFinalResultsObject;
    private GameObject screenEndSession;
    private GameObject[] screenObjects;

    private string screenNameJoin = "ScreenJoin";
    private string screenNameFirstQuestion = "ScreenFirstQuestion";
    private string screenNameSecondQuestion = "ScreenSecondQuestion";
    private string screenNameMidResults = "ScreenMidResults";
    private string screenNameMidAnswers = "ScreenMidAnswers";
    private string screenNameNextPlayer = "ScreenNextPlayer";
    private string screenNameFinalResults = "ScreenFinalResults";
    private string screenNameEndSession = "ScreenEndSession";


    private int index;

    SessionHandler sessionhandler;
    QuestionHandler questionhandler;
    PlayerHandler playerhandler;

    // Start is called before the first frame update
    void Start()
    {
        screenJoinObject = GameObject.Find(screenNameJoin);
        screenFirstQuestionObject = GameObject.Find(screenNameFirstQuestion);
        screenSecondQuestionObject = GameObject.Find(screenNameSecondQuestion);
        screenMidResultsObject = GameObject.Find(screenNameMidResults);
        screenNextPlayerObject = GameObject.Find(screenNameNextPlayer);
        screenFinalResultsObject = GameObject.Find(screenNameFinalResults);
        screenMidAnswersObject = GameObject.Find(screenNameMidAnswers);
        screenEndSession = GameObject.Find(screenNameEndSession);
        screenObjects = new GameObject[] { screenJoinObject, screenFirstQuestionObject, screenSecondQuestionObject, screenMidResultsObject, screenNextPlayerObject, screenFinalResultsObject, screenMidAnswersObject, screenEndSession };

        sessionhandler = Singleton.Instance.sessionhandler;
        questionhandler = Singleton.Instance.questionhandler;
        playerhandler = Singleton.Instance.playerhandler;
    }


    public void SetJoinScreen()
    {
        int sessionId = sessionhandler.session.id;
        GameObject.Find("LinkText").GetComponent<Text>().text = "localhost:3000/joinsession/" + sessionId;
        ShowScreen("ScreenJoin");
       
    }

    public void SetFirstQuestionScreen()
    {
        QuestionHandler.Question currentQuestion = questionhandler.currentQuestion;
        string alternativeA = currentQuestion.alternativeA;
        string alternativeB = currentQuestion.alternativeB;

        ShowScreen(screenNameFirstQuestion);
        GameObject.Find("AlternativeA").GetComponent<Text>().text = "A: " + alternativeA;
        GameObject.Find("AlternativeB").GetComponent<Text>().text = "B: " + alternativeB;
    }

    public void SetSecondQuestionScreen()
    {
        string secondQuestionAlternative = questionhandler.FindCurrentQuestionAlternative();
        ShowScreen(screenNameSecondQuestion);
        GameObject.Find("SecondQuestionAlternative").GetComponent<Text>().text = secondQuestionAlternative;
    }

    public void SetFirstPhaseAnswersScreen()
    {
        string playerAnswers = "";
        List<PlayerResult> playerResults = Singleton.Instance.gamesituationhandler.firstQuestionResults;
        foreach (PlayerResult playerResult in playerResults)
        {
            playerAnswers += playerResult.player.playerData.name + ": " + playerResult.answer + "\n";
        }      
        ShowScreen(screenNameMidAnswers);
        GameObject.Find("PlayerAnswers").GetComponent<Text>().text = playerAnswers;
    }

    public void SetSecondPhaseAnswersScreen()
    {
        string playerAnswers = "";
        List<PlayerResult> playerResults = Singleton.Instance.gamesituationhandler.secondQuestionResults;
        foreach (PlayerResult playerResult in playerResults)
        {
            playerAnswers += playerResult.player + ": " + playerResult.answer + "\n";
        }
        ShowScreen(screenNameMidAnswers);
        //Debug.Log("Player points: " + playerAnswers);
        GameObject.Find("PlayerAnswers").GetComponent<Text>().text = playerAnswers;
    }

    public void SetMidResultsScreen()
    {
        Dictionary<Player, int> playerPointDictionary = Singleton.Instance.gamesituationhandler.getRoundPoints();
        string scoreString = "";
        foreach (var player in playerPointDictionary)
        {
            
            scoreString += player.Key.name + ": " + player.Value + " points\n";

            ShowScreen(screenNameMidResults);
            //Debug.Log("Player points: " + scoreString);
            GameObject.Find("PlayerPoints").GetComponent<Text>().text = scoreString;
        }
    }

    public void SetNextPlayerScreen()
    {
        string nextPlayer = playerhandler.GetPlayerInTurn().playerData.name;
        ShowScreen(screenNameNextPlayer);
        GameObject.Find("ScreenNextPlayer/NextPlayer").GetComponent<Text>().text = nextPlayer;
    }

    public void SetFinalResultsScreen()
    {
        List<Player> allPlayers = playerhandler.GetAllPlayers();
        string playerScores = "";
        foreach (Player player in allPlayers)
        {
            playerScores += player.playerData.name + ": " + player.totalScore + " points\n";
        }
        ShowScreen(screenNameFinalResults);
        GameObject.Find("ScoreText").GetComponent<Text>().text = playerScores;
    }

    public void SetEndSessionScreen()
    {
        ShowScreen(screenNameEndSession);
    }


    //sets the current screen text objects active and all others inactive
    private void ShowScreen(string ScreenToShow)
    {
        foreach (GameObject screenObject in screenObjects)
        {
            if (screenObject.name != ScreenToShow)
            {
                screenObject.SetActive(false);
            }
            else
            {
                screenObject.SetActive(true);
            }
        }
    }

}


