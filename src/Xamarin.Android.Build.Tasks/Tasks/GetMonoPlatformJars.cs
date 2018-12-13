// Copyright (C) 2011 Xamarin, Inc. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.Android.Tasks
{
	public class GetMonoPlatformJars : Task
	{
		[Required]
		public string[] TargetFrameworkDirectories { get; set; }

		[Required]
		public string[] JarFiles { get; set; }

		[Output]
		public ITaskItem[] MonoPlatformJarPaths { get; set; }

		[Output]
		public ITaskItem[] MonoPlatformDexPaths { get; set; }

		public override bool Execute ()
		{
			List<string> foundJars = new List<string> ();
			List<string> foundDexes = new List<string> ();
			foreach (var dir in TargetFrameworkDirectories) {
				foreach (var jar in JarFiles) {
					var monoPlatformJarPath = Path.Combine (dir, jar);
					if (File.Exists (monoPlatformJarPath)) {
						var monoPlatformDexPath = Path.ChangeExtension (monoPlatformJarPath, ".dex");
						foundJars.Add (monoPlatformJarPath);
						foundDexes.Add (monoPlatformDexPath);
					}
				}
				if (foundJars.Count == JarFiles.Length)
					break;
			}
			if (foundJars.Count == 0) {
				Log.LogCodedError ("XA0002", "Could not find mono.android.jar");
				return false;
			}

			MonoPlatformJarPaths = foundJars.Select (x => new TaskItem (x)).ToArray ();
			MonoPlatformDexPaths = foundDexes.Select (x => new TaskItem (x)).ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
