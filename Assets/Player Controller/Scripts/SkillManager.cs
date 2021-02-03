using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [SerializeField] Text focus;
    private Animator _animator;

    readonly private int _hashSkill = Animator.StringToHash("Skill");

    [SerializeField] private int playerID = 0;          // 0: Gravity   / 1: Create Obj
    public int PlayerID
    {
        get => playerID;
    }

    private bool isKeyHolding = false;
    public bool IsKeyHolding
    {
        get => isKeyHolding;
    }

    [Header("Gravity")]
    private GravityObject _selection;
    public GravityObject Selection
    {
        set => _selection = value;
    }
    [SerializeField] private Material gravityUIMaterial;
    [SerializeField] private Color[] gravityUIColors = new Color[5];
    public Color[] GravityColors
    {
        get => gravityUIColors;
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        // input key
        if(Input.GetButtonDown("Skill"))
        {
            isKeyHolding = true;
            focus.enabled = true;
        }

        if(Input.GetButtonUp("Skill"))
        {
            isKeyHolding = false;
            focus.enabled = false;
        }

        if(Input.GetButtonDown("LeftClick"))
        {
            isKeyHolding = false;
            focus.enabled = false;
            if (_selection != null)
            {
                Debug.Log("is Selecting");
                if (_selection.IsSelected)
                {
                    _selection.AdaptGravity();
                    _selection.SetUI(0f, Color.white);
                }
                else
                {
                    _selection.IsSelected = true;
                    _selection.SetUI(0.5f, Color.white);
                }

            }
        }

        // set animation
        _animator.SetBool(_hashSkill, isKeyHolding);

        if(playerID == 0)
        {
                
        }


    }
}
