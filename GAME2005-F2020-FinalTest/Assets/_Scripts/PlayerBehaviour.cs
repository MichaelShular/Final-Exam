using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public Transform bulletSpawn;
    public GameObject bullet;
    public int fireRate;


    public BulletManager bulletManager;

    [Header("Movement")]
    public float speed;
    public bool isGrounded;
    public RigidBody3D body;
    public CubeBehaviour cube;
    public Camera playerCam;

    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("Menu");
        }
        if (Time.timeScale == 1)
        { 
            _Fire();
            _Move();
        }
    }

    private void _Move()
    {
        if (isGrounded)
        {
            if (Input.GetAxisRaw("Horizontal") > 0.0f)
            {
                // move right
                body.velocity = playerCam.transform.right * speed * Time.deltaTime;
            }

            if (Input.GetAxisRaw("Horizontal") < 0.0f)
            {
                // move left
                body.velocity = -playerCam.transform.right * speed * Time.deltaTime;
            }

            if (Input.GetAxisRaw("Vertical") > 0.0f)
            {
                // move forward
                body.velocity = playerCam.transform.forward * speed * Time.deltaTime;
            }

            if (Input.GetAxisRaw("Vertical") < 0.0f) 
            {
                // move Back
                body.velocity = -playerCam.transform.forward * speed * Time.deltaTime;
            }

            body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, 0.9f);
            body.velocity = new Vector3(body.velocity.x, 0.0f, body.velocity.z); // remove y
            

            if (Input.GetAxisRaw("Jump") > 0.0f)
            {
                body.velocity = transform.up * speed * 0.07f * Time.deltaTime;
            }

            transform.position += body.velocity;

        }
        else if(body.velocity.y < 0 || body.velocity.y > 0)
        {
            if (Input.GetAxisRaw("Vertical") > 0.0f)
            {
                // move forward
                body.velocity.z = 0.02f * playerCam.transform.forward.z; 
            }
            if (Input.GetAxisRaw("Vertical") < 0.0f)
            {
                // move forward
                body.velocity.z = -0.02f * playerCam.transform.forward.z;
            }
            if (Input.GetAxisRaw("Vertical") > 0.0f)
            {
                // move left
                body.velocity.x = -0.02f * playerCam.transform.right.z;
            }
            if (Input.GetAxisRaw("Vertical") < 0.0f)
            {
                // move left
                body.velocity.x = 0.02f * playerCam.transform.right.z;
            }
            
            // transform.position += body.velocity;
        }
       
    }


    private void _Fire()
    {
        if (Input.GetAxisRaw("Fire1") > 0.0f)
        {
            // delays firing
            if (Time.frameCount % fireRate == 0)
            {

                var tempBullet = bulletManager.GetBullet(bulletSpawn.position, bulletSpawn.forward);
                tempBullet.transform.SetParent(bulletManager.gameObject.transform);
            }
        }
    }

    void FixedUpdate()
    {
        GroundCheck();
    }

    private void GroundCheck()
    {
        isGrounded = cube.isGrounded;
    }

}


