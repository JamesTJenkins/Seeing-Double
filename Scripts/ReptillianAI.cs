using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ReptillianAI : MonoBehaviour {

    [Header("Settings")]
    public bool aiDisable = false;
    public int lives = 3;

    public AudioSource source;
    public ParticleSystem ps;
    public Light2D bossLight;
    public CatsMovement cm;
    public Camera cam;
    public Animator animator;
    public GameObject[] gems;

    [Header("Attacks")]
    public float attackInterval = 2f;
    public GameObject attackHitbox;

    public float attackSpeed = 2f;
    public float attackLeftPos;
    public float attackRightPos;

    private bool attacking = false;
    private float ta = 0f;
    private bool isLeft = false;
    private bool inDelay = false;

    private void Start() {
        if (GlobalValues.rDead) {
            aiDisable = true;
            this.gameObject.SetActive(false);

            for (int i = 0; i < gems.Length; i++) {
                gems[i].SetActive(true);
            }
        }
    }

    private void Update() {
        if (lives < 1) {
            aiDisable = true;
            this.enabled = false;
            this.gameObject.SetActive(false);
            bossLight.gameObject.SetActive(false);
            ps.Emit(200);
            cam.orthographicSize = 5f;
            GlobalValues.rDead = true;

            for (int i = 0; i < gems.Length; i++) {
                gems[i].SetActive(true);
            }
        }

        if (!aiDisable) {
            if (!inDelay) {
                if (attacking) {
                    Attack();
                } else if (ta > attackInterval) {
                    ReTarget();
                    InitAttack();
                    StartCoroutine(AttackDelay());

                    ta = 0f;
                } else {
                    ReTarget();

                    ta += Time.unscaledDeltaTime;
                }
            }
        }
    }

    private void InitAttack() {
        attacking = true;
        attackHitbox.SetActive(true);

        if (Random.Range(0f, 1f) < 0.5f) {
            animator.SetBool("IsRunning", true);
        } else {
            animator.SetBool("IsRunningMO", true);
        }
    }

    private void Attack() {
        if (isLeft) {
            this.transform.Translate(-this.transform.right * attackSpeed * Time.deltaTime);

            if (this.transform.position.x < attackLeftPos) {
                StartCoroutine(AttackEndDelay());
            }
        } else {
            this.transform.Translate(this.transform.right * attackSpeed * Time.deltaTime);

            if (this.transform.position.x > attackRightPos) {
                StartCoroutine(AttackEndDelay());
            }
        }
    }


    private void ReTarget() {
        float wcDist = Vector2.Distance(cm.wc.position, this.transform.position);
        float bcDist = Vector2.Distance(cm.bc.position, this.transform.position);

        if (wcDist < bcDist) {
            // Target white cat
            if (cm.wc.position.x > this.transform.position.x) {
                this.transform.localScale = new Vector3(1f, 1f, 1f);
                isLeft = false;
            } else {
                this.transform.localScale = new Vector3(-1f, 1f, 1f);
                isLeft = true;
            }
        } else {
            // Target black cat
            if (cm.bc.position.x > this.transform.position.x) {
                this.transform.localScale = new Vector3(1f, 1f, 1f);
                isLeft = false;
            } else {
                this.transform.localScale = new Vector3(-1f, 1f, 1f);
                isLeft = true;
            }
        }
    }

    private IEnumerator AttackEndDelay() {
        attacking = false;
        attackHitbox.SetActive(false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsRunningMO", false);
        inDelay = true;

        float t = 0f;
        while (t > 2f) {
            t += Time.deltaTime;

            yield return null;
        }

        inDelay = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) { 
        if (collision.CompareTag("Cat")) {
            --lives;
            StartCoroutine(FlashRed());

            aiDisable = true;
            StartCoroutine(Stagger());
        }
    }

    private IEnumerator AttackDelay() {
        float t = 0f;

        while (t < 1f) {
            t += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator FlashRed() {
        source.Play();
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

    private IEnumerator Stagger() {
        float delay = 0.25f;

        while (delay > 0f) {
            delay -= Time.unscaledDeltaTime;

            yield return null;
        }

        aiDisable = false;
    }
}
