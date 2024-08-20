using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


// Location GameObject that is displayed on the map
public class TempBuildLocation : MapObject
{
    #region Fields

		#endregion

		#region Properties

		public Vector3 WorldPosition { get { 
			return gameObject.transform.position; 
		} }

		#endregion

		#region Methods

		public void SetData(Location data) {
			LocationTypeData defaultData = DataManager.Instance.GetLocationTypeData(data.type);

			_spriteRenderer = GetComponent<SpriteRenderer>();
			if (data.icon) {
				_spriteRenderer.sprite = data.icon;
			} else {
				_spriteRenderer.sprite = defaultData.icon;
			}
		}

		public void SetPosition(Vector3 position)
		{
			transform.position = position;
		}

		public void SetSortingOrder(int order)
		{
			_spriteRenderer.sortingOrder = order;
		}

		#endregion
}
