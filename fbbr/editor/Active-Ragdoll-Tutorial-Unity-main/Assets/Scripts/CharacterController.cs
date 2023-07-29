using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private ConfigurableJoint hipJoint;
    [SerializeField] private Rigidbody hip;

    [SerializeField] private Animator targetAnimator;

    private bool walk = false;

    public Transform target = null;
    [SerializeField] public Transform hipT;
    [SerializeField] private Transform gun;
    [SerializeField] private bool manual;
    float horizontal = 0;
    float vertical = 0;
    [SerializeField] private Vector3 direction = new(0, 0, 0);
    [SerializeField] private float angle = 0;



    public LayerMask whatIsGround, whatIsTarget;

    public float health;


    //Attacking
    public float timeBetweenAttacks;
    public float timeBetweenInv;
    bool alreadyAttacked;
    public bool alreadyInv;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public SphereCollider sightCollider;
    [SerializeField] private SphereCollider attackCollider;
    public bool targetInSightRange, targetInAttackRange;
    [SerializeField] float bulletDespawn = 4;
    [SerializeField] float bulletDamage = 10;




    [SerializeField] float distToCenter = 0;
    [SerializeField] Transform center;
    public Transform selfTarget;

    public List<CharacterController> targetedBy = new List<CharacterController>();

    
    public static event Action<CharacterController> onCharacterKilled;


    [SerializeField] Transform GUI;
    [SerializeField] public String _name;
    public int kills;
    public TMP_Text nametag;
    public TMP_Text killstag;
    public TMP_Text scoretag;
    public GameObject scoreSlab;
    public Transform scoreboard;
    [SerializeField] Slider healthslider;
    public GameObject spectatePoint;
    [SerializeField] GameObject killslab;

    private void Awake()
    {
        sightCollider.radius = sightRange;
        attackCollider.radius = attackRange;
        center = GameObject.Find("Center").transform;
        scoreboard = GameObject.Find("Scoreboard").transform.GetChild(0).transform.GetChild(0);
    }
    void Update()
    {
        GUI.LookAt(Camera.main.transform.position);
        GUI.transform.Rotate(0,180,0);
        //if manual controls, not AI
        if (manual)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

                this.hipJoint.targetRotation = Quaternion.Euler(0f, angle, 0f);

                this.hip.AddForce(direction * this.speed);

                this.walk = true;
            }
            else
            {
                this.walk = false;
            }
            this.targetAnimator.SetBool("Walk", this.walk);
        }
        else
        {
            if(target == null){
                distToCenter = Vector3.Distance(center.position, hipT.position);
                if(distToCenter >= 5){

                    direction = (new Vector3(0,0,0) - hipT.position).normalized;
                    angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
                    this.hipJoint.targetRotation = Quaternion.Euler(0f, angle, 0f);
                    this.hip.AddForce(direction * this.speed);


                    this.targetAnimator.SetBool("Walk", true);
                }
                else{
                    this.targetAnimator.SetBool("Walk", false);
                }
            }
            else{
                if (targetInSightRange && !targetInAttackRange) ChasePlayer();
                if (targetInAttackRange && targetInSightRange) AttackPlayer();
            }
        }
    }

    void LateUpdate()
    {
        spectatePoint.transform.position = new Vector3(hipT.position.x, 1, hipT.position.z);
        
    }

    private void ChasePlayer()
    {
        direction = (target.position - hipT.position).normalized;
        angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        this.hipJoint.targetRotation = Quaternion.Euler(0f, angle, 0f);
        this.hip.AddForce(direction * this.speed);


        this.targetAnimator.SetBool("Walk", true);
    }

    private void AttackPlayer()
    {
        direction = (target.position - hipT.position).normalized;
        angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        this.hipJoint.targetRotation = Quaternion.Euler(0f, angle, 0f);
        this.targetAnimator.SetBool("Walk", false);

        if (!alreadyAttacked)
        {
            GameObject bullet  = Instantiate(projectile, gun.position, Quaternion.identity);
            bullet.GetComponent<bullet>().owner = GetComponent<CharacterController>();
            bullet.GetComponent<bullet>().range = bulletDespawn;
            bullet.GetComponent<bullet>().damage = bulletDamage;
            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
            Transform bulletT = bullet.GetComponent<Transform>();

            bulletRB.AddForce(gun.transform.forward * 32f, ForceMode.Impulse);
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), gun.GetComponent<Collider>());


            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public void ResetInvulnerability()
    {
        alreadyInv = false;
    }

    public void TakeDamage(float damage, CharacterController killer)
    {
        if(!alreadyInv){
//            Debug.Log("HIT");
            health -= damage;
            healthslider.value = health;
            if (health <= 0) {
                killer.alreadyInv = true;
                killer.Invoke(nameof(ResetInvulnerability), 0.1f);
                // killer.health = Math.Min(killer.health + 70, 100 );
                // killer.health = killer.health + (Math.Max(1, kills) * 20);
                killer.kills++;
                killer.health = 100 + (killer.kills * 30);
                killer.healthslider.maxValue = killer.health;
                killer.healthslider.value = killer.health;

                alreadyInv= true;
                killer.killstag.text = killer.kills.ToString();
                killer.scoretag.text = string.Format($"{killer._name}: {killer.kills}");
                killer.scoreSlab.GetComponent<score>().kills = killer.kills;
                // Debug.Log(string.Format($"{killer._name} killed {_name}"));
                // Debug.Log(string.Format($"{killer._name} kills = {killer.kills}"));
                // Debug.Log("");
                GameObject _killslab = Instantiate(killslab, GameObject.Find("Kill Feed").transform.GetChild(0).transform.GetChild(0).transform, false);
                _killslab.transform.GetChild(0).GetComponent<TMP_Text>().text = killer._name;
                _killslab.transform.GetChild(1).GetComponent<TMP_Text>().text = _name;

                int index = gameManager.totalPlayers-1;
                while(scoreboard.GetChild(index).GetComponent<score>().kills <= kills && scoreboard.GetChild(index).GetComponent<score>().alive == false){
                    // Debug.Log($"INDEX = {index}");
                    // Debug.Log(scoreboard.GetChild(index).GetComponent<score>().alive);
                    // Debug.Log(string.Format($"{scoreboard.GetChild(index).GetComponent<score>().kills} <= {kills} and {scoreboard.GetChild(index).GetComponent<score>()} IS DEAD"));
                    index--;
                        // Debug.Log("FOUND SELF");
                }
                scoreSlab.GetComponent<score>().alive = false;
                // Debug.Log($"FINAL INDEX = {index}");
                // Debug.Log("");
                scoreSlab.transform.SetSiblingIndex(index);
                scoreSlab.GetComponent<Button>().interactable = false;
                scoreSlab.GetComponent<Image>().color = Color.red;

                if(killer.scoreSlab.GetComponent<score>().alive){
                    index = 0;
                    while(scoreboard.GetChild(index).GetComponent<score>().kills > killer.kills && scoreboard.GetChild(index).GetComponent<score>().alive && index < gameManager.totalPlayers){
                        index++;
                    }
                    killer.scoreSlab.transform.SetSiblingIndex(index);
                }


                Invoke(nameof(DestroyEnemy), 0.5f);
            }
            else{
                alreadyInv= true;
                Invoke(nameof(ResetInvulnerability), timeBetweenInv);
            }
        }
    }


    //if died, get the list of those who are targeting this character, and make them find a new target. maybe a  better way of doing this would be before an npc attacks, it checks if the target is alive, if not, find a new target
    private void DestroyEnemy()
    { 
        CharacterController temp = GetComponent<CharacterController>();
        foreach(CharacterController CC in targetedBy){
            if(CC != null){
                CC.target = null;
                CC.targetInSightRange = false;
                CC.targetInAttackRange = false;
                Collider[] hitColliders = Physics.OverlapSphere(CC.hipT.position, CC.sightRange * 33.5f);
                    if(hitColliders.Length != 0){
                        foreach(Collider col in hitColliders){
                            // if(col.gameObject.name == "target"){
                            //     if(col.transform != CC.selfTarget){
                            //         Debug.Log("OTHER TARGET");
                            //     }
                            //     else{
                            //         Debug.Log("SELF TARGET");
                            //     }
                            // }
                            if(col.gameObject.name == "target" && col.transform != CC.selfTarget && col.transform.parent.transform.parent.transform.parent.GetComponent<CharacterController>() != temp){
                                CC.target = col.transform;
                                CC.target.transform.parent.transform.parent.transform.parent.GetComponent<CharacterController>().targetedBy.Add(CC);
                                CC.targetInSightRange = true;
                                if(Vector3.Distance(CC.hipT.position, CC.target.position) <= CC.attackRange * 33.5f){
                                    CC.targetInAttackRange = true;
                                }
                                break;
                            }
                        }
                    }
            }

                
            if(CC == target.transform.parent.transform.parent.transform.parent.GetComponent<CharacterController>() || CC == null){
                //Debug.Log("the character wathching me is also my target");
                CC.targetedBy.Remove(GetComponent<CharacterController>());
            }
        }
        Destroy(spectatePoint);
        Destroy(gameObject);
        onCharacterKilled?.Invoke(this);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(hipT.position, sightRange);
    }
    }