using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SpiderBossAI : MonoBehaviour {

    [Header("Settings")]
    public bool aiDisable = true;
    public bool movementDisable = false;
    public int lives = 3;

    public AudioSource attackedSource;
    public AudioSource source;
    public BoxCollider2D bCollider;
    public SpriteRenderer bossRenderer;
    public GameObject allLights;
    public GameObject ui;
    public Light2D spriteLight;
    public ParticleSystem ps;

    public float minX = 0f;
    public float maxX = 10f;
    public float minY = 0f;
    public float maxY = 10f;

    public float xLerpSpeed = 2f;
    public float yLerpSpeed = 1f;

    public float reTargetYTimer = 1f;

    public Sprite[] normalSprite;
    public Sprite[] warningSprite;
    public Sprite attackSprite;
    public Sprite resetSprite;

    public CatsMovement cm;
    public GameObject warningLight;

    public GameObject[] removed;
    public GameObject[] placed;

    public Camera cam;

    [Header("Attacks")]
    public float attackInterval = 5f;
    public float bcHeightReq = 0f;
    public float meleeAttackSpeed = 1f;

    public GameObject rangedAttackObj;

    private float targetY = 0f;
    private float t = 0f;
    private float ta = 0f;
    private bool isRangedAttack = false;
    private bool attacked = false;

    private void Start() {
        if (GlobalValues.sbDead) {
            aiDisable = true;
            this.gameObject.SetActive(false);

            for (int i = 0; i < removed.Length; i++)
                removed[i].SetActive(false);

            for (int i = 0; i < removed.Length; i++)
                placed[i].SetActive(true);
        } else {
            targetY = Random.Range(minY, maxY);
        }
    }

    private void Update() {
        if (lives < 1) {
            aiDisable = true;
            GlobalValues.sbDead = true;
            bossRenderer.enabled = false;
            this.enabled = false;
            allLights.SetActive(false);
            ps.Emit(200);

            for (int i = 0; i < removed.Length; i++)
                removed[i].SetActive(false);

            for (int i = 0; i < removed.Length; i++)
                placed[i].SetActive(true);

            cm.cameraMovementDisable = false;
            cam.orthographicSize = 5f;
        }

        if (!aiDisable) {
            if (!movementDisable) {
                Movement();

                if (ta > attackInterval) {
                    if (Random.Range(0f, 1f) > 0.5f)
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
        if (collision.tag == "Cat" && !cm.bcGrounded) {
            --lives;
            StartCoroutine(FlashRed());
            attacked = true;
        }
    }

    private void Movement() {
        if (t > reTargetYTimer) {
            targetY = Random.Range(minY, maxY);
            t = 0f;

            if (targetY >= bossRenderer.transform.position.y) {
                if (!movementDisable) {
                    bossRenderer.sprite = normalSprite[1];
                    warningLight.SetActive(false);
                } else {
                    bossRenderer.sprite = warningSprite[1];
                    warningLight.SetActive(true);
                }
            } else {
                if (!movementDisable) {
                    bossRenderer.sprite = normalSprite[0];
                    warningLight.SetActive(false);
                } else {
                    bossRenderer.sprite = warningSprite[0];
                    warningLight.SetActive(true);
                }
            }
        } else { 
            t += Time.deltaTime;
        }

        bossRenderer.transform.position = new Vector3(Mathf.Clamp(Mathf.Lerp(bossRenderer.transform.position.x, cm.bc.position.x, xLerpSpeed * Time.unscaledDeltaTime), minX, maxX), Mathf.Lerp(bossRenderer.transform.position.y, targetY, yLerpSpeed * Time.unscaledDeltaTime), 0f);
    }

    private void InitAttack() {
        if (bcHeightReq < cm.bc.position.y) {
            if (Random.Range(0f, 1f) > 0.55f) {
                movementDisable = true;
                isRangedAttack = true;
            } else {
                movementDisable = true;
                isRangedAttack = false;
            }
        } else {
            movementDisable = true;
            isRangedAttack = true;
        }
    }

    private void Attack() {
        if (isRangedAttack) {
            Movement();
            

            if (Mathf.Abs(bossRenderer.transform.position.x - cm.bc.position.x) < 0.1f) {
                source.Play();
                GameObject go = Instantiate(rangedAttackObj, bossRenderer.transform.position, Quaternion.identity);
                go.transform.position = bossRenderer.transform.position;
                go.GetComponent<WebDagger>().ui = cm.deathUI;

                movementDisable = false;
            }
        } else {
            Movement();

            if (Mathf.Abs(bossRenderer.transform.position.x - cm.bc.position.x) < 0.1f) {
                aiDisable = true;
                bCollider.enabled = true;
                StartCoroutine(AttackAnim());
            }
        }
    }

    private IEnumerator AttackAnim() {
        float target = cm.bc.position.y + 1.5f;

        while (Mathf.Abs(bossRenderer.transform.position.y - target) > 0.2f) {
            bossRenderer.transform.position = new Vector3(bossRenderer.transform.position.x, Mathf.Lerp(bossRenderer.transform.position.y, target, meleeAttackSpeed * Time.unscaledDeltaTime), 0f);
            if (attacked) {
                break;
            } else {
                yield return null;
            }
        }

        if (attacked) {
            attacked = false;

            bCollider.enabled = false;

            bossRenderer.sprite = resetSprite;

            while (Mathf.Abs(bossRenderer.transform.position.y - maxY) > 1f) {
                bossRenderer.transform.position = new Vector3(bossRenderer.transform.position.x, Mathf.Lerp(bossRenderer.transform.position.y, maxY, meleeAttackSpeed * Time.unscaledDeltaTime), 0f);

                yield return null;
            }

            movementDisable = false;
            aiDisable = false;
        } else {
            bossRenderer.sprite = attackSprite;

            RaycastHit2D[] col = Physics2D.BoxCastAll(new Vector2(bossRenderer.transform.position.x, bossRenderer.transform.position.y - 1f), new Vector2(1f, 1f), 0f, Vector2.down);

            if (col.Length > 0) {
                for (int i = 0; i < col.Length; i++) {
                    if (col[i].transform.tag == "Cat") {
                        ui.SetActive(true);
                        Time.timeScale = 0f;
                        break;
                    }
                }
            }

            bCollider.enabled = false;

            bossRenderer.sprite = resetSprite;

            target = Random.Range(minY, maxY);

            while (Mathf.Abs(bossRenderer.transform.position.y - target) > 0.2f) {
                bossRenderer.transform.position = new Vector3(bossRenderer.transform.position.x, Mathf.Lerp(bossRenderer.transform.position.y, target, meleeAttackSpeed * Time.unscaledDeltaTime), 0f);

                yield return null;
            }

            movementDisable = false;
            aiDisable = false;
        }
    }

    private IEnumerator FlashRed() {
        attackedSource.Play();
        float flashTimer = 0.25f;

        spriteLight.intensity = 4f;

        while (flashTimer > 0) {
            flashTimer -= Time.unscaledDeltaTime;

            yield return null;
        }

        spriteLight.intensity = 0f;
    }
}
