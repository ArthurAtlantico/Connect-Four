using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FieldMenu : MonoBehaviour
{
    [SerializeField] protected string sceneName;

    [SerializeField] protected GameObject cursor;

    [SerializeField] protected Color defaultColor;
    [SerializeField] protected Color previewColor;

    private Image _myImage;

    public static bool isAnimating = false;

    private void Awake()
    {
        _myImage = GetComponent<Image>();
    }

    public void MouseEnter()
    {
        if (isAnimating) {
            return;
        }

        cursor.transform.position = new Vector2(transform.position.x, cursor.transform.position.y);
        _myImage.color = previewColor;
    }

    public void MouseExit()
    {
        if (isAnimating) {
            return;
        }

        _myImage.color = defaultColor;
    }

    public void MouseClick()
    {
        if (isAnimating) {
            return;
        }

        isAnimating = true;
        _myImage.color = defaultColor;

        StartCoroutine(Drop(cursor.transform.position, this.transform.position));
    }

    protected IEnumerator Drop(Vector3 startPos, Vector3 endPos)
    {
        cursor.transform.position = startPos;

        float t = 0;
        while (cursor.transform.position != endPos) {
            cursor.transform.position = Vector3.Lerp(cursor.transform.position, endPos, t);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;

            if (t >= 1) {
                break;
            }
        }
        
        isAnimating = false;
        SceneManager.LoadScene(sceneName);
    }

}
