using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Any object on the map
// Should represent any object & it's coordinates
public class MapObject : MonoBehaviour
{
    #region Fields

		private GridCell _cell;

		#endregion

		#region Properties

		public Vector2 Coordinates { get { return _cell.Coordinates; } }

		#endregion

		#region Methods

		#endregion
}
