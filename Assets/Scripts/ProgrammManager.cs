using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class ProgrammManager : MonoBehaviour
{
    [Header("Put your Plane Marker here")]
    [SerializeField] private GameObject _planeMarkerPrefab;

    private ARRaycastManager _arRaycastManagerScript;

    private Vector2 _touchPosition = default;

    public GameObject ObjToSpawn;

    [Header("Put ScrollView here")]
    public GameObject ScrollView;

    public bool ChooseObject = false;

    [Header("Put your AR Camera here")]
    [SerializeField] private Camera ARCamera;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private GameObject SelectedObject;

    // Start is called before the first frame update
    void Start()
    {
        _arRaycastManagerScript = GetComponent<ARRaycastManager>();
        Assert.IsNotNull(_arRaycastManagerScript);

        _planeMarkerPrefab.SetActive(false);
        ScrollView.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (ChooseObject)
        {
            ShowPlaneMarkerAndSetObject();
        }

        MoveObject();

    }


    // Always positioned at the center of the screen
    void ShowPlaneMarkerAndSetObject()
    {
        _arRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        // Show marker
        if (hits.Count > 0)
        {
            _planeMarkerPrefab.transform.position = hits[0].pose.position;
            _planeMarkerPrefab.SetActive(true);
        }

        // Set object on marker
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            // Check if finger is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Debug.Log("Touched the UI");
                return;
            }
            // Instantiate your object
            Instantiate(ObjToSpawn, hits[0].pose.position, ObjToSpawn.transform.rotation);
            ChooseObject = false;
            _planeMarkerPrefab.SetActive(false);
        }
    }

    void MoveObject()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            _touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = ARCamera.ScreenPointToRay(_touchPosition);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider.CompareTag("Unselected"))
                    {
                        hitObject.collider.gameObject.tag = "Selected";
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                _arRaycastManagerScript.Raycast(_touchPosition, hits, TrackableType.Planes);
                SelectedObject = GameObject.FindWithTag("Selected");

                if (SelectedObject != null)
                {
                    SelectedObject.transform.position = hits[0].pose.position;
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if(SelectedObject.CompareTag("Selected"))
                {
                    SelectedObject.tag = "Unselected";
                }
            }
        }
    }
}
