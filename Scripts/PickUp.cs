using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickUpType
    {
        Heal, Coin
    }

    public ParticleSystem collectedVFX;
    
    public PickUpType type;

    public int value = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().PickUpItem(this);

            if (collectedVFX != null)
                Instantiate(collectedVFX, transform.position, Quaternion.identity);
            
            Destroy(gameObject);
        }
    }
}
