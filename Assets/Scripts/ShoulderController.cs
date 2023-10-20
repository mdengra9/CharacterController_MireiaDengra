using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShoulderController : MonoBehaviour
{
    private CharacterController _controller;
    private Transform _camera;
    private Transform _lookAtTransform;

    private float _horizontal;
    private float _vertical;

    //variables para velocidad, salto y gravedad
    [SerializeField] private float playerSpeed = 5;
    [SerializeField] private float _jumpHeight = 1;
    private float _gravity = -9.81f; 
    private Vector3 _playerGravity;

    //variables para rotación
    [SerializeField] float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    //variables para sensor
    [SerializeField] private Transform _sensorPosition;
    [SerializeField] private float _sensorRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;

    [SerializeField] private AxisState xAxis;
    [SerializeField] private AxisState yAxis;

    public GameObject normalCamera;
    public GameObject aimCamera;

    // Start is called before the first frame update
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main.transform;
        _lookAtTransform = GameObject.Find("Look At").transform;
    }

    // Update is called once per frame
    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetButton("Fire2"))
        {
            normalCamera.SetActive(false);
            aimCamera.SetActive(true);
        }

        else
        {
           normalCamera.SetActive(true);
           aimCamera.SetActive(false); 
        }

        Jump();
        Movement();
    }

    void Jump()
    {
        _isGrounded = Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);
        if(_isGrounded && _playerGravity.y < 0)
        {
            _playerGravity.y = 0;
        }

        if(_isGrounded && Input.GetButtonDown("Jump"))
        {
            _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
        }

        _playerGravity.y += _gravity * Time.deltaTime;
        _controller.Move(_playerGravity * Time.deltaTime);
    }

    void Movement()
    {
        Vector3 move = new Vector3 (_horizontal, 0 , _vertical).normalized;

        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, xAxis.Value, 0);
        _lookAtTransform.eulerAngles = new Vector3(yAxis.Value, transform.eulerAngles.y, 0);

        Vector3 direction = new Vector3(_horizontal, 0, _vertical);

        if(direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _controller.Move(moveDirection.normalized * playerSpeed * Time.deltaTime); 
        }
    }

}
