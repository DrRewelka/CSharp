using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    Vector3 velocity, playerRotation, cameraRotation;
    private Transform cam;

    [SerializeField] float speed = 60;
    [SerializeField] GameObject meat;
    int meatsAmount;
    [SerializeField] float meatThrowForce;
    [SerializeField] float meatThrowTorque;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        playerRotation = new Vector3(0, cam.eulerAngles.y, 0);
    }

    private void Start()
    {
        meatsAmount = GameManagerScript.Instance.Meats;
    }

    void FixedUpdate()
    {
        controller.Move(transform.TransformDirection(velocity) * Time.fixedDeltaTime);
    }

	void Update ()
    {
        if (Time.timeScale == 0)
            return;

        //WASD
        var direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        direction.y = -9.81f;
        velocity = direction * Time.deltaTime * speed;

        //Mouse Movement
        var rotation = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime;

        var playerRotationVector = new Vector3(0, rotation.x, 0);
        var cameraRotationVector = new Vector3(-rotation.y, 0, 0);

        playerRotation += playerRotationVector * 120.0f;
        cameraRotation += cameraRotationVector * 120.0f;

        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -60, 60);

        // Meat Throw
        if(Input.GetMouseButtonDown(0))
        {
            if (meatsAmount <= 0) return;
            meatsAmount--;

            var meatOBJ = Instantiate(meat, cam.position, Quaternion.Euler(cameraRotation + playerRotation));

            meatOBJ.GetComponent<Rigidbody>().AddForce(cam.forward * meatThrowForce, ForceMode.Impulse);
            meatOBJ.GetComponent<Rigidbody>().AddTorque((Vector3.up + new Vector3(Random.Range(0,0.4f), Random.Range(0, 0.4f), Random.Range(0, 0.4f))) * meatThrowTorque, ForceMode.Impulse);

            GameManagerScript.Instance.LoseMeat();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("menu");
    }

    void LateUpdate()
    {
        transform.eulerAngles = playerRotation;
        cam.localEulerAngles = cameraRotation;
    }
}
