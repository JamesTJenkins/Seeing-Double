using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackCatFireBall : MonoBehaviour {
    public enum FireBallDir { Left = 0, Right = 1, Up = 2, Down = 3 }

    public FireBallDir dir = 0;
    public CatsMovement cm;

    public float t = 0f;
    public float fireBallTime = 6f;
    private float speed = 4f;

    void Update() {
        if (t > fireBallTime) {
            cm.fire.gameObject.SetActive(false);
            cm.fireExists = false;
        } else {
            switch (dir) {
                case FireBallDir.Left:
                    this.transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
                    break;
                case FireBallDir.Right:
                    this.transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
                    break;
                case FireBallDir.Up:
                    this.transform.position = new Vector2(transform.position.x, transform.position.y + speed * Time.deltaTime);
                    break;
                case FireBallDir.Down:
                    this.transform.position = new Vector2(transform.position.x, transform.position.y - speed * Time.deltaTime);
                    break;
            }

            t += Time.unscaledDeltaTime;
        }
    }

    // Only handles not hitting triggers
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag != "CanBeBurned" && collision.transform.tag != "Cat") {
            cm.fire.gameObject.SetActive(false);
            cm.fireExists = false;
        }
    }
}
