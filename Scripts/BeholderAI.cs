using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BeholderAI : MonoBehaviour {

    [Header("Settings")]
    public bool aiDisable = false;
    public int lives = 9;

    public SpriteRenderer bossRenderer;
    public AudioSource attackedSource;
    public AudioSource source;
    public Animator animator;
    public Light2D bossLight;
    public ParticleSystem ps;
    public BoxCollider2D bossHitbox;
    public CatsMovement cm;

    public GameObject[] removed;
    public GameObject[] placed;

    [Header("Vines")]
    public Transform vines;
    public bool vineSpinningLeft = true;
    public float vineSpinSpeed = 2f;
    [Header("Laser")]
    public Transform laser;
    public bool laserSpinningLeft = true;

    [Header("Attacks")]
    public float attackInterval = 2f;

    private float ta = 0f;
    private float laserSpinSpeed = 2f;
    private float laserSpinTime = 0f;
    private float laserTime = 0f;
    private bool attacking = false;
    private bool eyeState = false; // False = closed, True = open
    private bool prevEyeState = false;
    

    void Start() {
        if (GlobalValues.tbDead) {
            aiDisable = true;
            this.gameObject.SetActive(false);

            for (int i = 0; i < removed.Length; i++)
                removed[i].SetActive(false);

            for (int i = 0; i < removed.Length; i++)
                placed[i].SetActive(true);
        }
    }

    void Update() {
        if (lives < 1) {
            aiDisable = true;
            GlobalValues.tbDead = true;
            bossRenderer.enabled = false;
            this.enabled = false;
            bossLight.gameObject.SetActive(false);
            vines.gameObject.SetActive(false);
            laser.gameObject.SetActive(false);
            ps.Emit(200);

            for (int i = 0; i < removed.Length; i++)
                removed[i].SetActive(false);

            for (int i = 0; i < removed.Length; i++)
                placed[i].SetActive(true);
        }

        VineSpin(vineSpinningLeft, vineSpinSpeed);

        if (!aiDisable) {
            if (attacking) {
                Attack();
            } else if (ta > attackInterval) {
                InitAttack();

                ta = 0f;
            } else {
                ta += Time.unscaledDeltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat" && !cm.bcGrounded) {
            --lives;
            StartCoroutine(FlashRed());

            aiDisable = true;
            StartCoroutine(Stagger());
        }
    }

    private void InitAttack() {
        attacking = true;
        eyeState = true;
        source.Play();
        laser.gameObject.SetActive(true);

        AnimationUpdate();

        // Get random spin time
        laserSpinTime = Random.Range(1f, 3f);
        // Spin faster if on for less time
        laserSpinSpeed = 70f / (laserSpinTime / 2);
        // Spin towards player
        laserSpinningLeft = GetSpinDirection();

        AttackStartDelay();
    }

    private void Attack() {
        if (laserTime < laserSpinTime) {
            LaserSpin(laserSpinningLeft, laserSpinSpeed);
            laserTime += Time.deltaTime;
        } else {
            attacking = false;
            eyeState = false;
            laserTime = 0f;
            source.Stop();
            laser.gameObject.SetActive(false);
            AnimationUpdate();
            AttackEndDelay();
        }
    }

    private void VineSpin(bool isLeft, float speed) {
        Quaternion q = Quaternion.Euler(vines.rotation.eulerAngles.x, vines.rotation.eulerAngles.y, vines.rotation.eulerAngles.z + (isLeft ? speed : -speed) * Time.deltaTime);
        vines.transform.rotation = q;
    }

    private void LaserSpin(bool isLeft, float speed) {
        Quaternion q = Quaternion.Euler(laser.rotation.eulerAngles.x, laser.rotation.eulerAngles.y, laser.rotation.eulerAngles.z + (isLeft ? speed : -speed) * Time.deltaTime);
        laser.transform.rotation = q;
    }

    private bool GetSpinDirection() {
        float angle = Mathf.Atan2(cm.bc.position.y - laser.position.y, cm.bc.position.x - laser.position.x);

        if (angle > 0) {
            return true;
        } else {
            return false;
        }
    }

    private void AnimationUpdate() {
        if (prevEyeState != eyeState) {
            animator.SetFloat("MotionTime", 0f);

            if (eyeState) {
                animator.Play("EyeOpen");
            } else {
                animator.Play("EyeClose");
            }

            prevEyeState = eyeState;

            PlayAnim();
        }
    }

    private IEnumerator PlayAnim() {
        float time = 0f;

        if (time > 0.1f) {
            time += Time.deltaTime;
            animator.SetFloat("MotionTime", time);

            yield return null;
        }

        bossHitbox.enabled = eyeState;
    }

    private IEnumerator AttackStartDelay() {
        float timer = 5f;

        if (timer < 0f) {
            timer -= Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator AttackEndDelay() {
        float timer = 2f;

        if (timer < 0f) {
            timer -= Time.deltaTime;

            yield return null;
        }
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

    private IEnumerator Stagger() {
        float delay = 0.25f;

        while (delay > 0f) {
            delay -= Time.unscaledDeltaTime;

            yield return null;
        }

        aiDisable = false;
    }
}
