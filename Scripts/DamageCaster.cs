using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    private Collider damageCasterCollider;

    public int damage = 30;

    public string targetTag;

    private List<Collider> damagedTargetList;

    private void Awake()
    {
        damageCasterCollider = GetComponent<Collider>();
        damageCasterCollider.enabled = false;

        damagedTargetList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == targetTag && !damagedTargetList.Contains(other))
        {
            Player targetPlayer = other.GetComponent<Player>();

            if (targetPlayer != null)
            {
                targetPlayer.ApplyDamage(damage, transform.parent.position);

                PlayerVfxManager playerVfxManager = transform.parent.GetComponent<PlayerVfxManager>();

                if (playerVfxManager != null)
                {
                    RaycastHit hit;

                    Vector3 originalPos = transform.position + (-damageCasterCollider.bounds.extents.z) * transform.forward;

                    bool isHit = Physics.BoxCast(originalPos, damageCasterCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation, damageCasterCollider.bounds.extents.z, 1 << 6);

                    if (isHit)
                        playerVfxManager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                }
            }

            damagedTargetList.Add(other);
        }
    }

    public void EnableDamageCaster()
    {
        damagedTargetList.Clear();
        damageCasterCollider.enabled = true;
    }

    public void DisableDamageCaster()
    {
        damagedTargetList.Clear();
        damageCasterCollider.enabled = false;
    }
}
