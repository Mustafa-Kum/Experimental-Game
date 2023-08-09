using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController cc;
    private PlayerInput playerInput;
    private Animator anim;

    public float moveSpeed = 5f;
    public float gravity = -9.8f;

    private float verticalVelocity;

    private Vector3 movementVelocity;

    public int _coin;

    public float rotationSpeed = 5f;

    //Enemy
    public bool isPlayer = true;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private Transform targetPlayer;

    // Health
    private Health health;

    // Damage Caster
    private DamageCaster damageCaster;

    // Player Slides

    private float attackStartTime;
    public float attackSlideDuration = 0.4f;
    public float attackSlideSpeed = 0.06f;

    private Vector3 impactOnPlayer;

    // State Machine
    public enum PlayerState
    {
        Normal, Attacking, Dead, BeingHit, Slide, Spawn
    }

    public PlayerState currentState;

    // Materials
    private MaterialPropertyBlock materialPropertyBlock;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    public GameObject itemToDrop;

    private float attackAnimationDuration;

    public float slideSpeed = 9f;

    public float spawnDuration = 2f;
    public float currentSpawnTime;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        damageCaster = GetComponentInChildren<DamageCaster>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        
        materialPropertyBlock = new MaterialPropertyBlock();
        skinnedMeshRenderer.GetPropertyBlock(materialPropertyBlock);

        if (!isPlayer)
        {
            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            targetPlayer = GameObject.FindWithTag("Player").transform;
            navMeshAgent.speed = moveSpeed;

            SwitchStateTo(PlayerState.Spawn);
        }
        else
        {
            playerInput = GetComponent<PlayerInput>();
        }
    }

    private void CalculatePlayerMovement()
    {
        if (playerInput.mouseButtonDown && cc.isGrounded)
        {
            SwitchStateTo(PlayerState.Attacking);

            return;
        }
        else if (playerInput.spaceKeyDown && cc.isGrounded)
        {
            SwitchStateTo(PlayerState.Slide);

            return;
        }
        
        movementVelocity.Set(playerInput.horizontalInput, 0f, playerInput.verticalInput);
        movementVelocity.Normalize();
        movementVelocity = Quaternion.Euler(0, -45f, 0) * movementVelocity;

        anim.SetFloat("Run", movementVelocity.magnitude);

        movementVelocity *= moveSpeed * Time.deltaTime;

        if (movementVelocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementVelocity);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        anim.SetBool("Airborne", !cc.isGrounded);
    }

    private void CalculateEnemyMovement()
    {
        if (Vector3.Distance(targetPlayer.position, transform.position) >= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.SetDestination(targetPlayer.position);

            anim.SetFloat("Run", 0.2f);
        }
        else
        {
            navMeshAgent.SetDestination(transform.position);

            anim.SetFloat("Run", 0f);

            SwitchStateTo(PlayerState.Attacking);
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case PlayerState.Normal:
                if (isPlayer)
                    CalculatePlayerMovement();
                else
                    CalculateEnemyMovement();
                
                break;
            
            case PlayerState.Attacking:

                if (isPlayer)
                {

                    if (Time.time < attackStartTime + attackSlideDuration)
                    {
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / attackSlideDuration;

                        movementVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
                    }

                    if (playerInput.mouseButtonDown && cc.isGrounded)
                    {
                        string currentClipName = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        attackAnimationDuration = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

                        if (currentClipName != "LittleAdventurerAndie_ATTACK_03" && attackAnimationDuration > 0.5f && attackAnimationDuration < 0.7f)
                        {
                            playerInput.mouseButtonDown = false;

                            SwitchStateTo(PlayerState.Attacking);

                            //CalculatePlayerMovement();
                        }
                    }
                }
                
                break;

            case PlayerState.Dead:
                return;

            case PlayerState.BeingHit:
                

                break;

            case PlayerState.Slide:
                movementVelocity = transform.forward * slideSpeed * Time.deltaTime;

                break;

            case PlayerState.Spawn:
                currentSpawnTime -= Time.deltaTime;

                if (currentSpawnTime <= 0)
                {
                    SwitchStateTo(PlayerState.Normal);
                }

                break;
        }

        if (impactOnPlayer.magnitude > 0.2f)
        {
            movementVelocity = impactOnPlayer * Time.deltaTime;
        }

        impactOnPlayer = Vector3.Lerp(impactOnPlayer, Vector3.zero, Time.deltaTime * 5);

        if (isPlayer)
        {
            if (cc.isGrounded == false)
                verticalVelocity = gravity;
            else
                verticalVelocity = gravity * 0.3f;

            movementVelocity += verticalVelocity * Vector3.up * Time.deltaTime;

            cc.Move(movementVelocity);

            movementVelocity = Vector3.zero;
        }
        else
        {
            if (currentState != PlayerState.Normal)
            {
                cc.Move(movementVelocity);

                movementVelocity = Vector3.zero;
            }
        }
    }

    public void SwitchStateTo(PlayerState newState)
    {
        if (isPlayer)
            playerInput.ClearCache();
        
        switch (currentState)
        {
            case PlayerState.Normal:
                break;
            
            case PlayerState.Attacking:

                if (damageCaster != null)
                    DisableDamageCaster();

                if (isPlayer)
                    GetComponent<PlayerVfxManager>().StopBlade();

                break;

            case PlayerState.Dead:
                return;

            case PlayerState.BeingHit:
                break;

            case PlayerState.Slide:
                break;

            case PlayerState.Spawn:
                break;
        }

        switch (newState)
        {
            case PlayerState.Normal:
                break;
            
            case PlayerState.Attacking:
                if (!isPlayer)
                {
                    Quaternion newRotation = Quaternion.LookRotation(targetPlayer.position - transform.position);

                    transform.rotation = newRotation;
                }
                
                anim.SetTrigger("Attack");

                if (isPlayer)
                {
                    attackStartTime = Time.time;

                    RotateToCursor();
                }

                break;

            case PlayerState.Dead:
                cc.enabled = false;
                anim.SetTrigger("Dead");

                StartCoroutine(MaterialDissolve());

                if (!isPlayer)
                {
                    SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();

                    mesh.gameObject.layer = 0;

                    Destroy(gameObject, 5f);
                }

                break;

            case PlayerState.BeingHit:
                anim.SetTrigger("BeingHit");

                break;

            case PlayerState.Slide:
                anim.SetTrigger("Slide");

                break;

            case PlayerState.Spawn:
                currentSpawnTime = spawnDuration;
                StartCoroutine(MaterialAppear());

                break;
        }

        currentState = newState;
    }

    public void SlideAnimationEnds()
    {
        SwitchStateTo(PlayerState.Normal);
    }

    public void AttackAnimationEnds()
    {
        SwitchStateTo(PlayerState.Normal);
    }

    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(PlayerState.Normal);
    }

    public void ApplyDamage(int damage, Vector3 attackerPos = new Vector3())
    {
        if (health != null)
        {
            health.ApplyDamage(damage);
        }

        if (!isPlayer)
        {
            GetComponent<EnemyVfxManager>().PlayBeingHitVFX(attackerPos);
        }

        StartCoroutine(MaterialBlink());

        if (isPlayer)
        {
            SwitchStateTo(PlayerState.BeingHit);

            AddImpact(attackerPos, 10f);
        }
        else
        {
            AddImpact(attackerPos, 2.5f);
        }
    }

    private void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;

        impactOnPlayer = impactDir * force;
    }

    public void EnableDamageCaster()
    {
        damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster()
    {
        damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink()
    {
        materialPropertyBlock.SetFloat("_blink", 0.4f);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);

        materialPropertyBlock.SetFloat("_blink", 0f);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2f);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHeight_start = 20f;
        float dissolveHeight_target = -10f;
        float dissolveHight;

        materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHight = Mathf.Lerp(dissolveHeight_start, dissolveHeight_target, currentDissolveTime / dissolveTimeDuration);

            materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight);
            skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);

            yield return null;
        }

        DropItem();
    }

    public void DropItem()
    {
        if (itemToDrop != null)
        {
            Instantiate(itemToDrop,transform.position, Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item)
    {
        switch(item.type)
        {
            case PickUp.PickUpType.Heal:
                AddHealth(item.value);
                
                break;

            case PickUp.PickUpType.Coin:
                AddCoin(item.value);

                break;

        }
    }

    private void AddHealth(int _health)
    {
        health.AddHealth(_health);
        GetComponent<PlayerVfxManager>().PlayHealVFX();
    }

    private void AddCoin(int coin)
    {
        _coin += coin;
    }

    public void RotateToTarget()
    {
        if (currentState != PlayerState.Dead)
        {
            transform.LookAt(targetPlayer, Vector3.up);
        }
    }

    IEnumerator MaterialAppear()
    {
        float dissolveTimeDuration = spawnDuration;
        float currentDissolveTime = 0;
        float dissolveHeight_Start = -10f;
        float dissolveHeight_Target = 20f;
        float dissolveHeight;

        materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_Start, dissolveHeight_Target, currentDissolveTime / dissolveTimeDuration);

            materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);

            yield return null;
        }

        materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        skinnedMeshRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitResult;

        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cursorPos, 1);
        }
    }

    private void RotateToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitResult;

        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;

            transform.rotation = Quaternion.LookRotation(cursorPos - transform.position, Vector3.up);
        }
    }
}
