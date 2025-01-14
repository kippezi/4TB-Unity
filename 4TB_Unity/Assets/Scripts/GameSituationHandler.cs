using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnswerFirst = AnswerHandler.AnswerFirst;
using AnswerSecond = AnswerHandler.AnswerSecond;

public class GameSituationHandler : MonoBehaviour
{
    // Classes for managing game situation
    public enum Phase
    {
        Join,
        Question1,
        ShowFirstQuestionAnswers,
        Question2,
        ShowSecondQuestionAnswers,
        ShowMidScores,
        ShowNextPlayerTurn,
        FinalResults,
        FinishGame
    }

    public struct PlayerResult
    {
        public Player player;
        public string answer;
        public bool isAnswerCorrect;

        public string toString()
        {
            return player.playerData.name + ": " + answer;
        }
    }

    // Whole game parameters
    public Phase phase;
    public int roundsPlayed;

    // Round parameters
    public List<Player> firstQuestionWinners { get; private set; }
    public Player secondQuestionWinner { get; private set; }
    public List<PlayerResult> firstQuestionResults { get; private set; }
    public List<PlayerResult> secondQuestionResults { get; private set; }

    //handlers
    AnswerHandler answerhandler;
    PlayerHandler playerhandler;

    void Start()
    {
        phase = Phase.Join;
        roundsPlayed = 0;

        firstQuestionWinners = new List<Player>();
        secondQuestionWinner = null;
        firstQuestionResults = new List<PlayerResult>();
        secondQuestionResults = new List<PlayerResult>();

        answerhandler = Singleton.Instance.answerhandler;
        playerhandler = Singleton.Instance.playerhandler;
    }

    void Update()
    {
        
    }


    public string FindFirstQuestionRightAnswer()
    {
        string rightAnswer = "";
        int playerInTurnId = playerhandler.GetPlayerInTurn().playerData.id;
        List<AnswerFirst> answers = answerhandler.answersFirstList;

        foreach (AnswerFirst answer in answers)
        {

            if (answer.playerid == playerInTurnId)
            {
                rightAnswer = answer.answer;
            }
        }

        if (!rightAnswer.Equals(""))
        {
            return rightAnswer;
        }
        else
        {
            Debug.Log("FindFirstQuestionRightAnswer has an issue: not finding values");
            return null;
        }


    }

    public int FindSecondQuestionRightAnswer()
    {
        int rightAnswer = -1;
        int winnerId = playerhandler.GetPlayerInTurn().playerData.id;
        List<AnswerSecond> answers = answerhandler.answersSecondList;

        foreach (AnswerSecond answer in answers)
        {
            if (answer.playerid == winnerId)
            {
                rightAnswer = answer.answer;
            }
        }

        if (winnerId != -1 && rightAnswer != -1)
        {
            return rightAnswer;
        }
        else
        {
            Debug.Log("FindRightSecondQuestionRightAnswer has an issue: not finding values");
            return -1;
        }
    }

    public Dictionary<Player, int> getRoundPoints()
    {

        List<Player> allPlayers = playerhandler.playerList;
        Dictionary<Player, int> playerPointsThisRound = new Dictionary<Player, int>();

        for (int i = 0; i < allPlayers.Count; i++)
        {
            int points = 0;
            for (int j = 0; j < firstQuestionWinners.Count; j++)
            {
                Debug.Log("player: " + allPlayers[i].playerData.name);
               // Debug.Log("question winner: " + allPlayers[i].playerData.name);
                if (allPlayers[i].playerData.id == firstQuestionWinners[j].playerData.id)
                {
                    points++;
                }
            }
            if (allPlayers[i].playerData.id == secondQuestionWinner.playerData.id)
            {
                points++;
            }
            playerPointsThisRound.Add(allPlayers[i], points);
        }
        return playerPointsThisRound;
    }

    public void FindFirstQuestionResultsAndAddPlayerPoints()
    {
        List<AnswerFirst> answers = answerhandler.answersFirstList;
        List<Player> allPlayers = playerhandler.GetAllPlayers();
        string questionRightAnswer = FindFirstQuestionRightAnswer();
        List<PlayerResult> playerResults = new List<PlayerResult>();
        Player playerInTurn = playerhandler.GetPlayerInTurn();

        foreach (Player player in allPlayers)
        {
            PlayerResult playerResult;

            foreach (AnswerFirst answer in answers)
            {
                if (player.playerData.id == answer.playerid)
                {
                    playerResult.player = player;
                    playerResult.answer = answer.answer;


                    if (answer.answer.Equals(questionRightAnswer))
                    {
                        playerResult.isAnswerCorrect = true;
                        if (answer.playerid != playerInTurn.playerData.id)
                        {
                            player.AddScore(1);
                            firstQuestionWinners.Add(player);
                            player.playerData.askquestion = 1;
                        }
                    }
                    else
                    {
                        playerResult.isAnswerCorrect = false;
                    }

                    playerResults.Add(playerResult);
                }
            }
        }

        firstQuestionResults = playerResults;

    }

    public void FindSecondQuestionResultsAndAddPlayerPoints()
    {
        List<AnswerSecond> answers = Singleton.Instance.answerhandler.answersSecondList;
        List<Player> allPlayers = Singleton.Instance.playerhandler.GetAllPlayers();
        int questionRightAnswer = FindSecondQuestionRightAnswer();
        List<PlayerResult> playerResults = new List<PlayerResult>();
        string closestAnswer = FindClosestAnswerSecondQuestion(answers, questionRightAnswer);
        Player playerInTurn = playerhandler.GetPlayerInTurn();

        Debug.Log("closest answer: " + closestAnswer);
        foreach (Player player in allPlayers)
        {
            PlayerResult playerResult;

            foreach (AnswerSecond answer in answers)
            {
                string answerString = answer.answer.ToString();
                if (player.playerData.id == answer.playerid)
                {
                    playerResult.player = player;
                    playerResult.answer = answerString;

                    Debug.Log(answer.answer);
                    if (answerString.Equals(closestAnswer))
                    {
                        playerResult.isAnswerCorrect = true;
                        if (answer.playerid != playerInTurn.playerData.id)
                        {
                            secondQuestionWinner = player;
                            player.AddScore(1);
                        }
                    }
                    else
                    {
                        playerResult.isAnswerCorrect = false;
                    }
                    playerResults.Add(playerResult);
                }
            }
        }
        

        secondQuestionResults = playerResults;

    }

    private string FindClosestAnswerSecondQuestion(List<AnswerSecond> answers, int questionRightAnswer)
    {
        int closestAnswer;

        if (answers[0].playerid != playerhandler.GetPlayerInTurn().playerData.id)
        {
          closestAnswer = answers[0].answer;
        }
        else
        {
            closestAnswer = answers[1].answer;
        }
            

        foreach (AnswerSecond answer in answers)
        {
            if(answer.playerid != playerhandler.GetPlayerInTurn().playerData.id)
            {
                int differenceClosestAnswer = Mathf.Abs(questionRightAnswer - closestAnswer);
                int differenceCurrentAnswer = Mathf.Abs(questionRightAnswer - answer.answer);
                if (differenceCurrentAnswer < differenceClosestAnswer)
                {
                    closestAnswer = answer.answer;
                }
            }
          
        }

        return closestAnswer.ToString();
    }


    public void resetRoundParameters()
    {
        firstQuestionWinners = new List<Player>();
        firstQuestionResults = new List<PlayerResult>();
        secondQuestionResults = new List<PlayerResult>();
    }
}

