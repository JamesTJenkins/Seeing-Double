using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class WingedDemonAI : MonoBehaviour {

    [Header("Settings")]
    public bool aiDisable = false;
    public bool isAttacking = false;
    public int lives = 3;

    public AudioSource attackedSource;
    public AudioSource source;

    public SpriteRenderer bossRenderer;
    public Animator animator;
    public Light2D bossLight;
    public ParticleSystem ps;

    public float minX = 0f;
    public float maxX = 10f;
    public float minY = 0f;
    public float maxY = 10f;

    public float xLerpSpeed = 2f;
    public float yLerpSpeed = 1f;

    public float reTargetYTimer = 1f;
    public float wcSideTargetSpacing = 5f;

    public float exitFightHeight;

    public CatsMovement cm;
    public Camera cam;
    public WDTrigger trigger;

    [Header("Attacks")]
    public float attackInterval = 5f;
    public float wcHeightReq = 0f;

    public GameObject attackObj;

    private float targetY = 0f;
    private float t = 0f;
    private float ta = 0f;
    private bool isSideAttack = false;
    private Vector2 initPos;
    private int maxLives;
    private bool stopDoubleFire = false;

    private void Start() {
        if (GlobalValues.wdDead) {
            aiDisable = true;
            this.gameObject.SetActive(false);
        } else {
            targetY = Random.Range(minY, maxY);
            initPos = transform.position;
            maxLives = lives;
        }
    }

    private void Update() {
        if (lives < 1) {
            aiDisable = true;
            GlobalValues.wdDead = true;
            bossRenderer.enabled = false;
            this.enabled = false;
            bossLight.gameObject.SetActive(false);
            ps.Emit(200);

            cm.cameraMovementDisable = false;
            cam.orthographicSize = 5f;
        } else {
            if (cm.wc.transform.position.y < exitFightHeight) {
                aiDisable = true;
                lives = maxLives;
                transform.position = initPos;

                trigger.enabled = true;

                cm.cameraMovementDisable = false;
                cam.orthographicSize = 5f;
            }
        }

        if (!aiDisable) {
            if (!isAttacking) {
                Movement();

                if (ta > attackInterval) {
                    if (Random.Range(0f, 1f) >= 0.5f)
                        InitAttack();

                    ta = 0f;
                } else {
                    ta += Time.deltaTime;
                }
            } else {
                Attack();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat" && !cm.wcGrounded) {
            --lives;
            animator.SetBool("SideAttack", false);
            animator.SetBool("IdleAttack", false);
            StartCoroutine(FlashRed());
            StartCoroutine(AttackEndDelay());
        }
    }

    private void Movement() {
        if (t > reTargetYTimer) {
            if (wcHeightReq < cm.wc.transform.position.y) {
                targetY = cm.wc.position.y;
            } else {
                targetY = Random.Range(minY, maxY);
                t = 0f;
            }

        } else {
            t += Time.deltaTime;
        }

        if (wcHeightReq > cm.wc.position.y) {
            animator.SetBool("Side", false);

            if (isAttacking) {
                isSideAttack = false;
                animator.SetBool("SideAttack", false);
                animator.SetBool("IdleAttack", true);
            }

            bossRenderer.transform.position = new Vector3(Mathf.Clamp(Mathf.Lerp(bossRenderer.transform.position.x, cm.wc.position.x, xLerpSpeed * Time.unscaledDeltaTime), minX, maxX), Mathf.Lerp(bossRenderer.transform.position.y, targetY, yLerpSpeed * Time.unscaledDeltaTime), 0f);
        } else {
            animator.SetBool("Side", true);

            if (isAttacking) {
                isSideAttack = true;
                animator.SetBool("IdleAttack", false);
                animator.SetBool("SideAttack", true);
            }

            if (cm.wc.transform.position.x < bossRenderer.transform.position.x) {
                bossRenderer.flipX = true;
                bossRenderer.transform.position = new Vector3(Mathf.Clamp(Mathf.Lerp(bossRenderer.transform.position.x, cm.wc.position.x + wcSideTargetSpacing, (xLerpSpeed / 3) * Time.unscaledDeltaTime), minX, maxX), Mathf.Lerp(bossRenderer.transform.position.y, targetY - 3f, yLerpSpeed * Time.unscaledDeltaTime), 0f);
            } else {
                bossRenderer.flipX = false;
                bossRenderer.transform.position = new Vector3(Mathf.Clamp(Mathf.Lerp(bossRenderer.transform.position.x, cm.wc.position.x - wcSideTargetSpacing, (xLerpSpeed / 3) * Time.unscaledDeltaTime), minX, maxX), Mathf.Lerp(bossRenderer.transform.position.y, targetY - 3f, yLerpSpeed * Time.unscaledDeltaTime), 0f);
            }
        }
    }

    private void InitAttack() {
        stopDoubleFire = false;

        if (wcHeightReq > cm.wc.position.y) {
            isAttacking = true;
            isSideAttack = false;

            animator.SetBool("Side", false);
            animator.SetBool("IdleAttack", true);
        } else {
            isAttacking = true;
            isSideAttack = true;

            animator.SetBool("Side", false);
            animator.SetBool("SideAttack", true);

            if (cm.wc.transform.position.x > bossRenderer.transform.position.x) {
                bossRenderer.flipX = true;
            } else {
                bossRenderer.flipX = false;
            }
        }
    }

    private void Attack() {
        if (isSideAttack) {
            Movement();

            if (stopDoubleFire) {
                // Stop both in hope to crush bug
                animator.SetBool("IdleAttack", false);
                animator.SetBool("SideAttack", false);
                return;
            }

            if (Mathf.Abs(bossRenderer.transform.position.y + 3.5f - cm.wc.position.y) < 0.2f) {
                stopDoubleFire = true;

                source.Play();
                GameObject go = Instantiate(attackObj, bossRenderer.transform.position, Quaternion.identity);
                go.transform.position = new Vector2(bossRenderer.transform.position.x, bossRenderer.transform.position.y + 3.8f);

                // Stop both in hope to crush bug
                animator.SetBool("SideAttack", false);
                animator.SetBool("IdleAttack", false);

                if (bossRenderer.flipX) {
                    go.transform.rotation = Quaternion.Euler(0f, 0f, -180f);
                    FireBall f = go.GetComponent<FireBall>();
                    f.dir = FireBall.FireBallDir.Left;
                    f.ui = cm.deathUI;
                } else {
                    FireBall f = go.GetComponent<FireBall>();
                    f.dir = FireBall.FireBallDir.Right;
                    f.ui = cm.deathUI;
                }

                StartCoroutine(AttackEndDelay());
            }
        } else {
            Movement();

            if (stopDoubleFire) {
                // Stop both in hope to crush bug
                animator.SetBool("IdleAttack", false);
                animator.SetBool("SideAttack", false);
                return;
            }

            if (Mathf.Abs(bossRenderer.transform.position.x - cm.wc.position.x) < 0.1f) {
                stopDoubleFire = true;

                // Stop both in hope to crush bug
                animator.SetBool("IdleAttack", false);
                animator.SetBool("SideAttack", false);

                aiDisable = true;
                source.Play();
                GameObject go = Instantiate(attackObj, new Vector3(bossRenderer.transform.position.x, bossRenderer.transform.position.y + 3.8f, bossRenderer.transform.position.z), Quaternion.identity);
                go.transform.position = new Vector2(bossRenderer.transform.position.x, bossRenderer.transform.position.y + 3.8f);
                go.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                FireBall f = go.GetComponent<FireBall>();
                f.dir = FireBall.FireBallDir.Down;
                f.ui = cm.deathUI;

                StartCoroutine(AttackEndDelay());
            }
        }
    }

    private IEnumerator AttackEndDelay() {
        float delay = 0.5f;

        while (delay > 0f) {
            delay -= Time.unscaledDeltaTime;

            yield return null;
        }

        isAttacking = false;
        aiDisable = false;
    }

    private IEnumerator FlashRed() {
        attackedSource.Play();
        float flashTimer = 0.25f;

        Color prev = bossLight.color;
        float prevIten = bossLight.intensity;

        bossLight.color = Color.red;
        bossLight.intensity = 4f;

        while (flashTimer > 0) {
            flashTimer -= Time.unscaledDeltaTime;

            yield return null;
        }

        bossLight.color = prev;
        bossLight.intensity = prevIten;
    }
}
