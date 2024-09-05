using System;
using UnityEngine;

namespace packageImporter
{
	[Serializable]
	public class Package
	{
		[SerializeField] private string _nameForUI;
		[SerializeField] private string _name;
		[SerializeField] private string _url;

		public string NameForUI => _nameForUI;
		public string Name => _name;
		public string Url => _url;
		public string InManifestString => $"\"{Name}\": \"{Url}\"";
	}
}