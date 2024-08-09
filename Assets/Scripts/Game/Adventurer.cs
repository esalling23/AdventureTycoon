using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : MonoBehaviour
{
    #region Fields

		private System.Guid _id = System.Guid.NewGuid();
		private int _gold;

		#endregion

		#region Properties

		public int Gold { get { return _gold; } }
		public System.Guid Id { get { return _id; } }

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
