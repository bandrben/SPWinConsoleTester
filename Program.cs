using BandR;
using Microsoft.SharePoint;
using System;
using System.IO;
using System.Security;

namespace SPWinConsoleTester
{
    class Program
    {

        /// <summary>
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                CreateLogOutputFile();

                cout("Started...");
                cout();

                Test1();
                //Test2();

            }
            catch (Exception exc)
            {
                cout("ERROR", exc.ToString());
            }

            cout();
            cout("Done. Press any key.");
            Console.ReadLine();

            if (_file != null)
            {
                _file.Dispose();
            }
        }

        /// <summary>
        /// </summary>
        private static void Test1()
        {
            cout("hello world");
        }

        /// <summary>
        /// </summary>
        private static void Test2()
        {
            using (var site = new SPSite("http://sp.bandr.com/sites/GAndEInfoPathDataComp"))
            {
                using (var web = site.OpenWeb())
                {
                    var list = web.Lists.TryGetList("LargeFormLibrary");

                    cout("list found, item count", list.RootFolder.ServerRelativeUrl, list.ItemCount);

                    string sQuery = @"<Where><Contains><FieldRef Name=""FileLeafRef"" /><Value Type=""Text"">.xml</Value></Contains></Where>";
                    string sViewFields = @"<FieldRef Name=""ID"" /><FieldRef Name=""FileLeafRef"" /><FieldRef Name=""FileRef"" /><FieldRef Name=""FSObjType"" />";
                    string sViewAttrs = @"Scope=""Recursive""";
                    uint iRowLimit = 2500;

                    var oQuery = new SPQuery();
                    oQuery.Query = sQuery;
                    oQuery.ViewFields = sViewFields;
                    oQuery.ViewAttributes = sViewAttrs;
                    oQuery.RowLimit = iRowLimit;
                    oQuery.IncludeMandatoryColumns = false;
                    oQuery.IncludeAttachmentUrls = false;
                    oQuery.IncludeAttachmentVersion = false;
                    oQuery.IncludeAllUserPermissions = false;
                    oQuery.IncludePermissions = false;

                    var count1 = 0;
                    var count2 = 0;

                    do
                    {
                        SPListItemCollection collListItems = list.GetItems(oQuery);

                        foreach (SPListItem oListItem in collListItems)
                        {
                            if (oListItem["FSObjType"].ToString() == "0" && oListItem.File != null)
                            {
                                var fileName = oListItem["FileLeafRef"].SafeTrim();

                                if (fileName.EndsWith(".xml"))
                                {
                                    //var b = oListItem.File.OpenBinary(); // ok
                                    var b = new Byte[1];

                                    cout(" -- file found", oListItem["FileLeafRef"], "filesize", b.Length);

                                    count1++;
                                }
                            }
                        }

                        oQuery.ListItemCollectionPosition = collListItems.ListItemCollectionPosition;

                        cout(" ** Page Ended");

                        count2++;

                    } while (oQuery.ListItemCollectionPosition != null);

                    cout("file count", count1);
                    cout("page count", count2);
                }
            }
        }

        #region "Ouput"

        /// <summary>
        /// </summary>
        static void cout(params object[] objs)
        {
            string output = "";

            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] == null) objs[i] = "";

                string delim = " : ";

                if (i == objs.Length - 1) delim = "";

                output += string.Format("{0}{1}", objs[i], delim);
            }
            output += Environment.NewLine;

            Console.Write(output);

            coutFile(objs);
        }

        /// <summary>
        /// </summary>
        static void coutFile(params object[] objs)
        {
            string output = "";

            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] == null) objs[i] = "";

                string delim = "";

                if (!outToCsv)
                {
                    delim = " : ";
                }
                else
                {
                    objs[i] = NormalizeForCsv(objs[i].ToString());
                    delim = ",";
                }

                if (i == objs.Length - 1) delim = "";

                output += string.Format("{0}{1}", objs[i], delim);
            }
            output += Environment.NewLine;

            _file.Write(output);
        }

        /// <summary>
        /// </summary>
        static string NormalizeForCsv(string s)
        {
            s = System.Text.RegularExpressions.Regex.Replace(s, @"\r\n|\n\r|\n|\r", "\r\n");

            if (s.Contains(",") || s.Contains("\r\n"))
            {
                s = string.Concat("\"", s, "\"");
            }

            return s;
        }

        /// <summary>
        /// </summary>
        private static void CreateLogOutputFile()
        {
            //_file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new char[] { '\\' }) + string.Format("\\output [{0}].txt", _curDateTime.ToString("u").Replace(":", "-")));
            _file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new char[] { '\\' }) + "\\output.txt");
        }

        private static StreamWriter _file;
        private static DateTime _curDateTime = DateTime.Now;
        private const bool outToCsv = false;

        #endregion

    }
}
