using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Build the tower.
/// </summary>
public class TowerActionBuild : TowerAction
{
    // Tower prefab for this icon
    public GameObject towerPrefab;
	// Icon for disabled state
	public GameObject disabledIcon;

    // Text field for tower price
    private Text price;
	// Level manger has a list with allowed tower upgrades for this level.
	private LevelManager levelManager;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        price = GetComponentInChildren<Text>();
		levelManager = FindObjectOfType<LevelManager>();
		Debug.Assert(price && towerPrefab && enabledIcon && disabledIcon, "Wrong initial parameters");
        // Display tower price
        price.text = towerPrefab.GetComponent<Price>().price.ToString();
		if (levelManager.allowedTowers.Contains(towerPrefab) == true)
		{
			enabledIcon.SetActive(true);
			disabledIcon.SetActive(false);
		}
		else
		{
			enabledIcon.SetActive(false);
			disabledIcon.SetActive(true);
		}
    }

	/// <summary>
	/// Clicked this instance.
	/// </summary>
	protected override void Clicked()
	{
		// Build the tower
		Tower tower = GetComponentInParent<Tower>();
		if (tower != null)
		{
			tower.BuildTower(towerPrefab);
		}
	}
}
