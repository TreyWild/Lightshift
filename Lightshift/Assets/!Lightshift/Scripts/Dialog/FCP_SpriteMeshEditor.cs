using UnityEngine;

[ExecuteInEditMode]
public class FCP_SpriteMeshEditor : MonoBehaviour {

    public int x, y;
    public Sprite sprite;
    private Sprite bufferedSprite;

    void Update() {
        if(sprite != bufferedSprite) {
            if(sprite == null)
                bufferedSprite = null;
            else {
                MakeMesh(sprite, x, y);
#if UNITY_EDITOR
                if(UnityEditor.Selection.Contains(gameObject))
                    Debug.Log("FCP: gave sprite " + sprite.name + " new mesh of " + x + "x" + y + " resolution");
#endif
                bufferedSprite = sprite;
            }
        }
    }

    private void MakeMesh(Sprite sprite, int x, int y) {
        int px = x + 1;
        int py = y + 1;
        int t = px * py;
        Vector2[] verts = new Vector2[t + (x * y)];
        ushort[] faces = new ushort[x * y * 12];
        for(int i = 0; i < px; i++) {
            float xi = (float)i / x;
            for(int j = 0; j < py; j++) {
                float yi = (float)j / y;
                verts[px * j + i] = new Vector2(xi, yi);
            }
        }
        for(int i = 0; i < x; i++) {
            float xi = (i + .5f) / x;
            for(int j = 0; j < y; j++) {
                float yi = (j + .5f) / y;
                verts[j * x + i + t] = new Vector2(xi, yi);
            }
        }
        for(int i = 0; i < x; i++) {
            for(int j = 0; j < y; j++) {
                int f = 12 * (j * x + i);
                int s = (j * px + i);
                ushort ns = (ushort)(j * x + i + t);
                faces[f + 11] = faces[f] = (ushort)s;
                faces[f + 3] = faces[f + 2] = (ushort)(s + 1);
                faces[f + 6] = faces[f + 5] = (ushort)(s + px + 1);
                faces[f + 9] = faces[f + 8] = (ushort)(s + px);
                faces[f + 1] = faces[f + 4] = faces[f + 7] = faces[f + 10] = ns;
            }
        }
        sprite.OverrideGeometry(verts, faces);
    }
}
