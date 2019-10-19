using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class Body : MonoBehaviour
{
    // Animator layers
    protected int torsoSwSh = 1;
    protected int torsoMage = 2;
    protected int handLeft = 3;
    protected int handRight = 4;


    [SerializeField]
    List<MyItemInventory> _itemsInventory = new List<MyItemInventory>();
    public List<MyItemInventory> ItemsInventory
    {
        private set { }
        get { return _itemsInventory; }
    }
    public void AddItemToInv(MyItemInventory ii)
    {
        _itemsInventory.Add(ii);
        ProjectIOSingletone.Get().MyBodyInventoryAdd(this);
    }
    //public Transform _testWeapon;
    public MyWeapon _testSword;

    //public Transform _testShield;
    public MyShield _testShield;
    //protected MeshCollider _testWeaponMesh;

    public float _health = 10f;

    //void OnEnable()
    //{
    //    StaticEvents.eventHit.AddListener(SetHit);
    //    StaticEvents.eventKill.AddListener(SetKill);
    //}

    //void OnDisable()
    //{
    //    StaticEvents.eventHit.RemoveListener(SetHit);
    //    StaticEvents.eventKill.RemoveListener(SetKill);
    //}

    //public void SetDamage(float damage)
    //{
    //    _health -= damage;
    //    Debug.Log(name + " health = " + _health);
    //    if (_health <= 0)
    //    {
    //        Debug.Log(name + " dead");
    //        //Destroy(gameObject);

    //        ProjectIOSingletone.Get().DestroyObject(this);
    //    }
    //}
    public void GeneralBehaviour()
    {
        //RotateOnPoint(countDown0);
        //WakAround(countDown1);
        //ReturnToHomePoint();
        //StayIdle(countDown2);
        //FollowTarget(countDown3);
    }
    public void SetHit(object[] damageInfo)
    {
        float damage = (float)damageInfo[0];
        if (damage > 0)
        {
            //if(NO TargeT)
               //look here //RotateOnPoint in 360 degree in random direction


        }

        _health -= damage;

        StaticEvents.eventHit.Invoke(damageInfo);

        if ((Body)damageInfo[1])
            Debug.Log(name + " hit by  " + ((Body)damageInfo[1]).name + " health = " + _health);
        else
            Debug.Log(name + " health = " + _health);

        

        if (_health <= 0)
        {
            SetKill(damageInfo);
        }
    }
    public void SetKill(object[] damageInfo)
    {

        if ((Body)damageInfo[1])
            Debug.Log(name + " Killed by " + ((Body)damageInfo[1]).name);
        //Destroy(gameObject);

        StaticEvents.eventKill.Invoke(damageInfo);
        ProjectIOSingletone.Get().DestroyObject(this);

    }

    public void BlockedAttack()
    {
        Debug.Log(name + "  blocks ATK");
    }

    public Vector3 rWRest = new Vector2();
    public Vector3 rWFight = new Vector2();
    public Vector3 shRest = new Vector2();
    public Vector3 shFight = new Vector2();

    [HideInInspector]
    public Transform
                    //All Body Parts
                    //_bodyHead, _bodyJaw,
                    _spine
                    //, _bodyAss
                    //,_bodyHipL, _bodyHipR
                    //, _bodyShinL, _bodyShinR
                    //,_bodyFootL, _bodyFootR

                    //,_bodyBladeL, _bodyBladeR
                    //,_bodyArmL, _bodyArmR
                    , _lHand, _rHand
                    //,_bodyPalmL, _bodyPalmR
                    ;

    [HideInInspector]
    public Transform _transform;

    protected CharacterController _ccBody;
    protected UnityEngine.AI.NavMeshAgent _nma;
    protected Animator _animator;
    protected Rigidbody _rigidbody;

    public float _bodySpeed = 4;
    protected float bodySpeedCurrent = 0;
    public float bodyAngularSpeed = 120;
    public float bodyAcceleration = 4;
    public float bodyStoppingDistance = 1;

    public Body botBodyTarget;//= new Vector2();

    public ArmState armState = ArmState.unarmed; // what type of weapon or unarmed
    public ArmState lastArmedState = ArmState.armSwSh; // last armed state befor unurm (example - automatic arm if attacked)
    public ArmState nextArmedState = ArmState.armSwSh; // it is for bot changing state
    public enum ArmState
    {
        unarmed,
        armMage,
        armSwSh
    }
    public string _tag = "NoTagg";
    public List<string> _tagFriendList = new List<string>();

    protected float followDistance = 7;
    protected float spotDistance = 5;
    protected float spotBackDistance = 1;
    protected LayerMask _targetLayer = 1 << 9;    // layer Body
    protected float angle;
    public void Start()
    {
        _transform = transform;
        AddComponentsToBody(gameObject);

        _rHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
        _lHand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
        _spine = _animator.GetBoneTransform(HumanBodyBones.Spine);

        WeaponOnStart();
        //******************************************
        Start_();
    }
    void WeaponOnStart()
    {
        if (_testSword) _testSword._Body = this;
        if (_testShield) _testShield._Body = this;
        SwShRest();
        //_testWeapon.WeaponMeshSwitch(false);
        //_testWeapon.WeaponTrailSwitch(false);
        _testSword.WeaponAttacking(false);
        _testShield.WeaponAttacking(false);
        switch (armState)
        {
            case ArmState.armMage:

                break;
            case ArmState.armSwSh:
                SwShFight();
                break;
            case ArmState.unarmed:

                break;
        }
    }
    public void SwShRest()
    {
        if (_testSword)
        {
            //_testWeapon.parent = _spine;
            //_testWeapon.localPosition = new Vector3(-0.55f, -0.058f, -0.1f);
            //_testWeapon.localEulerAngles = new Vector3(0, -107f, 0f);
            _testSword._Transform.parent = _spine;
            _testSword._Transform.localPosition = new Vector3(-0.55f, -0.058f, -0.1f);
            _testSword._Transform.localEulerAngles = new Vector3(0, -107f, 0f);
        }
        if (_testShield)
        {
            //_testShield.parent = _spine;
            //_testShield.localPosition = new Vector3(-0.274f, -0.08f, 0.066f);
            //_testShield.localEulerAngles = new Vector3(-5f, 0f, 175f);
            _testShield._Transform.parent = _spine;
            _testShield._Transform.localPosition = new Vector3(-0.274f, -0.08f, 0.066f);
            _testShield._Transform.localEulerAngles = new Vector3(-5f, 0f, 175f);
        }
    }
    public void SwShFight()
    {
        if (_testSword)
        {
            //_testWeapon.parent = _rHand;
            //_testWeapon.localPosition = new Vector3(-0.06f, -0.03f, 0.03f);
            //_testWeapon.localEulerAngles = new Vector3(0, 18f, 0f);
            _testSword._Transform.parent = _rHand;
            _testSword._Transform.localPosition = new Vector3(-0.06f, -0.03f, 0.03f);
            _testSword._Transform.localEulerAngles = new Vector3(0, 18f, 0f);
        }
        if (_testShield)
        {
            //_testShield.parent = _lHand;
            //_testShield.localPosition = new Vector2();
            //_testShield.localEulerAngles = new Vector2();
            _testShield._Transform.parent = _lHand;
            _testShield._Transform.localPosition = new Vector2();
            _testShield._Transform.localEulerAngles = new Vector2(); ;
        }
    }

    public virtual void Start_() { }


    void AddComponentsToBody(GameObject _body)
    {
        _ccBody = _body.GetComponent<CharacterController>();
        if (!_ccBody)
        {
            _ccBody = _body.AddComponent<CharacterController>();
            Debug.LogWarning("on GameObj " + _body.name + " CharacterController is. Missed. Added");
        }



        // _rigidbody = _body.GetComponent<Rigidbody>();




        if (_body.name == "Player") //{ }
            _ccBody.enabled = true;
        else
        {
            // Transform go = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            // Transform go =  new GameObject().transform;
            //go.name = "navMesh";
            //go.SetParent(_body.transform);
            //go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            //go.localPosition = Vector3.zero;
            //go.gameObject.GetComponent<BoxCollider>().enabled = false;
            //_nma = go.GetComponent<NavMeshAgent>();
            //if (!_nma)
            //{
            //    _nma = go.gameObject.AddComponent<NavMeshAgent>();
            //    Debug.LogWarning("on GameObj " + _body.name + " NavMeshAggent is. Missed. Added");
            //}
            _nma = _body.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (!_nma)
            {
                _nma = _body.AddComponent<UnityEngine.AI.NavMeshAgent>();
                Debug.LogWarning("on GameObj " + _body.name + " NavMeshAggent is. Missed. Added");
            }



            BodyNavParams(_nma);
        }

        BodyMove bodyMove = _body.GetComponent<BodyMove>();
        if (!bodyMove)
            bodyMove = _body.AddComponent<BodyMove>();

        _animator = _body.GetComponent<Animator>();
    }
    void BodyNavParams(UnityEngine.AI.NavMeshAgent nma)
    {
        nma.speed = _bodySpeed;
        nma.angularSpeed = bodyAngularSpeed;
        nma.updateRotation = true;
        nma.acceleration = bodyAcceleration;
        nma.stoppingDistance = bodyStoppingDistance;
        nma.autoBraking = true;

        nma.radius = 0.32f;
    }

    public enum ActionUnit
    {
        toArmStateUnurmed,
        toArmStateSwSh,
        toArmStateMage,
        attacking,
        jumping
    }
    protected bool nextFromStack = true;
    public List<ActionUnit> auList = new List<ActionUnit>();
    protected float stackAutocleanTimer = 0;
    protected float stackAutoclean = 1f;

    public bool actAnimating = false;
    public bool actAnimatingEnd = false;
    public bool atkAnimating = false;
    public bool atkAnimatingStart = false;
    public bool atkAnimatingMiddle = false;
    public bool atkAnimatingEnd = false;



    bool actHandLeftAnimatingEnd = false;//test
    bool actHandRightAnimatingEnd = false;//test

    //public bool ActAnimatingEnd
    //{
    //    set { }
    //}


    public void ActionStack(ActionUnit au)
    {
        if (auList.Count == 0)
        {
            auList.Add(au);
            stackAutocleanTimer = stackAutoclean;
        }
        else if (auList.Count == 1)
        {

            if (auList[0] != ActionUnit.toArmStateMage || auList[0] != ActionUnit.toArmStateSwSh || auList[0] != ActionUnit.toArmStateUnurmed)
            {
                auList.RemoveAt(0);
                auList.Add(au);
                stackAutocleanTimer = stackAutoclean;
            }
            else
            {

            }
        }
    }

    public virtual void UpdatePlayer(Vector3 moveDir, Vector3 lookDir) { }
    public virtual void UpdateBot(float deltaTime) { }
    public virtual void Attacking() { }
    public virtual void Jumping() { }
    public virtual void Arm(ArmState astate) { }


}
