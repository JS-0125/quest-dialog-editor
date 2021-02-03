using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private CharacterController _characterController;
    Animator _animator;

    [SerializeField] private PlayerState _state = PlayerState.Idle;
    internal PlayerState State { get => _state; set => _state = value; }

    // animation
    private readonly int _hashJumpStart = Animator.StringToHash("JumpStart");
    private readonly int _hashJumping = Animator.StringToHash("Jumping");
    private readonly int _hashLand = Animator.StringToHash("Land");
    private readonly int _hashInput = Animator.StringToHash("Input");
    private readonly int _hashHor = Animator.StringToHash("InputHor");
    private readonly int _hashVer = Animator.StringToHash("InputVer");
    private readonly int _hashPull = Animator.StringToHash("Pull");
    
    // Move
    [Header("Move")]
    private Vector2 _runData = Vector2.zero;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float walkSpeed = 3f;

    //Jump
    [Header("Jump")]
    [SerializeField] private float jumpPower = 300f;
    [SerializeField] private float jumpDuration= 0.28f;
    [SerializeField] private float landMinDist = 1f;
    private float _jumpDuration;

    // Fall
    [Header("Fall")]
    [SerializeField] private float mass = 10f;
    [SerializeField] private float minGroundedDist = 0.2f;
    private Vector3 _fallVelocity = Vector3.zero;
    private bool _isGrounded;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _jumpDuration = jumpDuration;
        
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Update()
    {
        JumpStart();
    }

    private void FixedUpdate()
    {
        Fall();
        Move();
        Jumping();
        CheckisGrounded();
    }

    private void Move()
    {
        _runData.x = Input.GetAxis("Horizontal");
        _runData.y = Input.GetAxis("Vertical");
        float speed = walkSpeed;

        if (_state != PlayerState.JumpStart && _state != PlayerState.Jumping)
        {
            if (_runData == Vector2.zero)
            {
                if (_state == PlayerState.Run)
                    _state = PlayerState.Idle;
            }
            else
            {
                if (_runData.y > 0)
                    speed = runSpeed;
                _state = PlayerState.Run;
            }
            // set animation
            _animator.SetFloat(_hashInput, _runData.magnitude);
            _animator.SetFloat(_hashHor, _runData.x);
            _animator.SetFloat(_hashVer, _runData.y);
        }
        else
            speed = runSpeed;

        var vec = transform.right * _runData.x + transform.forward * _runData.y;
        _characterController.Move(vec * speed * Time.deltaTime);     

    }

    void CheckisGrounded()
    {
        Debug.DrawRay(transform.position , -transform.up * landMinDist, Color.red);
        //RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out var hit, landMinDist))
        {
            if (hit.transform.gameObject.CompareTag("Floor"))
            {
                if (_state != PlayerState.JumpStart && !_isGrounded && _fallVelocity.y <= 0)
                {
                    float dist = transform.position.y - hit.point.y;
                    if (_state == PlayerState.Jumping)
                    {
                        if (dist < landMinDist)
                        {
                            _jumpDuration = jumpDuration;
                            _animator.SetTrigger(_hashLand);
                            _state = PlayerState.Idle;
                        }
                    }

                    if (dist < minGroundedDist)
                    {
                        var foot = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                        transform.SetPositionAndRotation(foot, transform.rotation);
                        _isGrounded = true;
                        if (_state == PlayerState.Jumping)
                        {
                            _jumpDuration = jumpDuration;
                            _animator.SetTrigger(_hashLand);
                        }

                    }
                }
            }
            else
            {
                _isGrounded = false;
            }
            return;
        }     
    }

    private void Fall()
    {
        float _accY = -9.8f;
        _fallVelocity.y += _accY * Time.deltaTime;

        _characterController.Move(_fallVelocity*Time.deltaTime);

    }

    private void JumpStart()
    {
        if (_isGrounded && _state != PlayerState.Jumping)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _isGrounded = false;
                _state = PlayerState.JumpStart;
                _animator.SetTrigger(_hashJumpStart);
            }
        }
    }

    private void Jumping()
    {
        if (_state != PlayerState.JumpStart) return;
        if(_jumpDuration <= 0)
        {
            // Start Jump
            _animator.SetTrigger(_hashJumping);
            _fallVelocity = Vector3.zero;
            float acc = (jumpPower / mass) * 10;
            _fallVelocity.y += acc * Time.deltaTime;
            _state = PlayerState.Jumping;
        }
        else
        {
            _jumpDuration -= Time.deltaTime;
        }
    }

    public void SetPullAnimation(bool val)
    {
        _animator.SetBool(_hashPull, val);
    }
}
