using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.VirtualTexturing;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor {
    void OnSceneGUI() {
        Player player = (Player) target;

        Handles.color = Color.green;
        Handles.DrawWireDisc(player.transform.position, Vector3.back, player.grappleRadius);

        Handles.color = Color.red;
        Handles.DrawWireDisc(player.transform.position + (Vector3) player.bottomOffset,
            Vector3.back, player.collisionRadius);
    }
}