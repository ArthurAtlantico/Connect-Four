using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    protected Image myImage;

    [SerializeField] protected Color redPieceColor;
    [SerializeField] protected Color bluePieceColor;

	public void SetPieceType(PieceType type)
	{
		myImage = GetComponent<Image>();
		myImage.color = type.HasFlag(PieceType.Blue) ? bluePieceColor : redPieceColor;
	}

	public void StartAnimation(Vector3 startPos, Vector3 endPos)
	{
		StartCoroutine(Drop(startPos, endPos));
	}

	protected IEnumerator Drop(Vector3 startPos, Vector3 endPos)
	{
		Utils.GetGameController().BlockInput();
		transform.position = startPos;

		float t = 0;
		while (transform.position != endPos) {
			transform.position = Vector3.Lerp(transform.position, endPos, t);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime;
		}

		Utils.GetGameController().AllowInput();
		Utils.GetGameController().UpdateState();
	}

}
