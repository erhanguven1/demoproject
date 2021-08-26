using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Unit
{
    // Start is called before the first frame update
    void Start()
    {
        unitType = UnitType.Sniper;

        damage = 35;
        speed = 5;

        Init();
    }
}
