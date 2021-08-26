using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private int turn;

    public delegate void OnNewTurnStarted();
    public static event OnNewTurnStarted onNewTurnStarted;

    public void OnClickNewTurn()
    {
        turn++;

        if (onNewTurnStarted != null)
        {
            onNewTurnStarted();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
