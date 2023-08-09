using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_02_Shoot : MonoBehaviour
{
    public Transform shootingPoint;

    public GameObject damageOrb;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void ShootTheDamageOrb()
    {
        Instantiate(damageOrb, shootingPoint.position, Quaternion.LookRotation(shootingPoint.forward));
    }

    private void Update()
    {
        player.RotateToTarget();
    }
}
