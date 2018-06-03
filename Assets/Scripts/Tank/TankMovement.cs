using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
	public float m_Speed = 12f, m_TurnSpeed = 180f, m_PitchRange = 0.3f;                 
    public AudioSource m_MovementAudio;    
	public AudioClip m_EngineIdling, m_EngineDriving;           

	 
	private string m_MovementAxisName, m_TurnAxisName;             
    private Rigidbody m_Rigidbody;         
	private float m_MovementInputValue, m_OriginalPitch, m_TurnInputValue, m_OriginalVolume, m_IdleVolumeDrop = 0.8f, m_TargetPitch, m_VelocityOut, m_VelocityOutVol, m_TargetVolume;             


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
		m_OriginalVolume = m_MovementAudio.volume;
		m_TargetPitch = m_MovementAudio.pitch;
		m_TargetVolume = m_MovementAudio.volume;
		m_MovementAudio.volume = m_MovementAudio.volume * m_IdleVolumeDrop;
    }
    

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
		m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
		m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

		EngineAudio ();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
		if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f) 
		{
			if (m_MovementAudio.clip == m_EngineDriving) 
			{
				// falloff
				m_MovementAudio.clip = m_EngineIdling;
				m_TargetPitch = Random.Range (m_OriginalPitch, m_OriginalPitch - m_PitchRange);
				m_TargetVolume = m_MovementAudio.volume * m_IdleVolumeDrop;
				m_MovementAudio.Play ();
			}

			if (m_MovementAudio.pitch != m_TargetPitch)
			{
				m_MovementAudio.pitch = Mathf.SmoothDamp (m_MovementAudio.pitch, m_TargetPitch, ref m_VelocityOut, 2f);
			}
			if (m_MovementAudio.volume != m_TargetVolume)
			{
				m_MovementAudio.volume = Mathf.SmoothDamp (m_MovementAudio.volume, m_TargetVolume, ref m_VelocityOutVol, 1f);
			}
		} 
		else 
		{
			if(m_MovementAudio.clip == m_EngineIdling)
			{
				// wind up
				m_MovementAudio.clip = m_EngineDriving;
				m_TargetPitch = Random.Range (m_OriginalPitch, m_OriginalPitch + m_PitchRange);
				m_TargetVolume = m_OriginalVolume;
				m_MovementAudio.Play ();
			}

			if (m_MovementAudio.pitch != m_TargetPitch)
			{
				m_MovementAudio.pitch = Mathf.SmoothDamp (m_MovementAudio.pitch, m_TargetPitch, ref m_VelocityOut, 2f);
			}
			if (m_MovementAudio.volume != m_TargetVolume)
			{
				m_MovementAudio.volume = Mathf.SmoothDamp (m_MovementAudio.volume, m_TargetVolume, ref m_VelocityOutVol, 1f);
			}
		}
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
		Move ();
		Turn ();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
		Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

		m_Rigidbody.MovePosition (m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation); // Cannot add quaternions, must be multiplied
    }
}