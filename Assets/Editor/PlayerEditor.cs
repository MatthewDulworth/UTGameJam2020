using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor {
    void OnSceneGUI() {
        Player player = (Player) target;
        Vector2 playerPos = player.transform.position;

        // draw player grapple radius
        Handles.color = Color.green;
        Handles.DrawWireDisc(playerPos, Vector3.back, player.grappleRadius);
        
        // draw player boost radius
        Handles.DrawWireDisc(playerPos, Vector3.back, player.boostRadius);
        
        // draw player ground detection
        Handles.color = Color.red;
        Handles.DrawWireDisc(playerPos + player.bottomOffset, Vector3.back, player.collisionRadius);
        
        // draw mouse grapple range
        Vector3 pos = Input.mousePosition;
        pos.z = Camera.main.nearClipPlane;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(pos);
        Handles.DrawWireDisc(mousePos, Vector3.back, player.mouseRadius);
    }
}