using UnityEngine;

namespace packageImporter
{
	[CreateAssetMenu(menuName = "ScriptableObjects/PackagesForImport")]
	public class PackagesForImport : ScriptableObject
	{
		[SerializeField] private Package[] _packages;

		public Package[] Packages => _packages;
	}
}