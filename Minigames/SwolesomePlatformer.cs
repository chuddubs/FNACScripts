using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwolesomePlatformer : MonoBehaviour
{
    public Transform cam;
    public RectTransform screen;
    public TextMeshProUGUI score;
    private int currScore = 0;
    private float w = 19.2f;
    private float h = 10.8f;
    [Header("Controls")]
    public float       speed = 10f;
    [Range(0f, 0.25f)]
    public float       runSmoothing = 0.15f;
    public float       jumpForce = 26f;
    public float       jumpCd = 0.6f;
    private float      lastJumpTime;
    [Header("Swolesome")]
    public Rigidbody2D   swoleRb;
    public GameObject idle;
    public GameObject moving;
    [Header("Audio")]
    public AudioSource  audioSource;
    public AudioClip    music;
    public AudioClip    jump;
    public AudioClip    land;
    
    private Vector2 dir = Vector2.zero;
    private Vector2 veloc = Vector2.zero;
    private Vector3 currentScale = Vector3.one;
    
    private bool _jump = false;
    private bool isGrounded = true;
    private bool wasGrounded = true;

    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode upKey;

    private float raycastDist = 2f;
    
    private void Awake()
    {
        Init();
        leftKey = SoySettings.Instance.leftKey;
        rightKey = SoySettings.Instance.rightKey;
        upKey = SoySettings.Instance.upKey;
        h = Screen.height / 100f;
        w = Screen.width / 100f;
    }

    private void Start()
    {
        Vector3 res = new Vector3(1920f / Screen.width, 1080f / Screen.height, 1f);
        if (res.x >= 1) // smaller than 1920x1080
            screen.localScale = res;
        else
        {
            jumpForce *= Screen.width / 1920f;
            cam.GetComponent<Camera>().orthographicSize = 5 * Screen.width / 1920f;
            speed *= 1.2f;
        }
        raycastDist = 2f * (Screen.width / 1920f);
    }

    public void Init()
    {
        audioSource.clip = music;
        audioSource.Play();
    }

    private void UpdateFromInputs()
    {
        if (Input.GetKey(leftKey) && !Input.GetKey(rightKey)) //going left
            dir.x = -speed;
        else if (Input.GetKey(rightKey) && !Input.GetKey(leftKey)) //going right
            dir.x = speed;
        else if (!Input.GetKey(rightKey) && !Input.GetKey(leftKey)) //stopping
            dir.x = 0f;
        if ((dir.x < 0 && currentScale.x == 1) || dir.x > 0 && currentScale.x == -1)
            Flip();
        if (isGrounded && Input.GetKey(upKey))
            _jump = true;

    }

    private void Flip()
    {
        currentScale.x *= -1;
        idle.transform.localScale = currentScale;
        moving.transform.localScale = currentScale;
    }

    private void Update()
    {
        if (isGrounded && !wasGrounded)
            audioSource.PlayOneShot(land, 0.25f);
        UpdateFromInputs();
        wasGrounded = isGrounded;
        cam.position = new Vector3(
            w * Mathf.FloorToInt((swoleRb.position.x + 0.5f * w)/ w),
            h * Mathf.FloorToInt((swoleRb.position.y + 0.5f * h)/ h),
            cam.position.z            
        );
    }
    
    void FixedUpdate()
    {
        dir.y = swoleRb.velocity.y;
        idle.SetActive(dir.x  == 0);
        moving.SetActive(dir.x != 0);
        if (_jump)
            Jump();
        swoleRb.velocity = Vector2.SmoothDamp(swoleRb.velocity, dir, ref veloc, runSmoothing);
        isGrounded = CheckForGround();
        dir.y = -swoleRb.gravityScale;
    }

    private bool CheckForGround()
    {
        Vector3 posLeft = swoleRb.position + Vector2.left * 0.6f;
        Vector3 posRight = swoleRb.position + Vector2.right * 0.6f;
        return Physics2D.Raycast(posLeft, Vector2.down, raycastDist, LayerMask.GetMask("Platform")).collider != null
         ||Physics2D.Raycast(posRight, Vector2.down, raycastDist, LayerMask.GetMask("Platform")).collider != null;
            
    }

    public void Increment(int value)
    {
        currScore += value;
        if (value == 0)
            AchievementsManager.Instance.UnlockChievo("chievo13");
        score.text = currScore.ToString() + "/7";
    }

    private void Jump()
    {
        _jump = false;
        if (Time.time - lastJumpTime < jumpCd)
            return;
        audioSource.PlayOneShot(jump, 0.15f);
        swoleRb.velocity = new Vector2(swoleRb.velocity.x, 0);
        swoleRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        lastJumpTime = Time.time;
    }

}
