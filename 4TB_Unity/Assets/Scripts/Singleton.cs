using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phase = GameSituationHandler.Phase;


public class Singleton : MonoBehaviour
{
    //TEST FILES

    public TextAsset sessionJSON;
    public TextAsset playersJSON;
    public TextAsset questionsJSON;
    public TextAsset answersFirstJSON_1;
    public TextAsset answersFirstJSON_2;
    public TextAsset answersFirstJSON_3;
    public TextAsset answersFirstJSON_4;
    public TextAsset AnswersSecondJSON_1;
    public TextAsset AnswersSecondJSON_2;
    [HideInInspector] public static Singleton Instance { get; private set; }
    [HideInInspector] public APIHandler apihandler { get; private set; }
    [HideInInspector] public PlayerHandler playerhandler { get; private set; }
    [HideInInspector] public ScreenHandler screenhandler { get; private set; }
    [HideInInspector] public SessionHandler sessionhandler { get; private set; }
    [HideInInspector] public QuestionHandler questionhandler { get; private set; }
    [HideInInspector] public AnswerHandler answerhandler { get; private set; }
    [HideInInspector] public GameSituationHandler gamesituationhandler { get; private set; }
    [HideInInspector] public Timer timer { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        apihandler = GetComponentInChildren<APIHandler>();
        playerhandler = GetComponentInChildren<PlayerHandler>();
        screenhandler = GetComponentInChildren<ScreenHandler>();
        sessionhandler = GetComponentInChildren<SessionHandler>();
        questionhandler = GetComponentInChildren<QuestionHandler>();
        answerhandler = GetComponentInChildren<AnswerHandler>();
        gamesituationhandler = GetComponentInChildren<GameSituationHandler>();
        timer = GetComponentInChildren<Timer>();

    }
    void Start()
    {
        questionhandler.GetLocalQuestions();
        PerformPhase();

    }

    public void NextPhase()
    {
        if (gamesituationhandler.phase == Phase.Join)
        {
            gamesituationhandler.phase = Phase.ShowNextPlayerTurn;
        }
        else if (gamesituationhandler.phase == Phase.ShowNextPlayerTurn)
        {
            gamesituationhandler.phase = Phase.Question1;
        }
        else if (gamesituationhandler.phase == Phase.Question1)
        {
            gamesituationhandler.phase = Phase.ShowFirstQuestionAnswers;
        }
        else if (gamesituationhandler.phase == Phase.ShowFirstQuestionAnswers)
        {
            if (gamesituationhandler.firstQuestionWinners.Count > 1)
            {
                gamesituationhandler.phase = Phase.Question2;
            }
            else
            {
                gamesituationhandler.phase = Phase.ShowMidScores;
            }
        }
        else if (gamesituationhandler.phase == Phase.Question2)
        {
            gamesituationhandler.phase = Phase.ShowSecondQuestionAnswers;
        }
        else if (gamesituationhandler.phase == Phase.ShowSecondQuestionAnswers)
        {
            gamesituationhandler.phase = Phase.ShowMidScores;
        }
        else if (gamesituationhandler.phase == Phase.ShowMidScores)
        {
            // If all questions have not been played start next round
            if (gamesituationhandler.roundsPlayed < questionhandler.GetTotalQuestionAmount())
            {
                gamesituationhandler.phase = Phase.ShowNextPlayerTurn;
            }
            //If this was the final question, go the the final results screen
            else
            {
                gamesituationhandler.phase = Phase.FinalResults;
            }
        }
        else if (gamesituationhandler.phase == Phase.FinalResults)
        {
            gamesituationhandler.phase = Phase.FinishGame;
        }

  
        PerformPhase();
    }

    IEnumerator PrepareJoinScreen()
    {
        apihandler.MakeNewSession();
        yield return new WaitUntil(() => apihandler.sessionDataIsLoaded);
        Debug.Log("session data loaded: " + apihandler.sessionDataIsLoaded);

        sessionhandler.CreateSessionEntry(apihandler.sessionAnswer);
        Debug.Log("session succesfully created");

        screenhandler.SetJoinScreen();
        playerhandler.GetPlayers();
    }
    private void PerformPhase()
    {

        switch (gamesituationhandler.phase)
        {
            case Phase.Join:
                StartCoroutine(PrepareJoinScreen());
                //questionhandler.CommunicateGameQuestionsToAPI();
                timer.StartTimer(5);
                break;
            case Phase.Question1:
                if (gamesituationhandler.roundsPlayed == 0)
                {
                    playerhandler.CancelPlayerFinding();
                    questionhandler.UpdateCurrentQuestionToAPI(true);
                }
                else
                {
                    questionhandler.NextQuestion();
                    questionhandler.UpdateCurrentQuestionToAPI(false);
                }
                sessionhandler.UpdateSessionPhase("first-question");
                screenhandler.SetFirstQuestionScreen();
                answerhandler.GetAnswers("first");
                timer.StartTimer(5);
                break;
            case Phase.ShowFirstQuestionAnswers:
                sessionhandler.UpdateSessionPhase("not-available");
                answerhandler.CancelAnswerFinding();
                gamesituationhandler.FindFirstQuestionResultsAndAddPlayerPoints();
                screenhandler.SetFirstPhaseAnswersScreen();
                timer.StartTimer(5);
                break;
            case Phase.Question2:
                playerhandler.UpdatePlayersOnAPI();
                sessionhandler.UpdateSessionPhase("second-question");
                screenhandler.SetSecondQuestionScreen();
                answerhandler.GetAnswers("second"); ;
                timer.StartTimer(5);
                break;
            case Phase.ShowSecondQuestionAnswers:
                sessionhandler.UpdateSessionPhase("not-available");
                answerhandler.CancelAnswerFinding();
                gamesituationhandler.FindSecondQuestionResultsAndAddPlayerPoints();
                screenhandler.SetSecondPhaseAnswersScreen();
                timer.StartTimer(5);
                break;
            case Phase.ShowMidScores:
                screenhandler.SetMidResultsScreen();
                gamesituationhandler.roundsPlayed++;
                timer.StartTimer(5);
                break;
            case Phase.ShowNextPlayerTurn:
                if (gamesituationhandler.roundsPlayed != 0)
                {
                    answerhandler.ResetRoundParameters();
                    gamesituationhandler.resetRoundParameters();
                    playerhandler.ChangePlayerInTurn();
                    playerhandler.ResetPlayerParametersOnAPI();
                    // UpdatePlayerOnTurnToAPI(true)

                }
                else
                {
                    // UpdatePlayerOnTurnToAPI(false)
                }
                screenhandler.SetNextPlayerScreen();
                timer.StartTimer(5);
                break;
            case Phase.FinalResults:
                screenhandler.SetFinalResultsScreen();
                timer.StartTimer(5);
                break;
            case Phase.FinishGame:
                screenhandler.SetEndSessionScreen();
                break;

        }
    }


    public void FinishGame()
    {
        sessionhandler.DeleteCurrentSession();
    }

}
