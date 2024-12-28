using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using Unity.VisualScripting;

public class PlaceHoop : MonoBehaviour
{

    public GameObject WinParticle;
    public Camera arCamera;
    [SerializeField]
    [Tooltip("Instantiate  this prefab on a place at the touch location")]
    GameObject m_HoopPrefab;
    
    [SerializeField]
    private GameObject BasketBall;

    public GameObject placedPrefab
    {
        get {return m_HoopPrefab;}
        set {m_HoopPrefab = value;}
    }
    public GameObject spawnedHoop { get; private set; }

    public static event Action onPlacedObject;

    private bool isPlaced = false;
    public Vector3 HoopOffset;

    [SerializeField]
    ARRaycastManager m_RaycastManager;

    static List<ARRaycastHit>s_hits = new List<ARRaycastHit>();

    private void Awake()
    {
        m_RaycastManager = GetComponentInParent<ARRaycastManager>();
    }
    private void Update()
    {
        BasketBall.SetActive(isPlaced);
        if (isPlaced) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (m_RaycastManager.Raycast(touch.position, s_hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitpose=s_hits[0].pose;

                    spawnedHoop = Instantiate(m_HoopPrefab, hitpose.position+HoopOffset, Quaternion.identity);
                    spawnedHoop.transform.parent = transform.parent;

                    Vector3 directionToCamera = arCamera.transform.position-spawnedHoop.transform.position;
                    directionToCamera.y = 0;
                    Quaternion rotationToCmaera = Quaternion.LookRotation(directionToCamera);
                    spawnedHoop.transform.rotation = rotationToCmaera;

                    isPlaced = true;

                    if(onPlacedObject != null)
                    {
                        onPlacedObject();
                    }
                }
            }
        }
    }
    public void RemoveHoop()
    {
        isPlaced=false;
        Destroy(spawnedHoop);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            WinParticle.SetActive(true);
            StartCoroutine(StopParticletime());
        }
    }
    IEnumerator StopParticletime()
    {
        WinParticle.SetActive(false);
        yield return new WaitForSeconds(2f);
    }
}
