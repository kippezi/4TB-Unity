using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerHandler : MonoBehaviour
{
    // ----------------------------------Player handler helping classes----------------------------------------------
    [System.Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int askquestion;

    }

    [System.Serializable]
    public class PlayerDataList
    {
        public PlayerData[] player;

    }

    // ----------------------------------Class Initiation-------------------------------------------------------------

    APIHandler apihandler;
    SessionHandler sessionhandler;

    public PlayerDataList playerDataList;
    public List<Player> playerList;

    public int playerInTurnIndex;

    Coroutine findNewPlayers;
    void Start()
    {
        playerList = new List<Player>();
        playerDataList = new PlayerDataList();
        playerInTurnIndex = 0;

        apihandler = Singleton.Instance.apihandler;
        sessionhandler = Singleton.Instance.sessionhandler;

    }

    // ----------------------------------Functions-------------------------------------------------------------
    public Player getPlayerInIndex(int index)
    {
        return playerList[index];
    }

    private PlayerDataList createPlayerListFromJSON(string jsonString)
    {
        PlayerDataList newPlayerDataList = JsonUtility.FromJson<PlayerDataList>(jsonString);
        return newPlayerDataList;
    }

    void AddNewPlayer(PlayerData new_player_data)
    {
        string playerName = new_player_data.name;
        GameObject new_player = new GameObject(playerName);
        new_player.transform.parent = this.gameObject.transform;
        new_player.AddComponent<Player>();
        new_player.GetComponent<Player>().playerData = new_player_data;
        new_player.tag = "Player";

        playerList.Add(new_player.GetComponent<Player>());
    }

    // API USING METHODS
    private IEnumerator FindNewPlayers()
    {
        while (true) {
            // Get data from api, wait until the data has loaded
            apihandler.GetPlayers(sessionhandler.session.id);
            Debug.Log("Player data loaded: " + apihandler.playerDataIsLoaded);
            yield return new WaitUntil(() => apihandler.playerDataIsLoaded);
            Debug.Log("Player data loaded: " + apihandler.playerDataIsLoaded);

            string new_players_json = apihandler.playerAnswer;
            Debug.Log(new_players_json);
            PlayerDataList newPlayerDataList = createPlayerListFromJSON(new_players_json);
            
            if (newPlayerDataList.player != null)
            {
                
                List<PlayerData> oldPlayerDataList = new List<PlayerData>();
                if (playerDataList.player != null)
                {
                    oldPlayerDataList.AddRange(playerDataList.player);
                }
                
                playerDataList = newPlayerDataList;

                // Go through player list and add players 
                for (int i = 0; i < playerDataList.player.Length; i++)
                {
                    // if this IS NOT the first time filling the list add only new players
                    if (oldPlayerDataList.Capacity != 0)
                    {
                        // look if the players in the new list already exist in the old list
                        bool wasFound = false;
                        for (int j = 0; j < oldPlayerDataList.Capacity; j++)
                        {
                            if (playerDataList.player[i].id == oldPlayerDataList[j].id)
                            {
                                wasFound = true;
                                break;
                            }
                        }
                        // if the new player wasn't found in the old list, add it as a player
                        if (!wasFound)
                        {
                            AddNewPlayer(playerDataList.player[i]);
                        }
                    }
                    // if this IS the first time filling the list add all players
                    else
                    {
                        AddNewPlayer(playerDataList.player[i]);
                    }

                }


            }

            //TEST STUFF
            List<Player> orderedPlayerList = playerList.OrderBy(player => player.playerData.id).ToList();
            playerList = orderedPlayerList;

            yield return new WaitForSecondsRealtime(1);
        }

      
    }

    public void CancelPlayerFinding()
    {
        StopCoroutine(findNewPlayers);
    }

    public void GetPlayers()
    {
        findNewPlayers = StartCoroutine(FindNewPlayers());
    }

 
    public void UpdatePlayersOnAPI()
    {
        foreach(Player player in playerList)
        {
            if (player.playerData.askquestion == 1)
            {
                string playersJSON = JsonUtility.ToJson(player.playerData);
                apihandler.UpdatePlayers(player.playerData.id, playersJSON);
            }
        }
    }

    public void ResetPlayerParametersOnAPI()
    {
        foreach (Player player in playerList)
        {
            if (player.playerData.askquestion == 1)
            {
                player.playerData.askquestion = 0;
                string playersJSON = JsonUtility.ToJson(player.playerData);
                apihandler.UpdatePlayers(player.playerData.id, playersJSON);
            }
        }
    }

    // IN GAME METHOds

    public void ChangePlayerInTurn()
    {
        playerList[playerInTurnIndex].onTurn = false;
        
        playerInTurnIndex++;
        playerList[playerInTurnIndex].onTurn = true;
    }

    public Player GetPlayerInTurn()
    {
        return playerList[playerInTurnIndex];
    }

   
    public List<Player> GetAllPlayers() {
        return playerList;
    }

    public Player GetPlayerWithId(int playerId)
    {
        foreach(Player player in playerList)
        {
            if (player.playerData.id == playerId)
            {
                return player;
            }
        }

        return null;
    }
    //-----------------------------------------Not active functions, usable for final product--------------------------------------
    /*
        public void AddPlayerPoints(string question)
        {
            List<Player> playersWon;
            if (question.Equals("first"))
            {
                playersWon = Singleton.Instance.gamesituationhandler.firstQuestionWinners;
            }else {
                playersWon = Singleton.Instance.gamesituationhandler.secondQuestionWinner;
            }


            foreach (Player player in playerList)
            {
                foreach (Player playerWon in playersWon)
                {
                    if(player.playerData.id == playerWon.playerData.id)
                    {
                        player.AddScore(1);
                    }
                }
            }
        }
    */

    /*
public void UpdatePlayerOnTurnToAPI(bool isFirstTurn)
{

    if (!isFirstTurn)
    {
        Player previousPlayer = playerList[playerInTurnIndex - 1];
        string previousPlayerJSON = JsonUtility.ToJson(previousPlayer.playerData);
        apihandler.UpdatePlayers(previousPlayer.playerData.id, previousPlayerJSON);
    }

    Player currentPlayer = playerList[playerInTurnIndex];
    string currentPlayerJSON = JsonUtility.ToJson(currentPlayer.playerData);
    apihandler.UpdatePlayers(currentPlayer.playerData.id, currentPlayerJSON);



}
*/


}
