using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaxPlatformer : MonoBehaviour
{
    [Header("Minigame Elements")]
    
    public RectTransform screen;
    public Camera        cam;
    public GameObject       overlay;
    public TextMeshProUGUI  points;
    private int             pts = 0;
    
    [Header("Controls")]
    public float       speed = 10f;
    [Range(0f, 0.25f)]
    public float       runSmoothing = 0.15f;
    public float       jumpForce = 26f;
    public float       jumpCd = 0.6f;
    private float      lastJumpTime;
    [Header("Max")]
    public Rigidbody2D   maxRb;
    public GameObject idle;
    public GameObject moving;
    public GameObject shot;
    [Header("Audio")]
    public AudioSource  audioSource;
    public AudioClip    music;
    public AudioClip    jump;
    public AudioClip    land;
    public AudioClip    rifle;
    public AudioClip    death;
    private Vector2 dir = Vector2.zero;
    private Vector2 veloc = Vector2.zero;
    private Vector3 currentScale = Vector3.one;
    private bool _jump = false;
    private bool isGrounded = true;
    private bool wasGrounded = true;
    private bool dead = false;
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode upKey;
    private float raycastDist = 1.1f;

    private void Awake()
    {
        leftKey = SoySettings.Instance.leftKey;
        rightKey = SoySettings.Instance.rightKey;
        upKey = SoySettings.Instance.upKey;
        Init();
    }

    private void Start()
    {
        Vector3 res = new Vector3(1920f / Screen.width, 1080f / Screen.height, 1f);
        if (res.x >= 1) // smaller than 1920x1080
            screen.localScale = res;
        else
        {
            jumpForce *= Screen.width / 1920f;
            cam.orthographicSize = 5 * Screen.width / 1920f;
            speed *= 1.2f;
        }
        raycastDist = 1.1f * (Screen.width / 1920f);
    }

    public void Init()
    {
        pts = 0;
        points.text = ":00";
        audioSource.clip = music;
        audioSource.Play();
        overlay.SetActive(true);
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

    private void Update()
    {
        if (dead)
            return;
        if (isGrounded && !wasGrounded)
            audioSource.PlayOneShot(land, 0.25f);
        UpdateFromInputs();
        wasGrounded = isGrounded;
    }

    void FixedUpdate()
    {
        if (dead)
            return;
        dir.y = maxRb.velocity.y;
        idle.SetActive(dir.x  == 0);
        moving.SetActive(dir.x != 0);
        if (_jump)
            Jump();
        maxRb.velocity = Vector2.SmoothDamp(maxRb.velocity, dir, ref veloc, runSmoothing);
        isGrounded = Physics2D.Raycast(maxRb.position, Vector2.down, raycastDist, LayerMask.GetMask("Platform")).collider != null;
        Debug.DrawRay(maxRb.position, Vector2.down * raycastDist, Color.white);
        dir.y = -maxRb.gravityScale;
    }

    private void Jump()
    {
        _jump = false;
        if (Time.time - lastJumpTime < jumpCd)
            return;
        audioSource.PlayOneShot(jump, 0.15f);
        maxRb.velocity = new Vector2(maxRb.velocity.x, 0);
        maxRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        lastJumpTime = Time.time;
    }

    private void Flip()
    {
        currentScale.x *= -1;
        idle.transform.localScale = currentScale;
        moving.transform.localScale = currentScale;
    }

    public void PickUpCollectible(MaxCollectible picked)
    {
        // Debug.Log("picked up" + picked.gameObject.name);
        audioSource.PlayOneShot(picked.pickupSound, 0.5f);
        picked.gameObject.SetActive(false);
        pts += picked.value;
        points.text = ":" + pts.ToString();
    }

    public void SetDeath()
    {
        dead = true;
    }

    public void GetShot()
    {
        audioSource.PlayOneShot(death);
        idle.SetActive(false);
        moving.SetActive(false);
        shot.SetActive(true);
        if (pts == 730)
            AchievementsManager.Instance.UnlockChievo("chievo12");
        Invoke("EndGame", 0.5f);
    }

    public void EndGame()
    {
        audioSource.Stop();
        MinigamesManager.Instance.EndMinigame();
    }
}
