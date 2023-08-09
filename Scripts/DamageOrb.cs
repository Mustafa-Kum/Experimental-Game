using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    public ParticleSystem hitVFX;

    private Rigidbody rb;
    
    public float speed = 2f;

    public int damage = 10;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player != null && player.isPlayer) 
        {
            player.ApplyDamage(damage, transform.position);
        }

        Instantiate(hitVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
