using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivityListItem : MonoBehaviour
{
    #region Fields

		public TMP_Text titleText;
		public TMP_Text descriptionText;

		public Image typeIcon;

		#endregion

		#region Properties

		// public string Property { get; set; }

		#endregion

		#region Methods

    public void SetData(LocationActivity activity)
    {
      titleText.text = activity.name;
      descriptionText.text = activity.description;

			GameManager manager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
			ActivityTypeData defaultData = manager.GetActivityTypeData(activity.type);
			typeIcon.sprite = defaultData.icon;
    }

    void Update()
    {
        
    }

		#endregion
}
