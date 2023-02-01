using UnityEngine;
using UnityEngine.Assertions;

[ExecuteAlways]
public class BoxBaseScript : MonoBehaviour
{
    private BoxCollider _collider;

    private Vector3 _currentSize;
    public delegate void SizeChangedDelegate(Vector3 from, Vector3 to);
    public static event SizeChangedDelegate onSizeChanged;


    void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(_collider);
        _currentSize = _collider.size;

        Assert.IsNotNull(onSizeChanged);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newSize = _collider.size;

        // Depending on you required precision
        // Vector3 == has a precision of 0.00001
        if (_currentSize != newSize)
        {
            // Debug.Log("Size changed from " + _currentSize + " to " + newSize);
            // If for some reason you need it more exact
            //if(Vector3.Distance(_currentLocalScale, newLocalScale) <= Mathf.Epsilon)
            {
                onSizeChanged(_currentSize, newSize);

                _currentSize = newSize;
            }
        }

    }
}
