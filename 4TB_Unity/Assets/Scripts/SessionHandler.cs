using UnityEngine;
using System;


public class SessionHandler : MonoBehaviour
{

    public bool sessionIsLoaded;
   
    [System.Serializable]
    public class Session 
    {
        public int id;
        public string phase;
    }

    public Session session;
    APIHandler apihandler;

    void Start()
    {
        apihandler = Singleton.Instance.apihandler;
        sessionIsLoaded = false;
    }

    void Update()
    {
        
    }

    public void CreateSessionEntry(string sessionJSON)
    {
        session = JsonUtility.FromJson<Session>(sessionJSON);
        sessionIsLoaded = true;
    }

    public void UpdateSessionPhase(string phase)
    {
        session.phase = phase;
        string sessionJSON = JsonUtility.ToJson(session);
        Debug.Log("session JSON: " + sessionJSON);
        apihandler.UpdateSessionPhase(session.id, sessionJSON);
    }
    public void DeleteCurrentSession()
    {
       apihandler.DeleteCurrentSession(session.id);
        
    }




}
