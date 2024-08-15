using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MapObject
{
    #region Fields

		[SerializeField] private Sprite _sprite;

		#endregion

		#region Properties

		public SpriteRenderer Renderer { get { return _spriteRenderer; } }


		#endregion

		#region Methods

		public override void Start() 
		{
			base.Start();

			SetSprite();
		}

		public void SetSprite()
		{
			if (!_spriteRenderer) {
				_spriteRenderer = GetComponent<SpriteRenderer>();
			}
			_spriteRenderer.sprite = _sprite;
		}

		public void SetSprite(Sprite sprite) 
		{
			_sprite = sprite;
			SetSprite();
		}

		#endregion
}
