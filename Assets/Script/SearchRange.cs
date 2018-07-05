using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchRange : MonoBehaviour {
    private float range = 0.0f;
    private string parentTag;

    private void Awake()
    {
        parentTag = gameObject.transform.parent.gameObject.tag;
    }

    public void SetRange(float rangeRad)
    { //유닛에게서 전달받은 크기만큼 범위 정함
        range = rangeRad;
        gameObject.transform.localScale = new Vector2(range, range);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Contains("Unit"))
        { //유닛이어야하며 동일 진영의 유닛은 취급하지 않는다.
            gameObject.transform.parent.gameObject.GetComponent("UnitBase").AddTarget(collision.gameObject);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Contains("Unit"))
        { //유닛이어야하며 동일 진영의 유닛은 취급하지 않는다.
            gameObject.transform.parent.gameObject.GetComponent("UnitBase").RemoveTarget(collision.gameObject);
        }
    }
}
