using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

[BoltGlobalBehaviour]
public class NetworkCallbacks : GlobalEventListener
{

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        //UnitSelectionUIManager.instance.Activate();
    }
}
