﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Unit
{
    // Start is called before the first frame update
    void Start()
    {
        unitType = UnitType.Melee;

        damage = 20;
        speed = 8;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
