using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CatsMovement : MonoBehaviour {

    [Header("Settings")]
    public bool lockMovement = false;
    public bool lockSwitching = false;
    public bool cameraMovementDisable = false;
    public bool pauseMenuDisable = false;
    public bool byAltar = false;
    public GameObject pauseMenu;
    public GameObject deathUI;

    [Header ("Altar Settings")]
    public Transform altar;
    public Text gemCounter;
    public SpriteRenderer altarIcon;
    public Text altarText;
    public Sprite[] altarOptions;
    public int[] altarCosts;
    public CloudForm clouds;
    
    [Header("Controls")]
    public KeyCode moveLeft = KeyCode.LeftArrow;
    public KeyCode moveRight = KeyCode.RightArrow;
    public KeyCode jump = KeyCode.UpArrow;
    public KeyCode exit = KeyCode.DownArrow;
    public KeyCode switchKey = KeyCode.R;
    public KeyCode pause = KeyCode.Escape;
    public KeyCode ability1 = KeyCode.E;

    [Header("Camera")]
    public Transform cam;
    public float minCameraXMove;
    public float maxCameraXMove;

    [Header("White Cat Settings")]
    public Rigidbody2D wc;
    public float wcMoveSpeed = 2f;
    public float wcJumpMultiplier = 2f;
    public float fearLength = 2f;
    public bool wcGrounded = false;
    public Animator wcAnimator;
    public SpriteRenderer wcSR;

    [Header("White Cat Abilities")]
    public Rigidbody2D cloudSpit;

    [Header("Black Cat Settings")]
    public Rigidbody2D bc;
    public float bcMoveSpeed = 2f;
    public float bcJumpMultiplier = 2f;
    public bool bcGrounded = false;
    public Animator bcAnimator;
    public SpriteRenderer bcSR;

    [Header("Black Cat Abilities")]
    public BlackCatFireBall fire;
    public float fireballTime = 6f;
    public bool fireExists = false;

    private bool prevMovementLeft = false;
    private bool isFeared = false;
    private bool fearIsLeft = true;
    private float ccTimer = 0f;
    private bool cloudExists = false;
    private bool killCloud = false;
    private bool wcCurrFlip = false;
    private bool bcCurrFlip = false;
    private bool interactingWithAltar = false;
    private bool disableAltar = false;

    private void Start() {
        LerpCameraToCat(true);
    }

    private void Update() {

        // Sprite flip

        wcSR.flipX = wcCurrFlip;

        if (bcSR != null)
            bcSR.flipX = bcCurrFlip;

        // Pausing

        if (!pauseMenuDisable && Input.GetKeyDown(pause)) {
            if (pauseMenu.activeSelf) {
                pauseMenu.SetActive(false);
            } else {
                pauseMenu.SetActive(true);
            }
        }

        // Update CC on cats

        UpdateWhiteCatCC();

        // If not in altar
        if (!interactingWithAltar) {

            // Cat Switch

            if (Input.GetKeyDown(switchKey) && !lockSwitching) {
                GlobalValues.isWhiteCat = !GlobalValues.isWhiteCat;
                UpdateWhiteCatAnimation(0, false, false);
                UpdateBlackCatAnimation(0, false, false);
            }

            // Cat Movement

            if (!lockMovement) {
                if (GlobalValues.isWhiteCat) {
                    if (Input.GetKey(moveLeft)) {
                        wc.AddForce(Vector2.left * wcMoveSpeed * Time.unscaledDeltaTime, ForceMode2D.Impulse);
                        UpdateWhiteCatAnimation(0, true, true);

                        if (!prevMovementLeft) {
                            wc.velocity = new Vector2(0f, wc.velocity.y);
                            prevMovementLeft = true;
                        }
                    } else if (Input.GetKey(moveRight)) {
                        wc.AddForce(Vector2.right * wcMoveSpeed * Time.unscaledDeltaTime, ForceMode2D.Impulse);
                        UpdateWhiteCatAnimation(0, true, false);

                        if (prevMovementLeft) {
                            wc.velocity = new Vector2(0f, wc.velocity.y);
                            prevMovementLeft = false;
                        }
                    } else {
                        wc.velocity = new Vector2(0f, wc.velocity.y);
                        UpdateWhiteCatAnimation(0, false, false);
                    }

                    wc.velocity = new Vector2(Mathf.Clamp(wc.velocity.x, -10f, 10f), wc.velocity.y);

                    if (Input.GetKeyDown(jump) && byAltar && !disableAltar) {
                        if (GlobalValues.altarIndex != -1) {
                            if (Vector2.Distance(wc.transform.position, altar.position) < 2f) {
                                wc.velocity = new Vector2(0f, wc.velocity.y);
                                UpdateWhiteCatAnimation(0, false, false);
                                interactingWithAltar = true;
                                gemCounter.text = "Total Gems: " + GlobalValues.gemsCollected;
                                gemCounter.gameObject.SetActive(true);
                                altarIcon.transform.parent.gameObject.SetActive(true);
                                UpdateAltarOptions(GlobalValues.altarIndex);
                            } else if (Input.GetKeyDown(jump) && wcGrounded) {
                                wc.AddForce(Vector2.up * wcJumpMultiplier, ForceMode2D.Impulse);
                                wcGrounded = false;
                            }
                        }
                    } else if (Input.GetKeyDown(jump) && wcGrounded) {
                        wc.AddForce(Vector2.up * wcJumpMultiplier, ForceMode2D.Impulse);
                        wcGrounded = false;
                    } else if (GlobalValues.canCloudSpit) {
                        // Can only cloud spit if not jumping
                        if (!cloudExists && Input.GetKeyDown(ability1)) {
                            lockMovement = true;
                            lockSwitching = true;
                            // Create cloud
                            if (prevMovementLeft) {
                                UpdateWhiteCatAnimation(2, true, true);
                                cloudSpit.transform.position = new Vector3(wc.position.x - 1f, wc.position.y - 0.5f, 0);
                                StartCoroutine(CreateCloud());
                            } else {
                                UpdateWhiteCatAnimation(2, true, false);
                                cloudSpit.transform.position = new Vector3(wc.position.x + 1f, wc.position.y - 0.5f, 0);
                                StartCoroutine(CreateCloud());
                            }
                        } else if (Input.GetKeyDown(ability1)) {
                            // Flag cloud for death
                            killCloud = true;
                        }
                    }
                } else {
                    if (Input.GetKey(moveLeft)) {
                        bc.AddForce(Vector2.left * bcMoveSpeed * Time.unscaledDeltaTime, ForceMode2D.Impulse);
                        UpdateBlackCatAnimation(0, true, true);

                        if (!prevMovementLeft) {
                            bc.velocity = new Vector2(0f, bc.velocity.y);
                            prevMovementLeft = true;
                        }
                    } else if (Input.GetKey(moveRight)) {
                        bc.AddForce(Vector2.right * bcMoveSpeed * Time.unscaledDeltaTime, ForceMode2D.Impulse);
                        UpdateBlackCatAnimation(0, true, false);

                        if (prevMovementLeft) {
                            bc.velocity = new Vector2(0f, bc.velocity.y);
                            prevMovementLeft = false;
                        }
                    } else {
                        bc.velocity = new Vector2(0f, bc.velocity.y);
                        UpdateBlackCatAnimation(0, false, false);
                    }

                    bc.velocity = new Vector2(Mathf.Clamp(bc.velocity.x, -10f, 10f), bc.velocity.y);

                    if (Input.GetKeyDown(jump) && byAltar && !disableAltar) {
                        if (GlobalValues.altarIndex != -1) {
                            if (Vector2.Distance(bc.transform.position, altar.position) < 2f) {
                                bc.velocity = new Vector2(0f, bc.velocity.y);
                                UpdateBlackCatAnimation(0, false, false);
                                interactingWithAltar = true;
                                gemCounter.text = "Total Gems: " + GlobalValues.gemsCollected;
                                gemCounter.gameObject.SetActive(true);
                                altarIcon.transform.parent.gameObject.SetActive(true);
                                UpdateAltarOptions(GlobalValues.altarIndex);
                            } else if (Input.GetKeyDown(jump) && bcGrounded) {
                                bc.AddForce(Vector2.up * bcJumpMultiplier, ForceMode2D.Impulse);
                                bcGrounded = false;
                            }
                        }
                    } else if(Input.GetKeyDown(jump) && bcGrounded) {
                        bc.AddForce(Vector2.up * bcJumpMultiplier, ForceMode2D.Impulse);
                        bcGrounded = false;
                    } else if (GlobalValues.canFire) {
                        // Can only cloud spit if not jumping
                        if (!fireExists && Input.GetKeyDown(ability1)) {
                            lockMovement = true;
                            lockSwitching = true;
                            // Create fire
                            if (prevMovementLeft) {
                                UpdateBlackCatAnimation(1, true, true);
                                fire.transform.position = new Vector3(bc.position.x - 1f, bc.position.y, 0);
                                CreateFire(true);
                            } else {
                                UpdateBlackCatAnimation(1, true, false);
                                fire.transform.position = new Vector3(bc.position.x + 1f, bc.position.y, 0);
                                CreateFire(false);
                            }
                        }
                    }
                }
            } else {
                UpdateWhiteCatAnimation(0, false, wcCurrFlip);
                if (bcSR != null)
                    UpdateBlackCatAnimation(0, false, bcCurrFlip);
            }
        } else {
            if (Input.GetKeyDown(exit)) {
                altarIcon.transform.parent.gameObject.SetActive(false);
                gemCounter.gameObject.SetActive(false);
                interactingWithAltar = false;
            } else if (Input.GetKeyDown(moveLeft)) {
                NextAltarUpgrade();
                UpdateAltarOptions(GlobalValues.altarIndex);
            } else if (Input.GetKeyDown(moveRight)) {
                NextAltarUpgrade();
                UpdateAltarOptions(GlobalValues.altarIndex);
            } else if (Input.GetKeyDown(jump)) {
                if (altarCosts[GlobalValues.altarIndex] <= GlobalValues.gemsCollected) {
                    GlobalValues.gemsCollected -= altarCosts[GlobalValues.altarIndex];
                    gemCounter.text = "Total Gems: " + GlobalValues.gemsCollected;
                    ApplyAltarUpgrades(GlobalValues.altarIndex);
                }
            }
        }

        // Camera Lerp

        if (!cameraMovementDisable) {
            if (GlobalValues.isWhiteCat) {
                LerpCameraToCat(true);
            } else {
                LerpCameraToCat(false);
            }
        }
    }

    public void DisablePauseMenu() {
        pauseMenuDisable = true;

        if (pauseMenu.activeSelf)
            pauseMenu.SetActive(false);
    }

    public void UpdateWhiteCatAnimation(int animation, bool value, bool flip) {
        switch (animation) {
            case 0:
                wcAnimator.SetBool("IsWalking", value);
                break;
            case 1:
                wcAnimator.SetBool("IsFeared", value);
                break;
            case 2:
                wcAnimator.SetTrigger("IsSpitting");
                break;
        }

        wcCurrFlip = flip;
    }

    public void UpdateBlackCatAnimation(int animation, bool value, bool flip) {
        switch (animation) {
            case 0:
                bcAnimator.SetBool("IsWalking", value);
                break;
            case 1:
                bcAnimator.SetTrigger("IsFiring");
                break;
        }

        bcCurrFlip = flip;
    }

    private void LerpCameraToCat(bool isWc) {
        if (isWc) {
            cam.transform.position = new Vector3(Mathf.Clamp(Mathf.Lerp(cam.transform.position.x, wc.position.x, Time.unscaledDeltaTime * 3f), minCameraXMove, maxCameraXMove), Mathf.Lerp(cam.transform.position.y, wc.position.y, Time.unscaledDeltaTime * 3f), cam.transform.position.z);
        } else {
            cam.transform.position = new Vector3(Mathf.Clamp(Mathf.Lerp(cam.transform.position.x, bc.position.x, Time.unscaledDeltaTime * 3f), minCameraXMove, maxCameraXMove), Mathf.Lerp(cam.transform.position.y, bc.position.y, Time.unscaledDeltaTime * 3f), cam.transform.position.z);
        }
    }

    private void UpdateWhiteCatCC() {
        if (isFeared) {
            if (ccTimer > 0f) {
                ccTimer -= Time.unscaledDeltaTime;
                FearedWhiteCatMovement();
            } else {
                UpdateWhiteCatAnimation(1, false, fearIsLeft);
                isFeared = false;
                lockMovement = false;
                lockSwitching = false;
            }
        }
    }

    public void FearWhiteCat(bool toLeft) {
        fearIsLeft = toLeft;
        ccTimer = fearLength;
        isFeared = true;
        lockMovement = true;
        lockSwitching = true;
        UpdateWhiteCatAnimation(1, true, !toLeft);
    }

    private void FearedWhiteCatMovement() {
        if (fearIsLeft) {
            wc.AddForce(Vector2.left * (wcMoveSpeed / 2) * Time.unscaledDeltaTime, ForceMode2D.Impulse);
        } else {
            wc.AddForce(Vector2.right * (wcMoveSpeed / 2) * Time.unscaledDeltaTime, ForceMode2D.Impulse);
        }
    }

    private void UpdateAltarOptions(int optionIndex) {
        // Check all upgrades havent been brought
        if (GlobalValues.altarIndex == -1) {
            altarIcon.transform.parent.gameObject.SetActive(false);
            gemCounter.gameObject.SetActive(false);
            interactingWithAltar = false;
            disableAltar = true;
        }

        // Update
        if (optionIndex > (altarOptions.Length - 1)) {
            GlobalValues.altarIndex = 0;
        } else if (optionIndex < 0) {
            GlobalValues.altarIndex = altarOptions.Length - 1;
        } else {
            GlobalValues.altarIndex = optionIndex;
        }

        altarIcon.sprite = altarOptions[GlobalValues.altarIndex];
        altarText.text = altarCosts[GlobalValues.altarIndex].ToString();
    }

    private void ApplyAltarUpgrades(int index) {
        switch (index) {
            case 0:
                // Cloudspit
                GlobalValues.canCloudSpit = true;
                GlobalValues.altarAvailability[index] = false;
                NextAltarUpgrade();
                UpdateAltarOptions(GlobalValues.altarIndex);
                break;
            case 1:
                // Fireball
                GlobalValues.canFire = true;
                GlobalValues.altarAvailability[index] = false;
                NextAltarUpgrade();
                UpdateAltarOptions(GlobalValues.altarIndex);
                break;
            case 2:
                // Get Clouds
                clouds.AddCloud();
                if (clouds.maxClouds <= clouds.cloudRegained) {
                    GlobalValues.altarAvailability[index] = false;
                    NextAltarUpgrade();
                    UpdateAltarOptions(GlobalValues.altarIndex);
                }
                break;
        }
    }

    private void NextAltarUpgrade() {
        // Goes to next index
        int tempIndex = GlobalValues.altarIndex + 1;

        if (tempIndex >= GlobalValues.altarAvailability.Length) {
            tempIndex = 0;
        }

        if (GlobalValues.altarAvailability[tempIndex]) {
            GlobalValues.altarIndex = tempIndex;
            return;
        }

        // Checks all
        for (int i = 0; i < GlobalValues.altarAvailability.Length; i++) {
            ++tempIndex;

            if (tempIndex >= GlobalValues.altarAvailability.Length)
                tempIndex = 0;

            if (GlobalValues.altarAvailability[tempIndex]) {
                GlobalValues.altarIndex = tempIndex;
                return;
            }
        }

        // Nothing else
        GlobalValues.altarIndex = -1;
    }

    private void CreateFire(bool isLeft) {
        fireExists = true;

        fire.t = 0f;
        fire.fireBallTime = fireballTime;

        if (isLeft) {
            fire.dir = BlackCatFireBall.FireBallDir.Left;
            fire.GetComponent<SpriteRenderer>().flipX = true;
        } else {
            fire.dir = BlackCatFireBall.FireBallDir.Right;
            fire.GetComponent<SpriteRenderer>().flipX = false;
        }

        StartCoroutine(FireDelay());
    }

    private IEnumerator FireDelay() {
        yield return new WaitForSecondsRealtime(0.5f);

        fire.gameObject.SetActive(true);
        lockMovement = false;
        lockSwitching = false;
    }

    private IEnumerator CreateCloud() {
        cloudExists = true;
        float t = 0f;
        bool triggerReset = true;
        cloudSpit.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        while (t < 10f) {
            yield return new WaitForSeconds(0.5f);
            
            t += 0.5f;

            if (triggerReset) {
                cloudSpit.velocity = Vector2.zero;
                cloudSpit.gameObject.SetActive(true);
                lockMovement = false;
                lockSwitching = false;
                triggerReset = false;
            }

            if (killCloud)
                break;
        }

        killCloud = false;
        cloudExists = false;
        cloudSpit.gameObject.SetActive(false);
        cloudSpit.constraints = RigidbodyConstraints2D.None;
    }
}
