
using UnityEngine;

public class AirBoost : MonoBehaviour {
    public float coolDownTime = 2;
    private float coolDownLeft;

    private void Update() {
        if (coolDownLeft > 0) {
            coolDownLeft -= Time.deltaTime;
        }
    }

    public void Use() {
        coolDownLeft = coolDownTime;
    }
}
