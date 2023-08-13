using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSupportLibrary;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    //State Controller
    internal StateMachine stateMachine { get; private set; }
    //Set in Editor
    public playerParameters parameters;
    public Animator visualAnimBase;

    //Colliders not ready ---
    [HideInInspector]
    public bool _active;
    void Activate() => _active = true;


    private int currentWeaponSelection;
    [SerializeField] private Transform visual;

    public Transform GetVisual
    {
        get => visual;
    }

    public void Awake()
    {
        Invoke(nameof(Activate), 1f); 
        stateMachine = new StateMachine();
        stateMachine.changeState(new MovementFree(this));
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {

    }



    private void OnDrawGizmos()
    {

    }
}

#region Controller Parameters
[System.Serializable]
public class playerParameters
{
    //Movement ---
    public Bounds _characterBounds;
    public LayerMask _groundLayer;
    public int _detectorCount = 3;
    public float _detectionRayLength = 0.1f;
    [Range(0.1f, 0.3f)] public float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground

    [Header("WALKING")] public float _acceleration = 90;
    public float _moveClamp = 13;
    public float _deAcceleration = 60f;
    public float _apexBonus = 2;

    [Header("GRAVITY")] public float _fallClamp = -40f;
    public float _minFallSpeed = 80f;
    public float _maxFallSpeed = 120f;

    [Header("JUMPING")] public float _jumpHeight = 30;
    public float _jumpApexThreshold = 10f;
    public float _coyoteTimeThreshold = 0.1f;
    public float _jumpBuffer = 0.1f;
    public float _jumpEndEarlyGravityModifier = 3;
}
#endregion

#region States
public class MovementFree : State, IPlayerController
{
    PlayerController controller;

    public Vector3 Velocity { get; private set; }
    public FrameInput Input { get; private set; }
    public bool JumpingThisFrame { get; private set; }
    public bool LandingThisFrame { get; private set; }
    public Vector3 RawMovement { get; private set; }
    public bool Grounded => _colDown;

    private Vector3 _lastPosition;
    private float _currentHorizontalSpeed, _currentVerticalSpeed;
    public MovementFree(PlayerController _controller)
    {
        controller = _controller;
    }

    public string stateName => "Grounded";

    public void onEnter()
    {
        Debug.Log("Is Grounded");
    }

    public void onExit()
    {

    }

