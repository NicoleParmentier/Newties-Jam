﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IHittable
{
    private int shootShipBalanceChange = 4, shipSpaghettifyBalanceChange = 7;
    [SerializeField] GameObject spaghetti, explosion;
    [SerializeField] GameObject floatingTextPrefab;

    private void Start()
    {
        transform.GetChild(Random.Range(0, 2)).gameObject.SetActive(false);
    }

    public void Hit()
    {
        DestroyAndChangeBalance(shootShipBalanceChange);
        Instantiate(explosion, transform.position, Quaternion.identity);

        if (floatingTextPrefab != null)
        {
            Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    public void Spaghettify()
    {
        Instantiate(spaghetti, transform.position, Quaternion.identity);

        DestroyAndChangeBalance(shipSpaghettifyBalanceChange);
    }

    private void DestroyAndChangeBalance(int balanceChange)
    {
        MafiaMeter meter = FindObjectOfType<MafiaMeter>();
        if (meter != null)
        {
            meter.ChangeBalance(shootShipBalanceChange);
        }

        Destroy(this.gameObject);
    }
}
