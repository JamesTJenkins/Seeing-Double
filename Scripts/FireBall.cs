using UnityEngine;

public class FireBall : MonoBehaviour {

    public enum FireBallDir { Left = 0, Right = 1, Up = 2, Down = 3  }

    public FireBallDir dir = 0;
    public GameObject ui;

    private float t = 0f;
    private float speed = 4f;

    void Update() {
        if (t > 6f) {
            Destroy(this.gameObject);
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

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag != "Cat") {
            Destroy(this.gameObject);
        } else {
            ui.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
