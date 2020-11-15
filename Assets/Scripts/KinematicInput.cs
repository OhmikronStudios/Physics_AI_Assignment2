using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KinematicInput : MonoBehaviour
{
    //Player Health
    public int playerHealth = 100;
    [SerializeField] Text healthText;

    //Camera Movement links
    [SerializeField] Transform playerInputSpace = default;

    // Horizontal movement parameters
    public Vector3 speed;
    public float maxSpeed = 10.0f;
    
    // Jump and Fall parameters
    public float maxJumpSpeed = 1.5f;
    public float maxDoubleJumpSpeed = 1.2f;
    public float maxFallSpeed = -2.2f;
    public float timeToMaxJumpSpeed = 0.2f;
    public float deccelerationDuration = 0.0f;
    public float maxJumpDuration = 1.2f;
    public float hoverTime = 0.5f;
    public float maxHeightReached = 0.0f;
    public float fallDamageThreshold = 3.0f;
    
    // Jump and Fall helpers
    bool jumpStartRequest = false;
    bool jumpRelease = false;
    bool isMovingUp = false;
    bool isFalling = false;
    bool secondJump = false;
    bool aboutToCollide = false;
    float currentJumpDuration = 0.0f;
    float gravityAcceleration = -9.8f;

    public float groundSearchLength = 0.6f;
    public float collisionCheckDistance = 0.2f;
    RaycastHit currentGroundHit;
    

    // Rotation Parameters
    float angleDifferenceForward = 0.0f;

    // Components and helpers
    Rigidbody rigidBody;
    Vector2 playerInput;
    Vector3 playerSize;

    // Debug configuration
    public GUIStyle myGUIStyle;
    
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerSize = GetComponent<Collider>().bounds.size;
    }

    void Start()
    {
        jumpStartRequest = false;
        jumpRelease = false;
        isMovingUp = false;
        isFalling = false;
        secondJump = false;
        playerHealth = 100;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        playerInput = new Vector2();
        playerInput.x = horizontal;
        playerInput.y = vertical;

        if (playerInputSpace)
        {
            Vector3 forward = playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = playerInputSpace.right;
            right.y = 0f;
            right.Normalize();
            speed = (forward * playerInput.y + right * playerInput.x) * maxSpeed;
        }
        else
        {
            speed = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        }


        if (Input.GetButtonDown("Jump"))
        {
            jumpStartRequest = true;
            jumpRelease = false;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpRelease = true;
            jumpStartRequest = false;
        }
        healthText.text = playerHealth.ToString();

        RaycastHit hit;
        if (playerInput.x >= 0.1f || playerInput.y >= 0.1f || playerInput.x <= -0.1f || playerInput.y <= -0.1f)
        {
            if (rigidBody.SweepTest(rigidBody.velocity, out hit, collisionCheckDistance))
            {
                if (hit.collider.gameObject.tag == "Solid")
                {
                    aboutToCollide = true;
                    Debug.Log("hit a Solid");
                    maxSpeed = 0.0f;

                }
            }
        }
        else
        {
            aboutToCollide = false;
            maxSpeed = 10.0f;

        }
        
        if (isOnGround() && currentGroundHit.collider.gameObject.tag == "TeeterTotter")
        {
               currentGroundHit.collider.gameObject.GetComponent<TeeterPlatform>().Rotate(transform.position);
        }

    }

    void StartFalling()
    {
        isMovingUp = false;
        isFalling = true;
        currentJumpDuration = 0.0f;
        jumpRelease = false;
    }

    void FixedUpdate()
    {
        // Calculate horizontal movement
        Vector3 movement = Vector3.right * playerInput.x * maxSpeed * Time.deltaTime;
        movement += Vector3.forward * playerInput.y * maxSpeed * Time.deltaTime;
        movement.y = 0.0f;
        Vector3 targetPosition = rigidBody.position;

        if (aboutToCollide)
        { targetPosition = rigidBody.position; }
        else
        { targetPosition = rigidBody.position + movement; }


        // Calculate Vertical movement
        float targetHeight = 0.0f;

        if (!isMovingUp && jumpStartRequest && isOnGround())
        {
            isMovingUp = true;
            jumpStartRequest = false;
            currentJumpDuration = 0.0f;
        }

        if (isMovingUp)
        {
            if (jumpRelease)
            {
                StartFalling();
            }
            else if (currentJumpDuration >= maxJumpDuration)
            {
                StartCoroutine(Hover());
            }
            else
            {
                float jumpSpeed;
                if (!secondJump)
                {
                    jumpSpeed = maxJumpSpeed;
                }
                else
                {
                    jumpSpeed = maxDoubleJumpSpeed;
                }
                float currentYpos = rigidBody.position.y;
                float newVerticalVelocity = jumpSpeed + gravityAcceleration * Time.deltaTime;
                targetHeight = currentYpos + (newVerticalVelocity * Time.deltaTime) + (0.5f * jumpSpeed * Time.deltaTime * Time.deltaTime);
                if (currentYpos > maxHeightReached)
                { maxHeightReached = currentYpos; }
                currentJumpDuration += Time.deltaTime;
            }
        }
        else if (!isOnGround())
        {
            StartFalling();
        }

        if (isFalling)
        {
            if (jumpStartRequest && !secondJump)
            {
                isFalling = false;
                secondJump = true;
                isMovingUp = true;
                jumpStartRequest = false;
                currentJumpDuration = 0.0f;
            }
            else if (isOnGround())
            {
                // End of falling state. No more height adjustments required, just snap to the new ground position
                isFalling = false;
                secondJump = false;
                targetHeight = currentGroundHit.point.y + (0.5f * playerSize.y);
                if (currentGroundHit.collider.gameObject.tag == "TeeterTotter")
                {
                    currentGroundHit.collider.gameObject.GetComponent<TeeterPlatform>().Rotate(transform.position);
                }

                if (maxHeightReached - targetHeight > fallDamageThreshold)
                {
                    HealthChange((int)(5 * (maxHeightReached - targetHeight)));
                    
                }
                maxHeightReached = 0.0f;
            }
            else
            {
                float currentYpos = rigidBody.position.y;
                float currentYvelocity = rigidBody.velocity.y;

                float newVerticalVelocity = maxFallSpeed + gravityAcceleration * Time.deltaTime;
                targetHeight = currentYpos + (newVerticalVelocity * Time.deltaTime) + (0.5f * maxFallSpeed * Time.deltaTime * Time.deltaTime);
            }

        }

        

        if ( targetHeight > Mathf.Epsilon)
        {
            // Only required if we actually need to adjust height
            targetPosition.y = targetHeight;
        }

        // Calculate new desired rotation
        Vector3 movementDirection = targetPosition - rigidBody.position;
        movementDirection.y = 0.0f;
        movementDirection.Normalize();

        Vector3 currentFacingXZ = transform.forward;
        currentFacingXZ.y = 0.0f;

        angleDifferenceForward = Vector3.SignedAngle(movementDirection, currentFacingXZ, Vector3.up);
        Vector3 targetAngularVelocity = Vector3.zero;
        targetAngularVelocity.y = angleDifferenceForward * Mathf.Deg2Rad;

        Quaternion syncRotation = Quaternion.identity;
        syncRotation = Quaternion.LookRotation(movementDirection);

        //Debug.DrawLine(rigidBody.position, rigidBody.position + movementDirection * 2.0f, Color.green, 0.0f, false);
        //Debug.DrawLine(rigidBody.position, rigidBody.position + currentFacingXZ * 2.0f, Color.red, 0.0f, false);

        //if (isColliding() && collidingObject.collider.gameObject.tag == "Solid")
        //{
        //    rigidBody.velocity = Vector3.zero;
        //    targetPosition = rigidBody.position;
        //    Debug.Log("collided with solid");
        //}
    
        // Finally, update RigidBody    
        
        rigidBody.MovePosition(targetPosition);
        
        

        if (movement.sqrMagnitude > Mathf.Epsilon )
        {
            
            // Currently we only update the facing of the character if there's been any movement
            rigidBody.MoveRotation(syncRotation);
        }
    }

    private bool isOnGround()
    {
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y - groundSearchLength, lineStart.z);

        Debug.DrawLine(lineStart, vectorToSearch);

        return Physics.Linecast(lineStart, vectorToSearch, out currentGroundHit);
    }


    

    void OnGUI()
    {
        // Add here any debug text that might be helpful for you
        GUI.Label(new Rect(10, 10, 100, 20), "Angle " + angleDifferenceForward.ToString(), myGUIStyle);
    }

    private void OnDrawGizmos()
    {
        // Debug Draw last ground collision, helps visualize errors when landing from a jump
        if (currentGroundHit.collider != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(currentGroundHit.point, 0.25f);
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        // Debug-draw all contact points and normals, helps visualize collisions when the physics of the RigidBody are enabled (when is NOT Kinematic)
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finish")
        {
            FindObjectOfType<GameManager>().GameOver();
        }    

    }

    IEnumerator Hover()
    {
        float currentYpos = rigidBody.position.y;
        yield return new WaitForSeconds(hoverTime);
        StartFalling();
    }

    public void HealthChange(int healthDelta)
    {
        playerHealth = playerHealth - healthDelta;
        if (playerHealth <= 0)
            { FindObjectOfType<GameManager>().GameOver(); }
    }
   


}

