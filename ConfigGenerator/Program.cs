﻿/*
    _                _      _  ____   _                           _____
   / \    _ __  ___ | |__  (_)/ ___| | |_  ___   __ _  _ __ ___  |  ___|__ _  _ __  _ __ ___
  / _ \  | '__|/ __|| '_ \ | |\___ \ | __|/ _ \ / _` || '_ ` _ \ | |_  / _` || '__|| '_ ` _ \
 / ___ \ | |  | (__ | | | || | ___) || |_|  __/| (_| || | | | | ||  _|| (_| || |   | | | | | |
/_/   \_\|_|   \___||_| |_||_||____/  \__|\___| \__,_||_| |_| |_||_|   \__,_||_|   |_| |_| |_|

 Copyright 2015-2016 Łukasz "JustArchi" Domeradzki
 Contact: JustArchi@JustArchi.net

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0
					
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.

*/

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfigGenerator {
	internal static class Program {
		internal const string ASF = "ASF";
		internal const string ConfigDirectory = "config";
		internal const string GlobalConfigFile = ASF + ".json";

		private const string ASFDirectory = "ArchiSteamFarm";

		private static readonly string ExecutableDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main() {
			Init();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		private static void Init() {
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
			TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;

			Directory.SetCurrentDirectory(ExecutableDirectory);

			// Allow loading configs from source tree if it's a debug build
			if (Debugging.IsDebugBuild) {

				// Common structure is bin/(x64/)Debug/ArchiSteamFarm.exe, so we allow up to 4 directories up
				for (byte i = 0; i < 4; i++) {
					Directory.SetCurrentDirectory("..");
					if (!Directory.Exists(ASFDirectory)) {
						continue;
					}

					Directory.SetCurrentDirectory(ASFDirectory);
					break;
				}

				// If config directory doesn't exist after our adjustment, abort all of that
				if (!Directory.Exists(ConfigDirectory)) {
					Directory.SetCurrentDirectory(ExecutableDirectory);
				}
			}

			if (Directory.Exists(ConfigDirectory)) {
				return;
			}

			Logging.LogGenericError("Config directory could not be found!");
			Environment.Exit(1);
		}

		private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args) {
			if ((sender == null) || (args == null) || (args.ExceptionObject == null)) {
				Logging.LogNullError(nameof(sender) + " || " + nameof(args) + " || " + nameof(args.ExceptionObject));
				return;
			}

			Logging.LogGenericException((Exception) args.ExceptionObject);
		}

		private static void UnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs args) {
			if ((sender == null) || (args == null) || (args.Exception == null)) {
				Logging.LogNullError(nameof(sender) + " || " + nameof(args) + " || " + nameof(args.Exception));
				return;
			}

			Logging.LogGenericException(args.Exception);
		}
	}
}
