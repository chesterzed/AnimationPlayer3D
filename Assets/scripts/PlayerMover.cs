using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class PlayerMover : MonoBehaviour
{
	public Enemy EnemyObj;

	[SerializeField] private TMP_Text _text;

	[SerializeField] private Camera _camera;
	[SerializeField] private Transform _target;
	[SerializeField] private float _jumpSpeed;
	[SerializeField] private float _moveSpeed;
	[SerializeField] private float _rotationSpeed;
	[SerializeField] private float _enemyDistance;
	[SerializeField] private float _killEnemyDistance;
	
	[SerializeField] private GameObject[] _gunPool;
	private Attach_R _gunPos;
	private GameObject _currentGun;

	private Animator _animator;
	private PlayerInput _playerInput;

	private Vector2 _direction;
	private Vector2 _mousePosition;
	private Vector3 _KillEnemyPos;

	private Transform _chest;
	private bool isAttack = false;

	private float _timer = 0;


	private void Awake()
	{
		_playerInput = new PlayerInput();

		_playerInput.Player.Finnish.performed += ctx => OnFinnish();
	}

	private void OnEnable()
	{
		_playerInput.Enable();
		Debug.Log(_gunPool[1].name);
	}

	private void OnDisable()
	{
		_playerInput.Disable();
	}

	private void Start()
	{
		_gunPos = GetComponentInChildren<Attach_R>();
		EnemyObj = FindObjectOfType<Enemy>();
		_animator = GetComponent<Animator>();
		_chest = GetComponentInChildren<ChestRotate>().transform;
		
		SwitchGun(1);
	}

	private void Update()
	{
		_direction = _playerInput.Player.Move.ReadValue<Vector2>();

		
		if (isAttack || _animator.GetCurrentAnimatorClipInfo(_animator.GetLayerIndex("BaseLayer"))[0].clip.name == "Finishing")
		{
			if ((_KillEnemyPos - transform.position).magnitude >= 0.005f)
			{
				float scaledJumpSpeed = _jumpSpeed * Time.deltaTime;
				transform.position = Vector3.Lerp(transform.position, _KillEnemyPos, scaledJumpSpeed);
				if ((_KillEnemyPos - transform.position).magnitude <= 0.005f)
				{
					transform.position = _KillEnemyPos;
					_animator.SetTrigger("Attack");
				}
			}
			else
			{
				_timer += Time.deltaTime;
			}       
			if (_timer >= 1.8f)
			{
				SwitchGun(1);
				transform.rotation = new Quaternion(0, 0, 0, 0);
				EnemyObj.GetComponent<Animator>().enabled = false;
				EnemyObj.isDead = true;
				isAttack = false;

				_timer = 0;
				_playerInput.Enable();
			}
		}
		else
		{
			_timer = 0;
			Move(_direction);
			EnemyChecker();
		}
	}

	private void LateUpdate()
	{
		_mousePosition = _playerInput.Player.Rotate.ReadValue<Vector2>();

		if (!isAttack)
			Look(_mousePosition);
	}

	private void Move(Vector2 direction)
	{
		float scaledMoveSpeed = _moveSpeed * Time.deltaTime;
		transform.position += new Vector3(direction.x, 0, direction.y) * scaledMoveSpeed;

		_animator.SetFloat("SpeedX", direction.x);
		_animator.SetFloat("SpeedY", direction.y);
		_animator.SetFloat("Blend", new Vector3(direction.x, 0, direction.y).magnitude);
	}

	private void Look(Vector2 mousePosition)
	{
		float scaledRotateSpeed = _rotationSpeed * Time.deltaTime;

		Ray ray = _camera.ScreenPointToRay(mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			_target.position = Vector3.Lerp(_target.position, hit.point, scaledRotateSpeed);
			
			if ((hit.point - _target.position).magnitude < 0.1f)
				_target.position = hit.point;
		} 
		if (_animator.GetBool("isReady"))
		{
			_target.position = EnemyObj.transform.position;
		}

		_chest.localRotation = Quaternion.AngleAxis(Mathf.Atan2(_target.position.x - transform.position.x, _target.position.z - transform.position.z) * Mathf.Rad2Deg, -Vector3.right);
	}

	private void OnFinnish()
	{
		if (_animator.GetBool("isReady"))
		{
			_playerInput.Disable();
			SwitchGun(24);

			float Dist = (EnemyObj.transform.position - transform.position).magnitude;
			float dx = EnemyObj.transform.position.x - transform.position.x;
			float dx1 = dx * (_killEnemyDistance / Dist);

			float dz = EnemyObj.transform.position.z - transform.position.z;
			float dz1 = dz * (_killEnemyDistance / Dist);

			_KillEnemyPos = new Vector3(EnemyObj.transform.position.x - dx1, 0, EnemyObj.transform.position.z - dz1);

			transform.LookAt(_target, Vector3.up);
			_chest.localRotation = new Quaternion(0, 0, 0, 0);
			EnemyObj.transform.rotation = transform.rotation;
			isAttack = true;
		}
	}

	private void SwitchGun(int gunId)
    {
		Destroy(_currentGun);
		_currentGun = Instantiate(_gunPool[gunId], _gunPos.transform);
		_currentGun.transform.SetParent(_gunPos.transform);
		_currentGun.transform.localPosition = new Vector3(0, 0, 0);
		_currentGun.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
	}

	private void EnemyChecker()
	{
		if ((EnemyObj.transform.position - transform.position).magnitude < _enemyDistance)
		{
			_animator.SetBool("isReady", true);
			_text.enabled = true;
		}
		else
		{
			_animator.SetBool("isReady", false);
			_text.enabled = false;
		}
	}
}
