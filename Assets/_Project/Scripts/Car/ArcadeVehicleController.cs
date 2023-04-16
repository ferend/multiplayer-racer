using Toolkit;
using UnityEngine;

namespace _Project.Scripts.Car
{
    public class ArcadeVehicleController : MonoBehaviour
    {

        private void OnValidate() => CarIndicator.allCarControllers.Add(this);
        
        public enum GroundCheck { RayCast, SphereCast };

        [SerializeField] private enum MovementMode { Velocity, AngularVelocity };
        [SerializeField] private MovementMode movementMode;
        public GroundCheck groundCheck;
        public LayerMask drivableSurface;

        [SerializeField] private float maxSpeed, acceleration, turn, gravity = 7f;
        public Rigidbody rb, carBody;
    
        [HideInInspector] private RaycastHit hit;
        [SerializeField] private AnimationCurve frictionCurve;
        [SerializeField] private AnimationCurve turnCurve;
        [SerializeField] private PhysicMaterial frictionMaterial;
       
        [Header("Visuals")]
        [SerializeField] private Transform bodyMesh;
        [SerializeField] private Transform[] frontWheels = new Transform[2];
        [SerializeField] private Transform[] rearWheels = new Transform[2];
        [HideInInspector]
        public Vector3 carVelocity;
    
        [Range(0,10)] [SerializeField] private float bodyTilt;
        
        [Header("Audio settings")]
        [SerializeField] private AudioSource engineSound;
        [Range(0, 1)]
        [SerializeField] private float minPitch;
        [Range(1, 3)]
        [SerializeField] private float maxPitch;
        [SerializeField] private AudioSource skidSound;

        [HideInInspector]
        public float skidWidth;
        
        private float radius, horizontalInput, verticalInput;
        private Vector3 origin;

        private void Start()
        {
            radius = rb.GetComponent<SphereCollider>().radius;
            if (movementMode == MovementMode.AngularVelocity)
            {
                Physics.defaultMaxAngularSpeed = 100;
            }
        }
        private void Update()
        {
            horizontalInput = UltimateJoystick.GetHorizontalAxis("Movement"); //turning input
            verticalInput = UltimateJoystick.GetVerticalAxis("Movement");     //accelaration input
            Visuals();
            AudioManager();
        }




        void FixedUpdate()
        {
            carVelocity = carBody.transform.InverseTransformDirection(carBody.velocity);
        
            if (Mathf.Abs(carVelocity.x) > 0)
            {
                //changes friction according to sideways speed of car
                frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(carVelocity.x/100)); 
            }
        
        
            if (grounded())
            {
                //turnlogic
                float sign = Mathf.Sign(carVelocity.z);
                float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude/ maxSpeed);
                if (verticalInput > 0.1f || carVelocity.z >1)
                {
                    carBody.AddTorque(Vector3.up * horizontalInput * sign * turn*100* TurnMultiplyer);
                }
                else if (verticalInput < -0.1f || carVelocity.z < -1)
                {
                    carBody.AddTorque(Vector3.up * horizontalInput * sign * turn*100* TurnMultiplyer);
                }

                //brakelogic
                rb.constraints = Input.GetAxis("Jump") > 0.1f ? RigidbodyConstraints.FreezeRotationX : RigidbodyConstraints.None;


                switch (movementMode)
                {
                    case MovementMode.AngularVelocity:
                    {
                        if (Mathf.Abs(verticalInput) > 0.1f)
                        {
                            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * verticalInput * maxSpeed/radius, acceleration * Time.deltaTime);
                        }

                        break;
                    }
                    case MovementMode.Velocity:
                    {
                        if (Mathf.Abs(verticalInput) > 0.1f && Input.GetAxis("Jump")<0.1f)
                        {
                            rb.velocity = Vector3.Lerp(rb.velocity, carBody.transform.forward * verticalInput * maxSpeed, acceleration/10 * Time.deltaTime);
                        }

                        break;
                    }
                }

                //body tilt
                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, hit.normal) * carBody.transform.rotation, 0.12f));
            }
            else
            {
                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, Vector3.up) * carBody.transform.rotation, 0.02f));
                rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity + Vector3.down* gravity, Time.deltaTime * gravity);
            }

        }
        
        private void AudioManager()
        {
            engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(carVelocity.z) / maxSpeed);
            if (Mathf.Abs(carVelocity.x) > 10 && grounded())
            {
                skidSound.mute = false;
            }
            else
            {
                skidSound.mute = true;
            }
        }

        private void Visuals()
        {
            //tires
            foreach (Transform FW in frontWheels)
            {
                FW.localRotation = Quaternion.Slerp(FW.localRotation, Quaternion.Euler(FW.localRotation.eulerAngles.x,
                    30 * horizontalInput, FW.localRotation.eulerAngles.z), 0.1f);
                FW.GetChild(0).localRotation = rb.transform.localRotation;
            }
            rearWheels[0].localRotation = rb.transform.localRotation;
            rearWheels[1].localRotation = rb.transform.localRotation;

            //Body
            if(carVelocity.z > 1)
            {
                bodyMesh.localRotation = Quaternion.Slerp(bodyMesh.localRotation, Quaternion.Euler(Mathf.Lerp(0, -5, carVelocity.z / maxSpeed),
                    bodyMesh.localRotation.eulerAngles.y, bodyTilt * horizontalInput), 0.05f);
            }
            else
            {
                bodyMesh.localRotation = Quaternion.Slerp(bodyMesh.localRotation, Quaternion.Euler(0,0,0) , 0.05f);
            }
        

        }

        public bool grounded() //checks for if vehicle is grounded or not
        {
            origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
            var direction = -transform.up;
            var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;

            if (groundCheck == GroundCheck.RayCast)
            {
                if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if(groundCheck == GroundCheck.SphereCast)
            {
                if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface))
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            else { return false; }
        }

    }
}
