using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { Sniper, Melee }

public class Unit : MonoBehaviour, ISelectable
{
    public UnitType unitType;

    public int team;

    public float speed = 5;

    internal int health = 100;

    internal int damage;

    private bool isSelected;

    public float energy = 100;
    private int movementEnergyCost = 15;

    public void Init()
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = team == 0 ? Color.blue : Color.red;

        TurnManager.onNewTurnStarted += OnNewTurnStarted;
    }

    private void OnNewTurnStarted()
    {
        energy = 100;
        health += 5;
        health = Mathf.Min(100, health);
    }

    public void Select()
    {
        print("Selected" + name);
        isSelected = true;
        if (transform.childCount == 2)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void Deselect()
    {
        print("Deselected" + name);
        isSelected = false;
        if (transform.childCount == 2)
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isSelected) return;

        transform.position += speed * Time.fixedDeltaTime * (Input.GetAxis("Vertical") * Vector3.forward + Input.GetAxis("Horizontal") * Vector3.right);

        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            energy -= Time.deltaTime * movementEnergyCost;
        }
    }

    private void Update()
    {
        if (!isSelected) return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && Input.GetMouseButtonDown(1))
        {
            if (hit.collider.GetComponentInParent<Unit>() != null)
            {
                var targetUnit = hit.collider.GetComponentInParent<Unit>();

                if (CanAttack(ref targetUnit))
                {
                    Attack(ref targetUnit);
                }
            }
        }
    }

    void Attack(ref Unit targetUnit)
    {
        energy = 0;
        targetUnit.health -= damage;
        print(targetUnit.health);
    }

    bool CanAttack(ref Unit targetUnit)
    {
        //need some energy and friendly fire 0
        if (energy < 50 || targetUnit.team == team)
        {
            return false;
        }

        switch (unitType)
        {
            case UnitType.Melee:
                //Range : 2
                if(Vector3.Distance(transform.position, targetUnit.transform.position) < 2)
                {
                    return true;
                }
                return false;
            case UnitType.Sniper:
                //Infinite range for now
                return true;
            default:
                return false;
        }
    }
}
