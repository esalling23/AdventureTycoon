using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Any object on the map
// Should represent any object & it's coordinates
public class MapObject : MonoBehaviour
{
    #region Fields

		public GridCell cell;

		protected SpriteRenderer _spriteRenderer;
		

		#endregion

		#region Properties

		public Vector2Int Coordinates { get { return cell.Coordinates; } }

		#endregion

		#region Methods

		public virtual void Start()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		public void SetSpriteSize(float width = 10f, float height = 10f) {
			if (!_spriteRenderer) {
				_spriteRenderer = GetComponent<SpriteRenderer>();
			}
			float ratioX = width / _spriteRenderer.bounds.size.x;
			float ratioY = height / _spriteRenderer.bounds.size.y;
			_spriteRenderer.transform.localScale = Vector3.Scale(_spriteRenderer.transform.localScale, new Vector3(ratioX, ratioY, 1f));
		}

		public void SetSpriteSizeSquare(float setSize, float spriteSize) {
			if (!_spriteRenderer) {
				_spriteRenderer = GetComponent<SpriteRenderer>();
			}
			float ratio = setSize / spriteSize;
			_spriteRenderer.transform.localScale *= ratio;
		}

		#endregion
}
