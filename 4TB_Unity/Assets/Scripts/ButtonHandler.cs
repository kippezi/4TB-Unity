using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    GameObject gameEnded;
    // Start is called before the first frame update
    void Start()
    {
        gameEnded = GameObject.Find("GameEnded");
        gameEnded.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(handleButton);
    }

    private void handleButton()
    {
        GameObject.Find("Button").SetActive(false);
        gameEnded.SetActive(true);
        Singleton.Instance.sessionhandler.DeleteCurrentSession();
        
    }
}
