using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using TMPro;

[ExecuteAlways]

public class ProgrammManager : MonoBehaviour
{
    [Header("Put your Plane Marker here")]
    [SerializeField] private GameObject _planeMarkerPrefab;

    private ARRaycastManager _arRaycastManagerScript;

    private Vector2 _touchPosition = default;

    public GameObject objToSpawn;

    [Header("Put scrollView here")]
    public GameObject scrollView;

    public bool chooseObject = false;

    [Header("Put your AR Camera here")]
    [SerializeField] private Camera _ARCamera;

    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    private GameObject _selectedObject;

    public bool rotation;

    private Quaternion _YRotation;

    [Header("Put your object size TextMesh here")]
    public GameObject objectSizeText;

    private TextMeshProUGUI _sizeText;

    // Start is called before the first frame update
    void Start()
    {
        _arRaycastManagerScript = GetComponent<ARRaycastManager>();
        Assert.IsNotNull(_arRaycastManagerScript);

        Assert.IsNotNull(objectSizeText);
        _sizeText = objectSizeText.GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_sizeText);

        _planeMarkerPrefab.SetActive(false);
        scrollView.SetActive(false);
    }


    // Print real world size of the object
    void PrintObjectSize(GameObject _obj)
    {
        if (_obj != null)
        {
            // Print all local scale values and all lossyScale values with 2 decimal places
            _sizeText.text = "Local scale: " + _obj.transform.localScale.ToString("F2") +
                            System.Environment.NewLine +
                            "Lossy scale: " + _obj.transform.lossyScale.ToString("F2");

        }
        else
        {
            _sizeText.text = "Object is null";
        }
    }

    // Update is called once per frame
    void Update()
    {
        _arRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), _hits, TrackableType.Planes);

        if (chooseObject)
        {
            SetObject();
        }

        MoveAndRotateObject();

        ShowPlaneMarker();

        GameObject _obj = GameObject.FindWithTag("Unselected");
        if (_obj != null)
        {
            _obj = _obj.transform.Find("Top").gameObject;
            PrintObjectSize(_obj);
        }
    }

    // Show marker
    void ShowPlaneMarker()
    {
        if (_hits.Count > 0)
        {
            _planeMarkerPrefab.transform.position = _hits[0].pose.position;

            if (!_planeMarkerPrefab.activeInHierarchy)
                _planeMarkerPrefab.SetActive(true);
        }
    }


    void SetObject()
    {
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
            Instantiate(objToSpawn, _hits[0].pose.position, objToSpawn.transform.rotation);
            chooseObject = false;
            // _planeMarkerPrefab.SetActive(false);
        }
    }

    void MoveAndRotateObject()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            _touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = _ARCamera.ScreenPointToRay(_touchPosition);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider.CompareTag("Unselected"))
                    {
                        hitObject.collider.gameObject.tag = "Selected";
                    }
                    else
                    {
                        Debug.Log("No object found with tag 'Unselected'");
                    }
                }
            }

            _selectedObject = GameObject.FindWithTag("Selected");
            if (_selectedObject == null)
            {
                Debug.Log("No object found with tag 'Selected'");
                return;
            }

            if (touch.phase == TouchPhase.Moved && Input.touchCount == 1)
            {

                if (rotation)
                {
                    _YRotation = Quaternion.Euler(0f, -touch.deltaPosition.x * 0.1f, 0f);
                    _selectedObject.transform.rotation = _YRotation * _selectedObject.transform.rotation;
                }
                else
                {
                    _arRaycastManagerScript.Raycast(_touchPosition, _hits, TrackableType.Planes);
                    _selectedObject.transform.position = _hits[0].pose.position;
                }
            }
            else if (touch.phase == TouchPhase.Moved && Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                _selectedObject.transform.localScale += new Vector3(difference * 0.001f, difference * 0.001f, difference * 0.001f);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if (_selectedObject.CompareTag("Selected"))
                {
                    _selectedObject.tag = "Unselected";
                }

            }
        }
    }
}
