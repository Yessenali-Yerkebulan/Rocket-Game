using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private AudioSource _audioSource;

    [SerializeField] float rotSpeed = 50f;
    [SerializeField] float flySpeed = 30f;
    
    [SerializeField] AudioClip flySound;
    [SerializeField] AudioClip boomSound;
    [SerializeField] AudioClip finishSound;

    [SerializeField] ParticleSystem flyParticles;
    [SerializeField] ParticleSystem boomParticles;
    [SerializeField] ParticleSystem finishParticles;

    private bool collisionOff = false;
    
    enum State {
        Playing, Dead, NextLevel
    };

    private State _state = State.Playing;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == State.Playing)
        {
            Launch();
            Rotation();
        }

        if (Debug.isDebugBuild)
        {
            DebugKeys();
        }
    }

    void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionOff = !collisionOff;
        }
    }

    void Launch()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            flyParticles.Play();
            _rigidbody.AddRelativeForce(Vector3.up * (flySpeed * Time.deltaTime));
            if (!_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(flySound);
            }
        }
        else
        {
            _audioSource.Pause();
            flyParticles.Stop();
        }
    }
    
    void Rotation() {
        float rotationSpeed = rotSpeed * Time.deltaTime;
        
        _rigidbody.freezeRotation = true;
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        } else if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        _rigidbody.freezeRotation = false;
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_state != State.Playing || collisionOff)
        {
            return;
        }
        
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                Finish();
                break;
            case "Battery":
                print("PlusEnergy");
                break;
            default:
                Lose();
                break;
        }
    }

    void Finish()
    {
        _state = State.NextLevel;
        _audioSource.Stop();
        _audioSource.PlayOneShot(finishSound);
        finishParticles.Play();
        Invoke(nameof(LoadNextLevel), 2f);
    }

    void Lose()
    {
        _state = State.Dead;
        Invoke(nameof(LoadFirstLevel), 2f);
        _audioSource.Stop();
        _audioSource.PlayOneShot(boomSound);
        boomParticles.Play();
        print("RocketBoom!");
    }
}
