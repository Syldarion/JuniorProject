﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class FleetScript : NetworkedMonoBehavior
{
    public ShipScript Flagship;
    public List<ShipScript> Ships;
    
    public int FleetSpeed;

    public HexTile CurrentPosition;

    public GameObject ShipPrefab;

    public static int NewShipID = 0;

	void Start()
    {
        Ships = new List<ShipScript>();
        FleetSpeed = 5;
	}

	void Update()
    {

	}

    public void SetFlagship(ShipScript ship)
    {
        if (ship != Flagship && Ships.Contains(ship))
            Flagship = ship;
    }

    public void AddShip(ShipScript ship)
    {
        if (!Ships.Contains(ship))
            Ships.Add(ship);
        UpdateFleetSpeed();

        RPC("AddShipOthers", NetworkReceivers.Others, ship.name);
    }

    [BRPC]
    void AddShipOthers(string ship_name)
    {
        ShipScript ship = GameObject.Find(ship_name).GetComponent<ShipScript>();
        if (!Ships.Contains(ship))
            Ships.Add(ship);
        UpdateFleetSpeed();
    }

    public void RemoveShip(ShipScript ship)
    {
        if (Ships.Contains(ship))
            Ships.Remove(ship);
        UpdateFleetSpeed();
        if (Ships.Count <= 0)
            Destroy(this.gameObject);

        RPC("RemoveShipOthers", NetworkReceivers.Others, ship.name);
    }

    [BRPC]
    void RemoveShipOthers(string ship_name)
    {
        ShipScript ship = transform.FindChild(ship_name).GetComponent<ShipScript>();
        if (Ships.Contains(ship))
            Ships.Remove(ship);
        UpdateFleetSpeed();
        if (Ships.Count <= 0)
            Destroy(this.gameObject);
    }

    //Fleet speed will always be equal to the slowest ship in the fleet
    public void UpdateFleetSpeed()
    {
        FleetSpeed = 5;                             //Fastest fleet speed possible
        foreach (ShipScript s in Ships)
        {
            if (s.Speed < FleetSpeed)
                FleetSpeed = s.Speed;
        }
    }

    [BRPC]
    public void SpawnFleet(string fleet_name, string initial_tile_name)
    {
        this.name = fleet_name;

        HexTile initial_tile = GameObject.Find(initial_tile_name).GetComponent<HexTile>();
        transform.SetParent(initial_tile.transform, false);
        transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        CurrentPosition = initial_tile;

        if (IsOwner)
            Networking.Instantiate(ShipPrefab, NetworkReceivers.AllBuffered, callback: OnShipCreated);
    }

    void OnShipCreated(SimpleNetworkedMonoBehavior new_ship)
    {
        new_ship.transform.SetParent(transform);
        new_ship.transform.localPosition = Vector3.zero;
        new_ship.RPC("SpawnShip", string.Format("{0}Ship{1}", Networking.PrimarySocket.Me.Name, (++NewShipID).ToString()), this.name);
        AddShip(new_ship.GetComponent<ShipScript>());
    }

    public bool MoveFleet(HexTile new_tile)
    {
        if (HexGrid.MovementHex(CurrentPosition, FleetSpeed).Contains(new_tile))
        {
            transform.SetParent(new_tile.transform, false);
            transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
            CurrentPosition = new_tile;

            return true;
        }
        return false;
    }
}
