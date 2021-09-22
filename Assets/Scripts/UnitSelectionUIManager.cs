using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using Photon.Bolt.Matchmaking;

public class UnitSelectionUIManager : MonoBehaviour
{
    public static UnitSelectionUIManager instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private List<string> selections = new List<string>();
    [SerializeField] private Transform[] teamSpawnLocations = new Transform[2];

    public int teamId;

    private void Start()
    {
        teamId = BoltNetwork.Connections.Count();
        Activate();
    }

    public void Activate()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void TapPlus(string unit)
    {
        if (selections.Count == 3)
        {
            return;
        }
        selections.Add(unit);
    }
    public void TapMinus(string unit)
    {
        selections.Remove(unit);
    }

    public void OnSelectionDone()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        CameraMovement.instance.MoveCameraToPosition(teamSpawnLocations[teamId].GetChild(0).position);
        CameraMovement.instance.RotateTo(teamSpawnLocations[teamId].GetChild(0).eulerAngles);

        for (int i = 0; i < 3; i++)
        {
            GameObject unit = null;

            var spawnPosition = new Vector3(teamSpawnLocations[teamId].position.x + (i - 1) * 7, -1.9f, teamSpawnLocations[teamId].position.z);

            if (selections[i] == "melee")
            {
                unit = BoltNetwork.Instantiate(BoltPrefabs.Melee, spawnPosition, Quaternion.identity);
            }
            if (selections[i] == "sniper")
            {
                unit = BoltNetwork.Instantiate(BoltPrefabs.Sniper, spawnPosition, Quaternion.identity);
            }

            //unit.GetComponent<Unit>().team = teamId;
        }
    }
}
