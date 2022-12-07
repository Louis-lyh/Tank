using UnityEngine;


public class Character : MonoBehaviour
{
    [SerializeField]
    private Map map;

    [SerializeField]
    private float maxVelocity;
    
    [SerializeField]
    private float acceleration;

    private Vector3 currentVelocity;


    private Rigidbody rig;
    

    // Start is called before the first frame update
    void Start()
    {
        currentVelocity = Vector3.zero;
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    private Vector3 currentF;

    void Move()
    {
        currentF = map.GetVector(transform.position)*acceleration;
        float dt = Time.deltaTime;
        
        

        
        currentF=map.GetVector(rig.position)*acceleration;
        currentVelocity += currentF * dt;
        if (currentVelocity.sqrMagnitude > maxVelocity * maxVelocity)
        {
            currentVelocity = currentVelocity.normalized * maxVelocity;
        }
        rig.velocity = currentVelocity;
        transform.forward = currentVelocity;
       
       

    }

    private void OnDrawGizmos()
    {
        
        if (Application.isPlaying)
        {
            Gizmos.color=Color.blue;
            DrawArrow.ForGizmo(transform.position,transform.forward );
            
            Gizmos.color=Color.red;
            DrawArrow.ForGizmo(transform.position, currentF.normalized );
        }
    }
}
