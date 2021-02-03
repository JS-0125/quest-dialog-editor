using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    [SerializeField] private float mouseXSensitivity = 100f;
    [SerializeField] private float mouseYSensiticity = 10f;
    [SerializeField] private float thirdMinY = -3.3f;
    [SerializeField] private float thirdMaxY = 0.6f;
    [SerializeField] private float firstMinY = -1.73f;
    [SerializeField] private float firstMaxY = 1.3f;
    

    [SerializeField] private Transform thirdCameraFollow;
    [SerializeField] private Transform firstCameraLookAt;

    private SkillManager _skillManager;

    private float _defualt3rdHeight;
    private float _defualt1stHeight;
    private float _thirdDeltaY = 0f;
    private float _firstDeltaY = 0f;

    private void Awake()
    {
        _defualt1stHeight = firstCameraLookAt.position.y - transform.position.y;
        _defualt3rdHeight = thirdCameraFollow.position.y - transform.position.y;
        _skillManager = GetComponent<SkillManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float rotX, rotY;
        rotX = Input.GetAxis("Mouse X");
        rotY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * (rotX * Time.deltaTime * mouseXSensitivity));

        float deltaY = rotY * Time.deltaTime * mouseYSensiticity;
        if (_skillManager.IsKeyHolding)
        {
            // 1st
            // min / max ó��
            float checkY = (firstCameraLookAt.position.y + deltaY) - transform.position.y;
            if (checkY < firstMinY)
            {
                rotY = 0f;
            }
            else if (checkY > thirdMaxY)
            {
                rotY = 0f;
            }
            firstCameraLookAt.Translate(Vector3.up * rotY * Time.deltaTime * mouseYSensiticity);



            // 3rd ó��
            if(thirdCameraFollow.position.y - transform.position.y != _defualt3rdHeight)
            {
                thirdCameraFollow.SetPositionAndRotation(new Vector3(thirdCameraFollow.position.x, transform.position.y + _defualt3rdHeight, thirdCameraFollow.position.z),transform.rotation);
            }

            if (_thirdDeltaY != 0f)
                _thirdDeltaY = 0f;
        }
        else
        {
            // 3rd          
            // �浹 ó��
            Debug.DrawRay(thirdCameraFollow.position - Vector3.up * deltaY, Vector3.down, Color.red, 0.5f);
            if (Physics.Raycast(thirdCameraFollow.position - Vector3.up * deltaY, Vector3.down, out var hit, 0.5f))
            {
                 if(thirdCameraFollow.position.y - deltaY - hit.transform.position.y <= 0.2f)
                {
                    rotY = 0f;
                }
            }

            // min / max ó��
            float checkY = (thirdCameraFollow.position.y - deltaY) - transform.position.y;
            if (checkY < thirdMinY)
            {
                rotY = 0f;
            }
            else if (checkY  > thirdMaxY)
            {
                rotY = 0f;
            }


            // ī�޶� ���� �̵�
            thirdCameraFollow.Translate(Vector3.up * (-rotY * Time.deltaTime * mouseYSensiticity));

            // 1st �������� ��ȯ�� ���� ó��
            if (firstCameraLookAt.position.y - transform.position.y != _defualt1stHeight)
            {
                firstCameraLookAt.SetPositionAndRotation(new Vector3(firstCameraLookAt.position.x, transform.position.y + _defualt1stHeight, firstCameraLookAt.position.z), transform.rotation);
            }
            if (_firstDeltaY != 0f)
                _firstDeltaY = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(thirdCameraFollow.position, 0.3f);
    }
}
