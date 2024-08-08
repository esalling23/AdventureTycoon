using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    #region Fields

		[SerializeField] private GameManager _manager;

		#endregion

		#region Properties

		public string Property { get; set; }

		#endregion

		#region Methods

    void Start() {
			
		}

		#endregion
}
