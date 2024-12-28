using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Debug = UnityEngine.Debug;

public class BallControl : MonoBehaviour
{
    public Rigidbody RBball;
    public Transform BallPlace;
    public float throwForceMultiplier = 0.1f;
    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    private bool isThrown = false;

    private float timeInterval = 2f;
    private float timeCounter = 0f;
    private bool shouldDrawLine = false;
    private Vector3 startLineWorldPosition;
    private Vector3 endLineWorldPosition;

    [Header("Win explosion")]
    private ParticleSystem[] particlesystem;

    private Transform orginalParent;
    public Camera Arcam;
    public Vector3 ballOffset;

    //Observer
    public static event Action onBallThrown;

    private void Start()
    {
        Arcam = Camera.main;
    }
    private void OnEnable()
    {
        RespawnBall();
        orginalParent = transform.parent;
    }
    private void Update()
    {
        if (isThrown)
        {
            timeCounter += Time.deltaTime;
            if(RBball.velocity.magnitude<0.1f|| timeCounter >= timeInterval)
            {
                RespawnBall();
                isThrown = false;
            }
        }
        else
        {
            HandleInput();
        }
        if (shouldDrawLine)
        {
            Debug.DrawLine(startLineWorldPosition, endLineWorldPosition, Color.red);
        }
    }
    void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase==TouchPhase.Began)
            {
                touchStartPosition = touch.position;
                shouldDrawLine = false;
            }else if(touch.phase==TouchPhase.Ended)
            {
                touchEndPosition = touch.position;
                ProcessThrow();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            touchStartPosition = Input.mousePosition;
            shouldDrawLine=false;
        }else if (Input.GetMouseButtonUp(0))
        {
            touchEndPosition= Input.mousePosition;
            ProcessThrow();
        }
    }
    void ProcessThrow()
    {
        Vector2 direction = touchEndPosition - touchStartPosition;
        float magn = direction.magnitude;

        Debug.Log("Direction:" + direction + "Magnitude" + magn);

        ThrowBallInDirection(direction, magn);
        isThrown = true;

        Vector3 startScreenPosition = new Vector3(touchStartPosition.x, touchStartPosition.y, Arcam.nearClipPlane + 1f);
        Vector3 endScreenPosition = new Vector3(touchEndPosition.x,touchEndPosition.y,Arcam.farClipPlane + 1f);
            startLineWorldPosition=Arcam.ScreenToWorldPoint(startScreenPosition);
            endLineWorldPosition = Arcam.ScreenToWorldPoint(endScreenPosition);
            shouldDrawLine=true;
        Debug.Log("Start Line Position: " + startLineWorldPosition + " End Line Position: " + endLineWorldPosition);
    }
    
    void ThrowBallInDirection(Vector2 dire,float mag)
    {
        // Detach from parent (AR camera) before throwing
        transform.SetParent(null);

        // Normalize the screen direction vector and create a 3D direction vector
        Vector3 screenDirection = new Vector3(dire.x, dire.y, 0).normalized;

        Vector3 forwardDirection = Arcam.transform.forward;
        Vector3 rightDirection = Arcam.transform.right;
        Vector3 upDirection = Arcam.transform.up;

        Vector3 throwDirection = (forwardDirection*screenDirection.y+rightDirection*screenDirection.x+upDirection*0.7f).normalized;

        Vector3 force = throwDirection * (mag / 100f) * throwForceMultiplier;

        RBball.isKinematic = false;
        RBball.useGravity = true;
        RBball.AddForce(force, ForceMode.Impulse);

        //Rais ball throw event
        Debug.Log("Force applied: " + force);
    }
    void RespawnBall()
    {
        timeCounter = 0;
        RBball.velocity = Vector3.zero;
        RBball.angularVelocity = Vector3.zero;
        RBball.useGravity = false;
        RBball.isKinematic = true;
        // Reattach to the original parent (AR camera) when respawning
        transform.SetParent(Arcam.transform);

        Vector3 respawnPosition = BallPlace.position + Arcam.transform.TransformDirection(ballOffset);

        transform.position = respawnPosition;
        onBallThrown?.Invoke();
        Debug.Log("Ball respawned at: " + BallPlace.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hoop"))
        {
            RespawnBall();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("HoopBar"))
        {
            AudioManager.PlayAudio(global::audio.BallHoophit);
            Debug.Log("Playing HoopHit");
        }
        else
        {
            AudioManager.PlayAudio(global::audio.BallGroundPich);
        }
    }

}
