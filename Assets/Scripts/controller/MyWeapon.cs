using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(BodyPrefs))]
//[RequireComponent(typeof(BodyPrefs))]
public class MyWeapon : MonoBehaviour
{
    public Transform pointAtRest;
    public Transform pointAtFight;
    public Transform pointTrail;

    public float _damage = 2f;

    protected Body _body;
    public Body _Body
    {
        set { _body = value; }
        get { return _body; }
    }
    protected Transform _transform;
    public Transform _Transform
    {
        set { _transform = value; }
        get { return _transform; }
    }

    protected MeshCollider _weaponMesh;
    //public MeshCollider _weaponMesh;
    protected TrailRenderer _weaponTrailRenderer;
    //public TrailRenderer _weaponTrailRenderer;

    protected Rigidbody _weaponRigidbody;

    protected bool _weaponAttacking = false;

    protected List<Body> _successAttack = new List<Body>(); // list of bodys susccess attacked - hit will not be prevented on thems shields
    protected List<Body> _noSuccessAttack = new List<Body>();
    protected bool _atkIsBlocked = false;

    void Awake()
    {
        _Transform = transform;

        _weaponTrailRenderer = GetComponent<TrailRenderer>();

        _weaponMesh = GetComponent<MeshCollider>();
        if (_weaponMesh)
            _weaponMesh.isTrigger = true;

        _weaponRigidbody = GetComponent<Rigidbody>();

        _weaponRigidbody.freezeRotation = true;
        _weaponRigidbody.useGravity = false;
        _weaponRigidbody.isKinematic = false;

        WeaponAttacking(false);
    }


    //public void WeaponTrailSwitch(bool turner)
    //{
    //    if (_testWeaponTrailRenderer) _testWeaponTrailRenderer.enabled = turner;
    //}
    //public void WeaponMeshSwitch(bool turner)
    //{
    //    if (_testWeaponMesh) _testWeaponMesh.enabled = turner;
    //}
    public virtual void WeaponAttacking(bool turner)
    {
        //_successAttack = new List<Body>();
        //_noSuccessAttack = new List<Body>();
        //if (_weaponTrailRenderer) _weaponTrailRenderer.enabled = turner;
        //if (_weaponMesh) _weaponMesh.enabled = turner;
    }
    /*
    Всем добрый день. 
В обьект с CharacterController вложен обьект с меш коллайдером(тригер выключен) - моб с щитом.
в OnEnterCollider (Collider c) вывожу строку Debug.Log(c.name)
в выводе всегда имя моба и никогда имя щита.
Мне это не понятно. По идее, если событие происходит по щиту, то и имя щита должен увидеть, а не родительского обьекта. Что не так?
    */

    public void OnCollisionEnter(Collision collision)
    {
        Debug.LogError("WTF i dont use collisions");
        //    Debug.Log("hit on " + collision.gameObject.name);
        //    foreach (ContactPoint contact in collision.contacts)
        //    {
        //        Debug.DrawRay(contact.point, contact.normal, Color.white);
        //    }
        //    // if (collision.relativeVelocity.magnitude > 2)
        //    //     audio.Play();
    }
    void OnTriggerEnter(Collider c)
    {
        if (!_weaponAttacking) return;

        // Debug.Log("0) hit on " + c.name);
        if (_atkIsBlocked) return;//on current turn, if i been blocked before, i will not hit anyone

        Body body;
        MyShield shield = c.GetComponent<MyShield>();

        if (shield)
        {
            body = shield._body;
            // Debug.LogWarning("2)  " + _Body.name + " atacked shield of " + body.name);
        }
        else
            body = c.GetComponent<Body>();

        if (!body) return;// hit is not on body or equipment, so IGNORE
        if (_Body == body) return;// if it is my equipment

        if (shield)
        {
            if (body._tag != _Body._tag)//&&!_Body._tagFriendList.Contains(body._tag))// not a team member
            {
                if (!_successAttack.Contains(body))// if i have not hit the body already
                {
                    _noSuccessAttack.Add(body);
                    // Debug.Log("3) "+_Body.name + " atacked shield of " + body.name);
                    Debug.Log(body.name + " blockea atk of " + _Body.name);
                    c.SendMessage("BlockedAttack", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else
        {
            if (_Body._tag != body._tag)//&&!_Body._tagFriendList.Contains(body._tag))// not a team member
            {
                if (!_noSuccessAttack.Contains(body)) // if i didnt hit his shield
                {
                    _successAttack.Add(body);
                    //c.SendMessage("SetDamage", _damage, SendMessageOptions.DontRequireReceiver);
                    object[] damageInfo = new object[3];// { _damage, _Body, body } ;
                    damageInfo[0] = _damage;
                    damageInfo[1] = _Body;
                    damageInfo[2] = body;

                    c.SendMessage("SetHit", damageInfo, SendMessageOptions.DontRequireReceiver);
                   
                }
                //else{
                //    Debug.Log("4) body attacked but blocked before");
                //}
            }
        }
    }

}