    public void onUpdate()
    {
        if (!controller._active) return;
        // Calculate velocity
        Velocity = (controller.transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = controller.transform.position;

        GatherInput();
        RunCollisionChecks();

        CalculateWalk(); // Horizontal movement
        CalculateJumpApex(); // Affects fall speed, so calculate before gravity
        CalculateGravity(); // Vertical movement
        CalculateJump(); // Possibly overrides vertical

        MoveCharacter(); // Actually perform the axis movement
    }
    public void onFixedUpdate()
    {

    }


    //Go Here for Functionality ---
    #region Input
    private void GatherInput()
    {
        Input = new FrameInput
        {
            JumpDown = UnityEngine.Input.GetButtonDown("Jump"),
            JumpUp = UnityEngine.Input.GetButtonUp("Jump"),
            X = UnityEngine.Input.GetAxisRaw("Horizontal")
        };
        if (Input.JumpDown)
        {
            _lastJumpPressed = Time.time;
        }
    }
    #endregion
    #region Collisions
    private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
    private bool _colUp, _colRight, _colDown, _colLeft;

    private float _timeLeftGrounded;

    // We use these raycast checks for pre-collision information
    private void RunCollisionChecks()
    {
        // Generate ray ranges. 
        CalculateRayRanged();

        // Ground
        LandingThisFrame = false;
        var groundedCheck = RunDetection(_raysDown);
        if (_colDown && !groundedCheck) _timeLeftGrounded = Time.time; // Only trigger when first leaving
        else if (!_colDown && groundedCheck)
        {
            _coyoteUsable = true; // Only trigger when first touching
            LandingThisFrame = true;
        }

        _colDown = groundedCheck;

        // The rest
        _colUp = RunDetection(_raysUp);
        _colLeft = RunDetection(_raysLeft);
        _colRight = RunDetection(_raysRight);

        bool RunDetection(RayRange range)
        {
            return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, controller.parameters._detectionRayLength, controller.parameters._groundLayer));
        }
    }

    private void CalculateRayRanged()
    {
        // This is crying out for some kind of refactor. 
        var b = new Bounds(controller.transform.position, controller.parameters._characterBounds.size);

        _raysDown = new RayRange(b.min.x + controller.parameters._rayBuffer, b.min.y, b.max.x - controller.parameters._rayBuffer, b.min.y, Vector2.down);
        _raysUp = new RayRange(b.min.x + controller.parameters._rayBuffer, b.max.y, b.max.x - controller.parameters._rayBuffer, b.max.y, Vector2.up);
        _raysLeft = new RayRange(b.min.x, b.min.y + controller.parameters._rayBuffer, b.min.x, b.max.y - controller.parameters._rayBuffer, Vector2.left);
        _raysRight = new RayRange(b.max.x, b.min.y + controller.parameters._rayBuffer, b.max.x, b.max.y - controller.parameters._rayBuffer, Vector2.right);
    }


    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
    {
        for (var i = 0; i < controller.parameters._detectorCount; i++)
        {
            var t = (float)i / (controller.parameters._detectorCount - 1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    private void OnDrawGizmos()
    {
        // Bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controller.transform.position + controller.parameters._characterBounds.center, controller.parameters._characterBounds.size);

        // Rays
        if (!Application.isPlaying)
        {
            CalculateRayRanged();
            Gizmos.color = Color.blue;
            foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft })
            {
                foreach (var point in EvaluateRayPositions(range))
                {
                    Gizmos.DrawRay(point, range.Dir * controller.parameters._detectionRayLength);
                }
            }
        }

        if (!Application.isPlaying) return;

        // Draw the future position. Handy for visualizing gravity
        Gizmos.color = Color.red;
        var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
        Gizmos.DrawWireCube(controller.transform.position + move, controller.parameters._characterBounds.size);
    }

    #endregion
    #region Walk
    private void CalculateWalk()
    {
        if (Input.X != 0)
        {
            // Set horizontal move speed
            _currentHorizontalSpeed += Input.X * controller.parameters._acceleration * Time.deltaTime;

            // clamped by max frame movement
            _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, - controller.parameters._moveClamp, controller.parameters._moveClamp);

            // Apply bonus at the apex of a jump
            var apexBonus = Mathf.Sign(Input.X) * controller.parameters._apexBonus * _apexPoint;
            _currentHorizontalSpeed += apexBonus * Time.deltaTime;
        }
        else
        {
            // No input. Let's slow the character down
            _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, controller.parameters._deAcceleration * Time.deltaTime);
        }

        if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft)
        {
            // Don't walk through walls
            _currentHorizontalSpeed = 0;
        }
    }

    #endregion
    #region Gravity
    private float _fallSpeed;

    private void CalculateGravity()
    {
        if (_colDown)
        {
            // Move out of the ground
            if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
        }
        else
        {
            // Add downward force while ascending if we ended the jump early
            var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * controller.parameters._jumpEndEarlyGravityModifier : _fallSpeed;

            // Fall
            _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

            // Clamp
            if (_currentVerticalSpeed < controller.parameters._fallClamp) _currentVerticalSpeed = controller.parameters._fallClamp;
        }
    }

    #endregion
    #region Jump
    private bool _coyoteUsable;
    private bool _endedJumpEarly = true;
    private float _apexPoint; // Becomes 1 at the apex of a jump
    private float _lastJumpPressed;
    private int jumpCounter = 0;

    private bool CanUseCoyote => _coyoteUsable && !_colDown && _timeLeftGrounded + controller.parameters._coyoteTimeThreshold > Time.time;
    private bool HasBufferedJump => _colDown && _lastJumpPressed + controller.parameters._jumpBuffer > Time.time;

    private void CalculateJumpApex()
    {
        if (!_colDown)
        {
            // Gets stronger the closer to the top of the jump
            _apexPoint = Mathf.InverseLerp(controller.parameters._jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
            _fallSpeed = Mathf.Lerp(controller.parameters._minFallSpeed, controller.parameters._maxFallSpeed, _apexPoint);
        }
        else
        {
            _apexPoint = 0;
        }
    }

    private void CalculateJump()
    {
        // Jump if: grounded or within coyote threshold || sufficient jump buffer
        if (Input.JumpDown && CanUseCoyote || HasBufferedJump || Input.JumpDown && jumpCounter < 1)
        {
            _currentVerticalSpeed = controller.parameters._jumpHeight;
            _endedJumpEarly = false;
            _coyoteUsable = false;
            _timeLeftGrounded = float.MinValue;
            JumpingThisFrame = true;
            jumpCounter++;
        }
        else
        {
            JumpingThisFrame = false;
        }

        // End the jump early if button released
        if (!_colDown && Input.JumpUp && !_endedJumpEarly && Velocity.y > 0)
        {
            // _currentVerticalSpeed = 0;
            _endedJumpEarly = true;
        }

        if (_colUp)
        {
            if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
        }

        if (_colDown)
        {
            jumpCounter = 0;
        }
    }

    #endregion
    #region Move

    [Header("MOVE")]
    [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
    private int _freeColliderIterations = 10;

    // We cast our bounds before moving to avoid future collisions
    private void MoveCharacter()
    {
        var pos = controller.transform.position;
        RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
        var move = RawMovement * Time.deltaTime;
        var furthestPoint = pos + move;

        // check furthest movement. If nothing hit, move and don't do extra checks
        var hit = Physics2D.OverlapBox(furthestPoint, controller.parameters._characterBounds.size, 0, controller.parameters._groundLayer);
        if (!hit)
        {
            controller.transform.position += move;
            return;
        }

        // otherwise increment away from current pos; see what closest position we can move to
        var positionToMoveTo = controller.transform.position;
        for (int i = 1; i < _freeColliderIterations; i++)
        {
            // increment to check all but furthestPoint - we did that already
            var t = (float)i / _freeColliderIterations;
            var posToTry = Vector2.Lerp(pos, furthestPoint, t);

            if (Physics2D.OverlapBox(posToTry, controller.parameters._characterBounds.size, 0, controller.parameters._groundLayer))
            {
               controller.transform.position = positionToMoveTo;

                // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                if (i == 1)
                {
                    if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                    var dir = controller.transform.position - hit.transform.position;
                    controller.transform.position += dir.normalized * move.magnitude;
                }

                return;
            }

            positionToMoveTo = posToTry;
        }
    }

    #endregion
}
#endregion


