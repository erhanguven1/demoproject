using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class TurnManager : GlobalEventListener
{
    public static TurnManager instance;
    private void Awake()
    {
        instance = this;
    }

    public delegate void OnNewTurnStarted();
    public static event OnNewTurnStarted onNewTurnStarted;

    public int readyPlayerCount;

    public bool isMyTurn;

    private void Start()
    {
        if (BoltNetwork.IsServer)
        {
            isMyTurn = true;
        }
        GameObject.Find("EndTurnBTN").GetComponent<UnityEngine.UI.Button>().interactable = isMyTurn;
    }

    public void OnClickNewTurn()
    {
        if (!isMyTurn)
        {
            return;
        }

        EndTurn endTurn = EndTurn.Create();
        endTurn.Send();
    }

    public override void OnEvent(EndTurn evnt)
    {
        if (onNewTurnStarted != null)
        {
            onNewTurnStarted();
            isMyTurn = !isMyTurn;
            GameObject.Find("EndTurnBTN").GetComponent<UnityEngine.UI.Button>().interactable = isMyTurn;
        }
    }
}
