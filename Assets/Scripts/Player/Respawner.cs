using UnityEngine;
using UnityEngine.UI;

public class Respawner : MonoBehaviour {
    public float respawnHeight = -40f;
    public Image deathImage;
    private Vector3 respawnPoint;
    private CanvasGroup deathScreen;
    private Rigidbody2D rb;
    private Player player;

    // Start is called before the first frame update
    void Start() {
        respawnPoint = gameObject.transform.position;
        deathScreen = deathImage.GetComponent<CanvasGroup>();
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update() {
        if (gameObject.transform.position.y <= respawnHeight) {
            Die();
        }

        if (Input.GetKeyDown(KeyCode.Return) && !player.alive) {
            Respawn();
        }
    }

    public void Die() {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        player.alive = false;
        deathScreen.alpha = 1;
    }

    public void Respawn() {
        gameObject.transform.position = respawnPoint;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;
        deathScreen.alpha = 0;
        player.alive = true;
    }

    public void setRespawnPoint(Vector3 point) {
        respawnPoint = point;
    }
}