using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class mouseCursor : MonoBehaviour
{
    [SerializeField] GameObject CursorSprite;

    private void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {


        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        CursorSprite.transform.position = pos;
    }
}
