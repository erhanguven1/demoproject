using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum UnitType { Sniper, Melee }

public class Unit : EntityEventListener<IUnitState>, ISelectable, IPointerUpHandler
{
    public UnitType unitType;

    private int team = -1;

    public float speed = 5;

    [SerializeField]internal int health = 100;
    public Image healthBar;

    internal int damage;

    private bool isSelected;

    public float energy = 100;
    public Image energyBar;

    private int movementEnergyCost = 15;

    public float maxDistance;

    private LineRenderer lineRenderer;

    public bool isMobile;

    [SerializeField] private FixedJoystick movementJoystick, attackJoystick;


    public void Init()
    {
        isMobile = GameManager.instance.isMobile;

        movementJoystick = GameObject.Find("MovementJoystick").GetComponent<FixedJoystick>();
        attackJoystick = GameObject.Find("AttackJoystick").GetComponent<FixedJoystick>();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = .1f;
        lineRenderer.material = GameManager.instance.lineMaterial;

        healthBar.fillAmount = (1.0f * health) / 100;

        TurnManager.onNewTurnStarted += OnNewTurnStarted;
        state.AddCallback("teamId", TeamChanged);

        StartCoroutine(setTeam());
        IEnumerator setTeam()
        {
            yield return new WaitForSeconds(1.5f);
            if (entity.IsOwner)
            {
                team = UnitSelectionUIManager.instance.teamId;
                state.teamId = team.ToString();

                transform.GetChild(0).GetComponent<MeshRenderer>().material.color = team == 0 ? Color.blue : Color.red;
            }
        }
    }

    private void OnDestroy()
    {
        TurnManager.onNewTurnStarted -= OnNewTurnStarted;
    }

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        if (entity.IsOwner)
        {
            state.energy = energy;
        }

        state.AddCallback("energy", EnergyChanged);
    }

    void EnergyChanged()
    {
        energy = state.energy;
        energyBar.fillAmount = energy / 100;
    }

    private void OnNewTurnStarted()
    {
        energy = 100;
        health += 5;
        health = Mathf.Min(100, health);
        healthBar.fillAmount = (1.0f * health) / 100;
        energyBar.fillAmount = energy / 100;
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

    public void TeamChanged()
    {
        team = int.Parse(state.teamId);
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = team == 0 ? Color.blue : Color.red;
    }

    void FixedUpdate()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);

        if (!isSelected || energy <= 0 || !entity.IsOwner || !TurnManager.instance.isMyTurn) return;

        int dir = team == 0 ? 1 : -1;

        if (!isMobile)
        {
            transform.position += dir * speed * Time.fixedDeltaTime * (Input.GetAxis("Vertical") * Vector3.forward + Input.GetAxis("Horizontal") * Vector3.right);
        }
        else
        {
            transform.position += dir * speed * Time.fixedDeltaTime * (movementJoystick.Vertical * Vector3.forward + movementJoystick.Horizontal * Vector3.right);
        }

        float verticalMovement = !isMobile ? Input.GetAxis("Vertical") : movementJoystick.Vertical;
        float horizontalMovement = !isMobile ? Input.GetAxis("Horizontal") : movementJoystick.Horizontal;

        if (verticalMovement != 0 || horizontalMovement != 0)
        {
            energy -= Time.deltaTime * movementEnergyCost * (Math.Abs(verticalMovement) + Math.Abs(horizontalMovement));
            state.energy = energy;
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (!isSelected || !entity.IsOwner || energy < 50 || !TurnManager.instance.isMyTurn) return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Ray shootRay;

            if (isMobile)
            {
                shootRay = new Ray(transform.position, new Vector3(attackJoystick.Horizontal, 0, attackJoystick.Vertical));
            }
            else
            {
                shootRay = new Ray(transform.position, (hit.point - transform.position).normalized);
            }

            if (Physics.Raycast(shootRay, out RaycastHit hit2, maxDistance))
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, hit2.point);

                if (!isMobile)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        if (hit2.collider.GetComponentInParent<Unit>() != null)
                        {
                            var targetUnit = hit2.collider.GetComponentInParent<Unit>();

                            if (CanAttack(ref targetUnit))
                            {
                                Attack(ref targetUnit);

                            }
                        }
                    }
                }
                else
                {
                    //Mobile shooting logic
                    if(attackJoystick.pointerUp)
                    {
                        if (attackJoystick.totalValue > .2f)
                        {
                            print("boom");
                            if (hit2.collider.GetComponentInParent<Unit>() != null)
                            {
                                var targetUnit = hit2.collider.GetComponentInParent<Unit>();

                                if (CanAttack(ref targetUnit))
                                {
                                    Attack(ref targetUnit);

                                }
                            }
                        }
                    }
                }
            }
            else if (isMobile)
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, transform.position + maxDistance * (new Vector3(attackJoystick.Horizontal, 0, attackJoystick.Vertical).normalized));

                if (attackJoystick.pointerUp)
                {
                    if (attackJoystick.totalValue > .2f)
                    {
                        print("boom but weaker");
                        energy = 0;
                        state.energy = energy;
                    }
                }
            }
            else
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, transform.position);
            }
        }
        else
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
        }
    }

    public override void OnEvent(Shoot evnt)
    {
        health -= evnt.Damage;
        healthBar.fillAmount = (1.0f * health) / 100;
        if (health <= 0 && entity.IsOwner)
        {
            BoltNetwork.Destroy(gameObject);
        }
    }

    void Attack(ref Unit targetUnit)
    {
        energy = 0;
        state.energy = energy;
        var damageEvnt = Shoot.Create(targetUnit.entity);
        damageEvnt.Damage = damage;
        damageEvnt.Send();
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
                if(Vector3.Distance(transform.position, targetUnit.transform.position) < maxDistance)
                {
                    return true;
                }
                return false;
            case UnitType.Sniper:
                if (Vector3.Distance(transform.position, targetUnit.transform.position) < maxDistance)
                {
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}