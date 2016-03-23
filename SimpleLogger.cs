using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BandR
{
    public static class SimpleLogger
    {

        private const string projName = "SPWinConsoleTester";

        private const bool USE_LOG = true; // #changeme should be false
        private static string logFilePath = @"C:\Temp\" + projName + ".log.txt";

        private const bool USE_ULS = true;
        private const string SPDiagCategoryName = projName; // Category filter in ULS

        /// <summary>
        /// </summary>
        public static void Write(params object[] objs)
        {
            string output = "";

            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] == null) objs[i] = "";
                string delim = " : ";
                if (i == objs.Length - 1) delim = "";
                output += string.Concat(objs[i], delim);
            }

            if (USE_LOG)
            {
                try
                {
                    System.IO.File.AppendAllText(logFilePath, DateTime.Now.ToString("o") + " : " + output + Environment.NewLine);
                }
                catch (Exception)
                {
                    // do nothing
                }
            }

            if (USE_ULS)
            {
                try
                {
                    SPDiagnosticsService.Local.WriteTrace(
                        0,
                        new SPDiagnosticsCategory(SPDiagCategoryName, TraceSeverity.High, EventSeverity.Information),
                        TraceSeverity.High,
                        output);
                }
                catch (Exception)
                {
                    // do nothing
                }
            }
        }

    }
}
