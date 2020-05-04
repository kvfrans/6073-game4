using System.Collections;
using System.Collections.Generic;
using FreeDraw;
using UnityEngine;

public class RemoteCanvas : MonoBehaviour {
    public Drawable dr;

    public SpriteRenderer sr;
    private Texture2D drawable_texture;

    void Start() {
        drawable_texture = sr.sprite.texture;
        drawable_texture.SetPixels(dr.clean_colours_array);
        drawable_texture.Apply();
    }
    
    // Update is called once per frame
    void Update() {
        Color32[] colors = dr.cur_colors;
        if (colors.Length > 0) {
            drawable_texture.SetPixels32(colors);
            drawable_texture.Apply();
        }
    }
}
