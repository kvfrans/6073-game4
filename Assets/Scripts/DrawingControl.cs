using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DrawingControl : MonoBehaviour {

    public SpriteRenderer canvas_mine;
    
    public SpriteRenderer canvas1;
    public SpriteRenderer canvas2;
    public SpriteRenderer canvas3;

    private MasterControl mc;
    
    void Start() {
        mc = GameObject.Find("MasterControl").GetComponent<MasterControl>();
        mc.dc = this;
        StartCoroutine(DrawableSend());
    }
    
    IEnumerator DrawableSend() {
        while (true) {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("sending bytes");
            byte[] imageData;
            imageData = canvas_mine.sprite.texture.EncodeToPNG();
            string s = Convert.ToBase64String(imageData);
            mc.SendStringServer("DRAW", s);
        }
    }
}