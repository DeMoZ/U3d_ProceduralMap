using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class BodyMove : Body
{

    public bool canjump = false;
    float nextjumpTime = 0; // jump countdown
    float jumpinterval = .5f; // time between jumps
    float jumpParameter = 0; // to deferentiate walk and run jump
    public float jumpHeight = 2f;
    float gravity = 9.8f;

    float _moveForward;
    float _moveForwardSmoothed;

    float _moveRight;
    float _moveRightSmoothed;

    float currAcceleration;

    Vector3 _moveDir = new Vector2();

    void Update()
    {
        CountTimerToZero(ref stackAutocleanTimer);
    }
    void CountTimerToZero(ref float timer)
    {
        timer -= Time.deltaTime;
        timer = Mathf.Clamp(timer, 0, float.MaxValue);
    }

    void FixedUpdate()
    {
        Falling();
        ActionStackRelise();
    }
    void ActionStackRelise()
    {
        if (nextFromStack == true && !actAnimating && !atkAnimating)
        {
            if (auList.Count > 0)
            {
                nextFromStack = false;
                switch (auList[0])
                {
                    case ActionUnit.toArmStateUnurmed:
                        Arm(ArmState.unarmed);
                        break;
                    case ActionUnit.toArmStateMage:
                        Arm(ArmState.armMage);
                        break;
                    case ActionUnit.toArmStateSwSh:
                        Arm(ArmState.armSwSh);
                        break;
                    case ActionUnit.attacking:
                        Attacking();
                        break;
                    case ActionUnit.jumping:
                        Jumping();
                        break;
                }
                auList.RemoveAt(0);
            }
        }
        if (stackAutocleanTimer == 0) auList.Clear();
    }

    public override void UpdatePlayer(Vector3 moveDir, Vector3 lookDir)//, Vector3 camPos)
    {
        if (_nma)
            _nma.enabled = false;

        //if (atkAnimating)
        //{
        //    //!!!!!!!          !!!!!!!!!! Plase walking while attack here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        //    _moveDir.x = Mathf.Lerp(_moveDir.x, 0, Time.deltaTime * bodyAcceleration);
        //    _moveDir.z = Mathf.Lerp(_moveDir.z, 0, Time.deltaTime * bodyAcceleration);
        //}
        //else
        //{
        //    _moveDir.x = Mathf.Lerp(_moveDir.x, moveDir.x, Time.deltaTime * bodyAcceleration);
        //    _moveDir.z = Mathf.Lerp(_moveDir.z, moveDir.z, Time.deltaTime * bodyAcceleration);
        //}
        //_ccBody.Move(_moveDir * _bodySpeed * Time.deltaTime);


        if (atkAnimating)
            bodySpeedCurrent = Mathf.Lerp(bodySpeedCurrent, _bodySpeed / 2, Time.deltaTime * bodyAcceleration);
        else
            bodySpeedCurrent = Mathf.Lerp(bodySpeedCurrent, _bodySpeed, Time.deltaTime * bodyAcceleration);

        _moveDir.x = Mathf.Lerp(_moveDir.x, moveDir.x, Time.deltaTime * bodyAcceleration);
        _moveDir.z = Mathf.Lerp(_moveDir.z, moveDir.z, Time.deltaTime * bodyAcceleration);

        _ccBody.Move(_moveDir * bodySpeedCurrent * Time.deltaTime);

        Vector3 transformForward = _transform.forward;
        transformForward.y = 0;
        transformForward = transformForward.normalized;

        if (!atkAnimating && (armState != ArmState.unarmed || moveDir.x != 0 || moveDir.z != 0 || !nextFromStack))
        {
            angle = Vector3.Angle(transformForward, lookDir);
            Vector3 cross = Vector3.Cross(transformForward, lookDir);
            if (cross.y < 0) angle = -angle;
            Quaternion toRotation = Quaternion.Euler(Vector3.up * angle + _transform.eulerAngles);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, toRotation, bodyAcceleration * Time.deltaTime);

        }

        AnimateBody(_ccBody.velocity, Time.deltaTime);

    }


    void ArmUnarmBTN()
    {
        //listen to buttons
        if (Input.GetKeyDown(KeyCode.E))//||Input.GetButtonDown("Fire3"))
            Arm(ArmState.armSwSh);
        //ArmedSwSh();

        if (Input.GetKeyDown(KeyCode.Q))
            Arm(ArmState.armMage);
        //ArmedMage();
    }

    public override void Arm(ArmState arm)
    {
        StartCoroutine(ArmCoroutine(arm));
        // ArmNonCoroutine(arm);
    }



    IEnumerator ArmCoroutine(ArmState arm)
    {

        while (actAnimating) { yield return null; }
        // need to compare what state we have and what we want

        if (arm == ArmState.unarmed || armState == arm)
        {
            StartCoroutine(DisarmCoroutine());
            nextFromStack = true;
            yield break;
        }
        else//if armed somethin but i want to arm different
        {
            yield return StartCoroutine(DisarmCoroutine());
            // arm!

            actAnimating = true;

            lastArmedState = armState;

            _animator.SetTrigger("arm");
            _animator.SetBool(armState.ToString(), false);
            armState = arm;
            _animator.SetBool(arm.ToString(), true);
            int armLayer = 0;
            switch (arm)
            {
                case ArmState.armSwSh:
                    armLayer = torsoSwSh;
                    break;

                case ArmState.armMage:
                    armLayer = torsoMage;
                    break;
            }
            _animator.SetLayerWeight(armLayer, 1);

            while (!actAnimatingEnd) { yield return null; }

            switch (arm)
            {
                case ArmState.armSwSh:
                    SwShFight();
                    break;

                case ArmState.armMage:
                    break;
            }
            _animator.SetTrigger("newState");

            while (actAnimatingEnd) { yield return null; }

            _animator.SetLayerWeight(armLayer, 0);

            actAnimating = false;

        }
        nextFromStack = true;
    }
    public enum WeaponHang
    {
        noWeapon,
        leftWaist,
        rightWaist,
        leftBack,
        rightBack
    }
    public WeaponHang rightHandWeaponHang;
    public WeaponHang leftHandWeaponHang;
    public
    IEnumerator TestDisarmCoroutine()
    {
        if (armState == ArmState.unarmed) yield break;
        actAnimating = true;
        lastArmedState = armState;
        _animator.SetTrigger("disarm");

        int weaponLeft = 0;
        int weaponRight = 0;

        switch (leftHandWeaponHang)
        {
            case WeaponHang.noWeapon:

                break;
            default:
                weaponLeft = handLeft;
                break;
        }
        switch (rightHandWeaponHang)
        {
            case WeaponHang.noWeapon:

                break;
            default:
                weaponRight = handRight;
                break;
        }
        _animator.SetLayerWeight(handLeft, 1);
        _animator.SetLayerWeight(handRight, 1);

        //while (!actAnimatingEnd) { yield return null; }


        //while (actAnimatingEnd||) { yield return null; }

        _animator.SetLayerWeight(handLeft, 0);
        _animator.SetLayerWeight(handRight, 0);

        actAnimating = false;

    }
    IEnumerator DisarmCoroutine()
    {
        if (armState == ArmState.unarmed) yield break;

        actAnimating = true;

        lastArmedState = armState;
        _animator.SetTrigger("disarm");

        int armLayer = 0;
        switch (armState)
        {
            case ArmState.armSwSh:
                armLayer = torsoSwSh;
                break;
            case ArmState.armMage:
                armLayer = torsoMage;
                break;
        }
        _animator.SetLayerWeight(armLayer, 1);

        while (!actAnimatingEnd) { yield return null; }

        _animator.SetBool(armState.ToString(), false);
        armState = ArmState.unarmed;
        _animator.SetBool(armState.ToString(), true);
        _animator.SetTrigger("newState");


        SwShRest();

        while (actAnimatingEnd) { yield return null; }

        _animator.SetLayerWeight(armLayer, 0);

        actAnimating = false;
    }

    public float meleDistance = 1.5f;
    public float mageDistance = 15;
    public bool atkFromCurrDistance = false; // curent weapon atk distance

    //object[] targetData = new object[3];// 0-Body-Target,1-float-angle to target; 2-float-dist to targ
    public float curDistToTurg;
    public override void UpdateBot(float deltaTime)
    {
        if (!_nma) return;
        if (!_nma.enabled) return;

        Vector3 wayPointTarget = new Vector2();

        //GetTargetData(); //targetData
        //botBodyTarget = (Body)targetData[0];
        //angle = (float)targetData[1];
        //float distance = (float)targetData[2];
        // float curDistToTurg;
        GetTargetDataOUT(ref botBodyTarget, out angle, out curDistToTurg);


       // if distance to close then move perpendicular to target movement



        //  не так. тело должно продолжать уметь вращаться
        //if (botBodyTarget && curDistToTurg < meleDistance * 3)
        //    _nma.updateRotation = false;
        //else
        //    _nma.updateRotation = true;

        if (botBodyTarget)
        {
            // Vector3 cross = Vector3.Cross(_transform.forward, botBodyTarget._transform.position - _transform.position);
            // if (cross.y < 0) targetData[1] = -1 * (float)targetData[1];



            //distance = Vector3.Distance(botBodyTarget._transform.position, _transform.position);
            //  bool updateRotation = distance < 3;
            //  _nma.updateRotation = updateRotation;// nav mesh auto rotate  nMA.updateRotation = true;


            // _nma.updateRotation = atkAnimating;

            //   if (!atkAnimating)
            _nma.destination = botBodyTarget._transform.position;

            //Need to modify _nma.destination depending on BEHAVIOUR


            Vector3 cross = Vector3.Cross(_transform.forward, botBodyTarget._transform.position - _transform.position);
            if (cross.y < 0) angle = -angle;
            Quaternion toRotation = Quaternion.Euler(Vector3.up * angle + _transform.eulerAngles);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, toRotation, bodyAcceleration * deltaTime);


            if (atkAnimating)
            {
                bodySpeedCurrent = Mathf.Lerp(bodySpeedCurrent, _bodySpeed / 2, Time.deltaTime * bodyAcceleration);
                _nma.angularSpeed = bodyAngularSpeed / 2;
            }
            else
            {
                bodySpeedCurrent = Mathf.Lerp(bodySpeedCurrent, _bodySpeed, Time.deltaTime * bodyAcceleration);
                _nma.angularSpeed = bodyAngularSpeed;
                if (!actAnimating)
                {
                    BotGetDesiredWeapon(out nextArmedState, out atkFromCurrDistance, curDistToTurg);
                }
            }

            _nma.speed = bodySpeedCurrent;


            ///!!!!!!!! make  atack only if on distance
            if (atkFromCurrDistance)// && angle <= 70) !!
                ActionStack(ActionUnit.attacking);

        }
        else
        {
            if (!actAnimating && !atkAnimating)
                nextArmedState = ArmState.unarmed;
            // PUT here botBehaviour switcher (in idle will change what bot does)
        }

        if (!actAnimating && !atkAnimating && nextArmedState != armState)
        {
            Arm(nextArmedState);
            if (nextArmedState == ArmState.armMage)
                ActionStack(ActionUnit.toArmStateMage);
            else if (nextArmedState == ArmState.armSwSh)
                ActionStack(ActionUnit.toArmStateSwSh);
            else if (nextArmedState == ArmState.unarmed)
                ActionStack(ActionUnit.toArmStateUnurmed);
        }

        AnimateBody(_nma.velocity, deltaTime);
        /*
        if (nextFromStack && botBodyTarget)
        {
            Vector3 cross = Vector3.Cross(_transform.forward, botBodyTarget._transform.position - _transform.position);
            if (cross.y < 0) angle = -angle;
            Quaternion toRotation = Quaternion.Euler(Vector3.up * angle + _transform.eulerAngles);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, toRotation, bodyAcceleration * deltaTime);
        }
        */
        //  Debug.DrawRay(_transform.position + _ccBody.center, _nma.destination+ _transform.position, Color.red);
        //  _nma.transform.localPosition = Vector3.zero;

    }

    void BotGetDesiredWeapon(out ArmState nextArmedState, out bool atkDistance, float distance)
    {
        nextArmedState = ArmState.armMage;
        //distance = Vector3.Distance(botBodyTarget._transform.position, _transform.position);
        atkDistance = (distance <= mageDistance);// for mage weapon
        if (distance < spotDistance / 1.5f)
        {
            nextArmedState = ArmState.armSwSh;
            atkDistance = (distance <= meleDistance);// for mele weapon
        }
        // return result;
    }

    Collider[] collisions;

    /*Body DEPRICATED_GetMoveTarget()
      {
          Body result = null;
          // Vector3 myCenter = _ccBody.center + _transform.position;
          Vector3 myCenter = new Vector3(0, 0.86f, 0) + _transform.position;
          collisions = Physics.OverlapSphere(myCenter, followDistance, _targetLayer);// follow dist is bigger than spot dist

          float closestDistance = followDistance * 2;//float.MaxValue;

          for (int i = 0; i < collisions.Length; i++)
          {
              Body body = collisions[i].GetComponent<Body>();
              if (!body._tagFriendList.Contains(_tag))//collision not in a friendly group
              {
                  Debug.DrawRay(myCenter, body._transform.position - myCenter, Color.red);
                  //taking closest
                  float dis = Vector3.Distance(myCenter, body._transform.position);
                  float angl = Vector3.Angle(_transform.forward, body._transform.position - _transform.position);
                  if ((body == botBodyTarget) || (dis <= spotDistance && angl <= 120) || (dis <= spotBackDistance))

                      if (dis < closestDistance)
                      {
                          closestDistance = dis;
                          result=body;
                      }
              }
          }
          return result;
      }*/
    /*  void DEPRICATED_GetTargetData()
      {
          // 0-Body-Target,
          // 1 -float-angle to target;
          // 2-float-dist to targ
          targetData[0] = (Body)null;
          targetData[1] = (float)180;
          targetData[2] = (float)1000;
          // Vector3 myCenter = _ccBody.center + _transform.position;
          Vector3 myCenter = new Vector3(0, 0.86f, 0) + _transform.position;
          collisions = Physics.OverlapSphere(myCenter, followDistance, _targetLayer);// follow dist is bigger than spot dist

          float closestDistance = followDistance * 2;//float.MaxValue;

          for (int i = 0; i < collisions.Length; i++)
          {
              Body body = collisions[i].GetComponent<Body>();
              if (!body._tagFriendList.Contains(_tag))//collision not in a friendly group
              {
                  Debug.DrawRay(myCenter, body._transform.position - myCenter, Color.red);
                  //taking closest
                  float dis = Vector3.Distance(myCenter, body._transform.position);
                  targetData[1] = (float)Vector3.Angle(_transform.forward, body._transform.position - _transform.position);

                  if ((body == botBodyTarget) || (dis <= spotDistance && Mathf.Abs((float)targetData[1]) <= 120) || (dis <= spotBackDistance))

                      if (dis < closestDistance)
                      {
                          closestDistance = dis;
                          targetData[0] = (Body)body;
                      }
              }
          }
          targetData[2] = (float)closestDistance;
      }*/

    void GetTargetDataOUT(ref Body botTarget, out float ang, out float closestDistance)
    {
        Body botTempTarget = null;
        ang = 180;
        closestDistance = followDistance * 2;//float.MaxValue;

        // dist = 1000;
        // Vector3 myCenter = _ccBody.center + _transform.position;
        Vector3 myCenter = new Vector3(0, 0.86f, 0) + _transform.position;
        collisions = Physics.OverlapSphere(myCenter, followDistance, _targetLayer);// follow dist is bigger than spot dist


        for (int i = 0; i < collisions.Length; i++)
        {
            Body body = collisions[i].GetComponent<Body>();
            if (!body._tagFriendList.Contains(_tag))//collision not in a friendly group
            {
                Debug.DrawRay(myCenter, body._transform.position - myCenter, Color.red);
                //taking closest
                float dis = Vector3.Distance(myCenter, body._transform.position);
                ang = Vector3.Angle(_transform.forward, body._transform.position - _transform.position);

                if ((body == botBodyTarget) || (dis <= spotDistance && Mathf.Abs(ang) <= 120) || (dis <= spotBackDistance))

                    if (dis < closestDistance)
                    {
                        closestDistance = dis;
                        botTempTarget = body;
                    }
            }
        }
        // if (botTempTarget != null) botTarget = botTempTarget;
        botTarget = botTempTarget;
    }

    void AnimateBody(Vector3 velocity, float deltaTime)
    {
        _moveForward = Vector3.Dot(velocity, _transform.forward);
        currAcceleration = (_moveForward >= 0) ? bodyAcceleration : bodyAcceleration / 2;
        _moveForwardSmoothed = Mathf.Lerp(_moveForwardSmoothed, _moveForward, currAcceleration * deltaTime);

        _moveRight = Vector3.Dot(velocity, _transform.right);
        _moveRightSmoothed = Mathf.Lerp(_moveRightSmoothed, _moveRight, currAcceleration * deltaTime);

        _animator.SetFloat("moveForward", _moveForward / 4);
        _animator.SetFloat("moveRight", _moveRight / 4);
        //_animator.SetFloat("moveForward", _moveForward * _moveForward / 10);
        //_animator.SetFloat("moveRight", _moveRight * _moveRight / 10);
        //float xx = 1;
        //float yy = 1;
        //if (_moveForward > 0.5f)
        //{
        //    _moveForward -= 0.5f;
        //}

        //xx = Mathf.Clamp01(xx);
        //_animator.SetFloat("moveForward", _moveForward / (_moveForward / 2));
        //_animator.SetFloat("moveRight", _moveRight / (_moveRight / 2));
    }

    public Vector2 randAtk = new Vector2();
    int cnt = 1000;
    int CNT(int max)
    {
        cnt++;
        if (cnt >= max) cnt = 0;
        return cnt;
    }

    public override void Attacking()
    {
        if (armState == ArmState.unarmed)
            nextFromStack = true;

        if (armState != ArmState.unarmed && !atkAnimating && !actAnimating)
            StartCoroutine(AttackDo());


    }

    IEnumerator AttackDo()
    {
        atkAnimating = true;

        int armLayer = 0;
        switch (armState)
        {
            case ArmState.armSwSh:
                armLayer = torsoSwSh;
                randAtk.x = CNT(2);// Random.insideUnitCircle.normalized;
                randAtk.y = 0;
                break;
            case ArmState.armMage:
                armLayer = torsoMage;
                randAtk.x = CNT(2);// Random.Range(0, 2);
                randAtk.y = 0;
                break;
        }

        _animator.SetLayerWeight(armLayer, 1);
        _animator.SetTrigger("atk");

        _animator.SetFloat("atkX", randAtk.x);
        _animator.SetFloat("atkY", 0);

        atkAnimatingStart = true;
        while (atkAnimatingStart) { yield return null; }
        atkAnimatingMiddle = true;
        DamageTrigger(true);
        while (atkAnimatingMiddle) { yield return null; }
        DamageTrigger(false);
        atkAnimatingEnd = true;
        while (atkAnimatingEnd) { yield return null; }

        //Debug.Log("AttackDo exit");

        _animator.SetLayerWeight(armLayer, 0);
        atkAnimating = false;
        nextFromStack = true;
    }

    void DamageTrigger(bool startTrigger)
    {
        if (startTrigger)
            switch (armState)
            {
                case ArmState.armSwSh:
                    _testSword.WeaponAttacking(true);
                    break;
                case ArmState.armMage:

                    break;
            }
        else
            switch (armState)
            {
                case ArmState.armSwSh:
                    _testSword.WeaponAttacking(false);
                    break;
                case ArmState.armMage:

                    break;
            }
    }

    public override void Jumping()
    {
        if (!_ccBody) return;
        if (_ccBody.isGrounded)
        //Physics.Raycast(_transform.position, -Vector3.up, collider.bounds. + 0.1);
        {
            canjump = true;
            //  if (Input.GetButton("Jump") && canjump && Time.time > nextjumpTime)
            if (canjump && Time.time > nextjumpTime)
            {
                jumpParameter = (Mathf.Abs(_moveDir.x) >= Mathf.Abs(_moveDir.z)) ? Mathf.Abs(_moveDir.x) : Mathf.Abs(_moveDir.z);
                _animator.SetBool("jump", true);
                _animator.SetFloat("jumpParam", jumpParameter);
                _moveDir.y = jumpHeight;
                // Debug.Log("jumpParameter = "+ jumpParameter);
                canjump = false;
            }
        }
    }
    void Falling()
    {
        if (!_ccBody) return;
        if (_nma) return;
        if (_animator && !_ccBody.isGrounded)
        {
            _animator.SetBool("jump", false);
            //_animator.ResetTrigger("grounded");
            _moveDir.y -= gravity * Time.deltaTime;
            nextjumpTime = Time.time + jumpinterval;
        }
        _animator.SetBool("grounded", _ccBody.isGrounded);
    }

    /*
    void KeyboardWalk()
    { // ходьба для клавиатуры (для джоя это значением джоя берется)
        if (Input.GetKeyDown(KeyCode.LeftShift)) _keyboardWalk = !_keyboardWalk;
        if (_keyboardWalk)
        {
            _moveDirection.x = Mathf.Clamp(_moveDirection.x, -0.5f, 0.5f);
            _moveDirection.z = Mathf.Clamp(_moveDirection.z, -0.5f, 0.5f);
        }
    }
  
    public void InvokeJump()
    {
        _bp._animator.SetTrigger("jumpTr");
    }
    */

    float behTimer = 0;
    BotState botState;
    enum BotState
    {
        armed,
        idle,
        chasingFar,
        chasingMiddle,
        chasingNear        
    }
    void Behaviour()
    { // will switch bot moving directions while target in attack destance
        switch (botState)
        {
            case BotState.armed:

            break;
            case BotState.idle:

                break;
            case BotState.chasingFar:

                break;
            case BotState.chasingMiddle:

                break;
            case BotState.chasingNear:

                break;
        }
    }



}
