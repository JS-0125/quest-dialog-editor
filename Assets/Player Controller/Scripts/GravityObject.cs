using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : OutlineObject
{
    private Rigidbody _rigidbody;
    [SerializeField] private GravityLevel defualtLevel = GravityLevel.Common;
    [SerializeField] private float defualtDuration = 5f;

    private float addedGravity;
    private GravityLevel currLevel;
    private GravityLevel tempLevel;
    private float duration;
    public int Level
    {
        get => (int)currLevel;
    }

    private bool isSelected = false;        // 중력을 바꿀 수 있는 상태임을 나타내는 flag
    private bool isAdapted = false;      // 변경된 중력이 적용되어 있는지 나타내는 flag
    public bool IsSelected
    {
        get => isSelected;
        set => isSelected = value;
    }

    private Transform _modelTransform;  // 모델 본체의 transform
    private Material uiMaterial;
    private Color[] uiColors;
    public Color[] UIColors
    {
        set => uiColors = value;
    }

    void Awake()
    {
        Renderer childRenderer = GetComponentInChildren<Renderer>();
        List < Material > materials= new List<Material>();
        childRenderer.GetMaterials(materials);
        uiMaterial = materials[1];
        GameObject model = childRenderer.gameObject;

        _modelTransform = model.transform;

        // Gravity
        currLevel = defualtLevel;
        _rigidbody = GetComponent<Rigidbody>();
        duration = defualtDuration;
        _rigidbody.mass = Mathf.Clamp(0.5f - (int)currLevel, 0.5f, 16f);
        uiColors = new Color[5];
        uiColors = FindObjectOfType<SkillManager>().GravityColors;
    }

    private void Update()
    {
        if ((int)currLevel < (int)GravityLevel.Light)
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        else
        {
            _rigidbody.constraints =  RigidbodyConstraints.FreezeRotation;

        }

        if (isSelected)
        {
            ChangeGravity();
            SetUI(0.5f, uiColors[((int)tempLevel + 16) / 5]);
        }

        if (isAdapted)
        {
            CheckDuration();
        }
        MoveUpAndDown();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((int)currLevel >= (int)GravityLevel.Light)
        {
            if (collision.transform.CompareTag("Player"))
            {
                PlayerAction player = collision.transform.GetComponentInParent<PlayerAction>();
                player.SetPullAnimation(true);                
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((int)currLevel >= (int)GravityLevel.Light)
        {
            if (collision.transform.CompareTag("Player"))
            {
                PlayerAction player = collision.transform.GetComponentInParent<PlayerAction>();
                player.SetPullAnimation(false);

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((int)currLevel >= (int)GravityLevel.Light)
        {
            if (other.transform.CompareTag("Player"))
            {
                PlayerAction player = other.transform.GetComponentInParent<PlayerAction>();
                player.SetPullAnimation(true);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((int)currLevel >= (int)GravityLevel.Light)
        {
            if (other.transform.CompareTag("Player"))
            {
                PlayerAction player = other.transform.GetComponentInParent<PlayerAction>();
                player.SetPullAnimation(false);

            }
        }
    }

    private void ChangeGravity()
    {
        addedGravity += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000;

        if((int)currLevel + addedGravity <= (int)GravityLevel.MoreHeavy)
        {
            tempLevel = GravityLevel.MoreHeavy;
        } 
        else if((int)currLevel + addedGravity <= (int)GravityLevel.Heavy)
        {
            tempLevel = GravityLevel.Heavy;
            Debug.Log("Heavy");
        }
        else if ((int)currLevel + addedGravity <= (int)GravityLevel.Common)
        {
            tempLevel = GravityLevel.Common;
            Debug.Log("Common");
        }
        else if ((int)currLevel + addedGravity <= (int)GravityLevel.Light)
        {
            tempLevel = GravityLevel.Light;
            Debug.Log("Light");
        }
        else 
        {
            tempLevel = GravityLevel.MoreLight;
            Debug.Log("More Light");
        }

    }

    public void CheckDuration()
    {
        duration -= Time.deltaTime;
        if(duration < 0)
        {
            duration = defualtDuration;
            isAdapted = false;
            currLevel = defualtLevel;
            tempLevel = defualtLevel;
            _rigidbody.mass = Mathf.Clamp(0.5f - (int)currLevel, 0.5f, 16f);
        }
    }

    public void AdaptGravity()
    {
        isSelected = false;
        isAdapted = true;
        currLevel = tempLevel;
        duration = defualtDuration;
        addedGravity = 0f;
        _rigidbody.mass = Mathf.Clamp(0.5f - (int)currLevel,0.5f,16f) + 0.5f;
    }

    public void MoveUpAndDown()
    {
        _rigidbody.velocity += Vector3.up*(int)currLevel * Time.deltaTime;
    }
        
    public override GameObject GetModel()
    {
        if (_modelTransform != null)
            return _modelTransform.gameObject;
        return null;
    }

    public void SetUI(float alpha, Color color)
    {
        uiMaterial.color = new Vector4(1f, 1f, 1f, alpha);
        uiMaterial.SetColor("_EmissionColor", color);
    }
}
