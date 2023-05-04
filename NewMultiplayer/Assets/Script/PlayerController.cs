using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private GameObject ui;
    [SerializeField] private Image healthbarImage;
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] private Item[] items;

    private int itemIndex;
    private int previousItemIndex = -1;
    
    private bool grounded;
    private float verticalLookRotation;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    
    private Rigidbody rb;
    private PhotonView PV;

    private const float maxHealth = 100f;
    private float currentHealth = maxHealth;
    

    private PlayerManager playerManager;

    [SerializeField]private GameObject shootingSound;
    [SerializeField]private GameObject jumpingSound;
    private bool isShooting = false;

    [SerializeField] public TMP_Text textTimer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb); 
            Destroy(ui);
        }
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;
        
        Look();
        Move();
        Jump();

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") <0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex -1);
            }
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            items[itemIndex].Use();
            isShooting = true;
            //timer = Time.time;
        }
        else
        {
            isShooting = false;
        }
        
        /*
        else if (Input.GetMouseButton(0))
        {
            if (Time.time - timer > holdDuration)
            {
                timer = float.PositiveInfinity;
                items[itemIndex].Use();
            }
            else
            {
                timer = float.PositiveInfinity;
            }
        }
        */

        if (transform.position.y < -10f) // respawn
        {
            Die();
        }

        if (!isShooting)
        {
            shootingSound.SetActive(true);
        }
        else
        {
            shootingSound.SetActive(false);
        }

        if (!isJumping)
        {
            jumpingSound.SetActive(true);
        }
        else
        {
            jumpingSound.SetActive(false);
        }
    }
    

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    private bool isJumping = false;
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }
    }
    

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void EquipItem(int _index)
    {
        if(_index == previousItemIndex)
            return;
        
        itemIndex = _index;
        
        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void SetGroundedState(bool _grouned)
    {
        grounded = _grouned;
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount)* Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        //Debug.Log("took damage : " + damage);
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        
        //if (!PV.IsMine)
        //    return;
        //Debug.Log("took damage : " + damage);
        
        currentHealth -= damage;
        healthbarImage.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    void Die()
    {
        playerManager.Die();
    }
}
