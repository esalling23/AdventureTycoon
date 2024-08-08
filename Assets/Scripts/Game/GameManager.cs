using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    #region Fields

		private int _gold;

		private GameMode _mode = GameMode.Pause;

		#endregion

		#region Properties

		public int Gold { get { return _gold; } }
		public GameMode Mode { get { return _mode; } }

		#endregion

		#region Methods

    void Start()
    {
        
    }

    void Update()
    {
        
    }

		#endregion
}
