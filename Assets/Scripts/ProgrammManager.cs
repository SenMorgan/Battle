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

    // Start is called before the first frame update
    void Start()
    {
        _arRaycastManagerScript = GetComponent<ARRaycastManager>();
        Assert.IsNotNull(_arRaycastManagerScript);

        _planeMarkerPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ShowPlaneMarkerAndSetObject();
    }


    // Always positioned at the center of the screen
    void ShowPlaneMarkerAndSetObject()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

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
        }
    }
}
