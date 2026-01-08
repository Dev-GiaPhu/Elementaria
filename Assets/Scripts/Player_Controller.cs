using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.UI;
public class Player_Controller : MonoBehaviour
{
    public enum ElementType { Normal, Fire, Water, Earth, Air };
    
    [System.Serializable]
    public struct PlayerStats {

        public string name;
        public float health;
        public float maxHealth;
        public float speedAttack;
        public float speedPlayer;
        public float jumpForce;
        public float damageplayer;
        public float damageSkill;

    }

    [Header("Cấu hình nhân vật")]
    public ElementType typePlayer;
    private ElementType tempTypePlayer;
    public PlayerStats[] allStats;

    [Header("Chỉ số hiện tại")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float speedAttack;
    public float speedPlayer;
    public float jumpForce;
    public float damageplayer;
    public float damageSkill;

    [Header("Unlock")]
    public bool UnlockWater;
    public bool UnlockFire;
    public bool UnlockEarth;
    public bool UnlockAir;

    [Header("Trạng thái nhân vật")]
    public bool isGrounded;
    public bool isJumping;
    public bool isAttacking;
    public bool isDead;

    [Header("UI Elements")]
    public GameObject UItransStyle;
    public bool isShowUItransStyle;
    private CanvasGroup canvasGroup;
    public GameObject textLocked;


    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D CheckGround;
    


    public void NormalStyle()
    {
        typePlayer = ElementType.Normal;
        StartCoroutine(FadeOutUITransStyle());
        isShowUItransStyle = false;
    }
    public void WaterStyle()
    {
        if (UnlockWater == true)
        {
            typePlayer = ElementType.Water;
        }
        else
        {
            textLocked.SetActive(true);
        }
        StartCoroutine(FadeOutUITransStyle());
        isShowUItransStyle = false;
    }
    public void FireStyle()
    {
        if (UnlockFire == true)
        {
            typePlayer = ElementType.Fire;
            StartCoroutine(FadeOutUITransStyle());
            isShowUItransStyle = false;
        }
    }
    public void EarthStyle()
    {
        if(UnlockEarth == true)
        {
            typePlayer = ElementType.Earth;
            StartCoroutine(FadeOutUITransStyle());
            isShowUItransStyle = false;
        }
    }
    public void AirStyle()
    {
        if (UnlockAir == true)
        {
            typePlayer = ElementType.Air;
            StartCoroutine(FadeOutUITransStyle());
            isShowUItransStyle = false;
        }
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        CheckGround = gameObject.transform.Find("Check Base").GetComponent<BoxCollider2D>();
        canvasGroup = UItransStyle.GetComponent<CanvasGroup>();

        GameObject Ice = GameObject.FindWithTag("Ice");
        GameObject Fire = GameObject.FindWithTag("Fire");
        GameObject Stone = GameObject.FindWithTag("Stone");
        GameObject Wind = GameObject.FindWithTag("Wind");
    }
    void Start()
    {
        ApplyStats();
    }

    void Update()
    {
        if (typePlayer != tempTypePlayer)
        {
            ApplyStats();
        }
        MovePlayer();
        Flip();
        ActionPlayer();
        UpdateAnimations();
        UI();
    }

    void ActionPlayer()
    {
        if( (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1")) && isAttacking == false)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("Collider"))
        {
            isGrounded = true;
            isJumping = false;
            animator.SetBool("Base", true);
        }
    }
    void OnTriggerStay2D(Collider2D trigger)
    {
        if(!isGrounded)
        {
            if (trigger.gameObject.CompareTag("Collider"))
            {
                isGrounded = true;
                isJumping = false;
                animator.SetBool("Base", true);
            }
        }
    }

    void MovePlayer()
    {
        
        float moveX = Input.GetAxis("Horizontal");
        rb.linearVelocityX = moveX * speedPlayer;
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isJumping = true;
            isGrounded = false;
            animator.SetBool("Base", false);
            animator.SetTrigger("Jump");
        }

    }
    void Flip()
    {
        if (rb.linearVelocityX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (rb.linearVelocityX < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void UpdateAnimations()
    {
        switch (typePlayer)
        {
            case ElementType.Normal:
                animator.SetInteger("Trans_Type", 0);
                break;
            case ElementType.Water:
                animator.SetInteger("Trans_Type", 1);
                break;
            case ElementType.Fire:
                animator.SetInteger("Trans_Type", 2);
                break;
            case ElementType.Earth:
                animator.SetInteger("Trans_Type", 3);
                break;
            case ElementType.Air:
                animator.SetInteger("Trans_Type", 4);
                break;
        }
        if(!isJumping)
        {
            if (Mathf.Abs(rb.linearVelocityX) > 0.1f)
                animator.SetBool("IsRun", true);
            else
                animator.SetBool("IsRun", false);
        }
    }

    void UI()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isShowUItransStyle = !isShowUItransStyle;
            if (isShowUItransStyle)
            {
                StartCoroutine(FadeInUITransStyle());
            }
            else
            {
                StartCoroutine(FadeOutUITransStyle());
            }
        }
    }

    IEnumerator FadeInUITransStyle()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        textLocked.SetActive(false);
        UItransStyle.SetActive(true);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            UItransStyle.transform.position = Vector3.Lerp(UItransStyle.transform.position, new Vector3(UItransStyle.transform.position.x, UItransStyle.transform.position.y + 5f, UItransStyle.transform.position.z), elapsed / duration);
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOutUITransStyle()
    {
        float duration = 0.5f; // Thời gian mờ dần
        if( textLocked.activeSelf == true) {duration = 1f;}
        float elapsed = 0f;
        canvasGroup.alpha = 1f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            UItransStyle.transform.position = Vector3.Lerp(UItransStyle.transform.position, new Vector3(UItransStyle.transform.position.x, UItransStyle.transform.position.y - 5f, UItransStyle.transform.position.z), elapsed / duration);
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        UItransStyle.SetActive(false);
    }
    public void ApplyStats() 
    {
        GameObject partical = gameObject.transform.Find("Particle").gameObject;
        partical.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("TransType");
        foreach (var config in allStats)
        {
            if (config.name == typePlayer.ToString()) 
            {
                float healthRatio = health / maxHealth;
                this.maxHealth = config.maxHealth;
                this.health = maxHealth * healthRatio;
                this.speedPlayer = config.speedPlayer;
                this.jumpForce = config.jumpForce;
                this.damageplayer = config.damageplayer;
                this.damageSkill = config.damageSkill;
                this.speedAttack = config.speedAttack;

                tempTypePlayer = typePlayer;
                break;
            }
        }
    }
}
