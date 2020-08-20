using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class Poly2DToMesh : MonoBehaviour {

    PolygonCollider2D polygon;
    Mesh mesh;
    MeshFilter filter;
    Bounds bounds;
    MeshRenderer render;
    Texture texture; 

    void Start() {
        mesh = new Mesh();
        polygon = GetComponent<PolygonCollider2D>();
        polygon.isTrigger = true;       
        filter = GetComponent<MeshFilter>();
        render = GetComponent<MeshRenderer>();
    }

    #if UNITY_EDITOR
    public void UpdateMesh() {
        Vector2[] path = polygon.GetPath(0);        
        mesh.vertices = path.Select(v => new Vector3(v.x, v.y, 0)).ToArray();
        mesh.triangles = new Triangulator(path).Triangulate();
        mesh.RecalculateNormals();
		mesh.RecalculateBounds();        
        filter.mesh = mesh;
        bounds = mesh.bounds;
        if(render.sharedMaterial) {
            if(render.sharedMaterial.GetTexture("_MainTex")) {
                texture = render.sharedMaterial.GetTexture("_MainTex");
            }else {
                texture = new Texture2D((int)bounds.size.x, (int)bounds.size.y, TextureFormat.ARGB32, false);
            }            
        }else {
           texture = new Texture2D((int)bounds.size.x, (int)bounds.size.y, TextureFormat.ARGB32, false); 
        }                        
        mesh.uv = path.Select(v => new Vector2(v.x / (texture.width / 100), v.y / (texture.height / 100))).ToArray();
    }

    void Update() {
		if(!Application.isPlaying) {
            UpdateMesh();
        }            
	}
    #endif
}
