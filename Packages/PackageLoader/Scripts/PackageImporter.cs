using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace packageImporter
{
	public class PackageImporter : EditorWindow
	{
		[SerializeField] private PackagesForImport packages;

		private bool[] packagesToggles;
		private bool[] startPackagesToggles;

		private string manifestPath;
		private string manifestHead;
		private List<string> manifestPackages;
		private string manifestTail;

		[MenuItem("Package Importer/Package Importer")]
		public static void ShowWindow()
		{
			GetWindow(typeof(PackageImporter));
		}

		private void Awake()
		{
			manifestPath = Path.Combine(Directory.GetCurrentDirectory(), "Packages", "manifest.json");
			manifestPackages = GetPackagesList(manifestPath, out manifestHead, out manifestTail);

			packagesToggles = new bool[packages.Packages.Length];
			startPackagesToggles = new bool[packagesToggles.Length];

			for (int i = 0; i < packagesToggles.Length; i++)
			{
				packagesToggles[i] = PackageIsAvailable(i);
			}

			Array.Copy(packagesToggles, startPackagesToggles, packagesToggles.Length);
		}

		private List<string> GetPackagesList(string manifestPath, out string head, out string tail)
		{
			string data = File.ReadAllText(manifestPath);

			string headSpliter = "\"dependencies\": {";
			string tailSpliter = "}";

			int headSplitIndex = data.IndexOf(headSpliter) + headSpliter.Length;
			head = data[..headSplitIndex];
			string other = data[(headSplitIndex + 1)..];
			int tailSplitIndex = other.IndexOf(tailSpliter);
			string body = other.Substring(0, tailSplitIndex);
			tail = other.Substring(tailSplitIndex);

			string[] packages = body.Split(',');

			for (int i = 0; i < packages.Length; i++)
			{
				packages[i] = packages[i].Trim();
			}

			return packages.ToList();
		}

		private void OnGUI()
		{
			MakeToggles();

			if (GUILayout.Button("Save")) {
				EditPackageList();
			}

			if (GUILayout.Button("Cancel")) {
				Close();
			}
		}

		private void MakeToggles()
		{
			for (int i = 0; i < packagesToggles.Length; i++)
			{
				packagesToggles[i] = EditorGUILayout.Toggle(packages.Packages[i].NameForUI, packagesToggles[i]);
			}
		}

		private bool PackageIsAvailable(int packageIndex)
		{
			return manifestPackages.Contains(packages.Packages[packageIndex].InManifestString);
		}

		private void EditPackageList()
		{
			bool needNewManifest = false;

			for (int i = 0; i < packagesToggles.Length; i++)				
			{
				if(packagesToggles[i] != startPackagesToggles[i])
				{
					needNewManifest = true;
					EditPackageState(i, packagesToggles[i]);
				}
			}

			if (needNewManifest)
			{
				MakeNewManifest();
			}
			
			Close();
		}

		private void EditPackageState(int packageIndex, bool state)
		{
			if (state)
			{
				manifestPackages.Add(packages.Packages[packageIndex].InManifestString);
			}
			else
			{
				manifestPackages.Remove(packages.Packages[packageIndex].InManifestString);
			}
		}

		private void MakeNewManifest()
		{
			string data = string.Empty;
			data += manifestHead;
			data += "\n    ";

			for (int i = 0; i < manifestPackages.Count; i++) {
				data += manifestPackages[i];

				if (i < manifestPackages.Count - 1) {
					data += ',';
				}

				data += $"\n    ";
			}

			data += manifestTail;
			File.WriteAllText(manifestPath, data);
		}
	}
}