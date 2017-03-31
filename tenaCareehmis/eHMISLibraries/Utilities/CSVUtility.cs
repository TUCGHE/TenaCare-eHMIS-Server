/*
 * 
 * Copyright © 2006-2017 TenaCareeHMIS  software, by The Administrators of the Tulane Educational Fund, 
 * dba Tulane University, Center for Global Health Equity is distributed under the GNU General Public License(GPL).
 * All rights reserved.

 * This file is part of TenaCareeHMIS
 * TenaCareeHMIS is free software: 
 * 
 * you can redistribute it and/or modify it under the terms of the 
 * GNU General Public License as published by the Free Software Foundation, 
 * version 3 of the License, or any later version.
 * TenaCareeHMIS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
 * FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License along with TenaCareeHMIS.  
 * If not, see http://www.gnu.org/licenses/.    
 * 
 * 
 */

using System.Collections;
using System.Data;
using System.IO;
using System.Text;

namespace General.Util
{
    public class CSVUtility
    {
        #region Member Variables

        #endregion

        #region Constructors

        public CSVUtility()
        {
            
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods       

        public static DataTable DoCSVImport(TextReader stream, bool headers)
        {
            DataTable table = new DataTable();
            CsvStream csv = new CsvStream(stream);
            string[] row = csv.GetNextRow();
            if (row == null)
                return null;
            if (headers)
            {
                foreach (string header in row)
                {
                    if (header != null && header.Length > 0 && !table.Columns.Contains(header))
                        table.Columns.Add(header, typeof(string));
                    else
                        table.Columns.Add(GetNextColumnHeader(table), typeof(string));
                }
                row = csv.GetNextRow();
            }
            while (row != null)
            {
                while (row.Length > table.Columns.Count)
                    table.Columns.Add(GetNextColumnHeader(table), typeof(string));
                table.Rows.Add(row);
                row = csv.GetNextRow();
            }
            return table;
        }

        private static string GetNextColumnHeader(DataTable table)
        {
            int c = 1;
            while (true)
            {
                string h = "Column" + c++;
                if (!table.Columns.Contains(h))
                    return h;
            }
        }


        #endregion

        #region Private Methods
        private class CsvStream
        {
            private TextReader stream;

            public CsvStream(TextReader s)
            {
                stream = s;
            }

            public string[] GetNextRow()
            {
                ArrayList row = new ArrayList();
                while (true)
                {
                    string item = GetNextItem();
                    if (item == null)
                        return row.Count == 0 ? null : (string[])row.ToArray(typeof(string));
                    row.Add(item);
                }
            }

            private bool EOS = false;
            private bool EOL = false;

            private string GetNextItem()
            {
                if (EOL)
                {
                    // previous item was last in line, start new line
                    EOL = false;
                    return null;
                }

                bool quoted = false;
                bool predata = true;
                bool postdata = false;
                StringBuilder item = new StringBuilder();

                while (true)
                {
                    char c = GetNextChar(true);
                    if (EOS)
                        return item.Length > 0 ? item.ToString() : null;

                    if ((postdata || !quoted) && c == ',')
                        // end of item, return
                        return item.ToString();

                    if ((predata || postdata || !quoted) && (c == '\x0A' || c == '\x0D'))
                    {
                        // we are at the end of the line, eat newline characters and exit
                        EOL = true;
                        if (c == '\x0D' && GetNextChar(false) == '\x0A')
                            // new line sequence is 0D0A
                            GetNextChar(true);
                        return item.ToString();
                    }

                    if (predata && c == ' ')
                        // whitespace preceeding data, discard
                        continue;

                    if (predata && c == '"')
                    {
                        // quoted data is starting
                        quoted = true;
                        predata = false;
                        continue;
                    }

                    if (predata)
                    {
                        // data is starting without quotes
                        predata = false;
                        item.Append(c);
                        continue;
                    }

                    if (c == '"' && quoted)
                    {
                        if (GetNextChar(false) == '"')
                            // double quotes within quoted string means add a quote       
                            item.Append(GetNextChar(true));
                        else
                            // end-quote reached
                            postdata = true;
                        continue;
                    }

                    // all cases covered, character must be data
                    item.Append(c);
                }
            }

            private char[] buffer = new char[4096];
            private int pos = 0;
            private int length = 0;

            private char GetNextChar(bool eat)
            {
                if (pos >= length)
                {
                    length = stream.ReadBlock(buffer, 0, buffer.Length);
                    if (length == 0)
                    {
                        EOS = true;
                        return '\0';
                    }
                    pos = 0;
                }
                if (eat)
                    return buffer[pos++];
                else
                    return buffer[pos];
            }
        }

        #endregion
    }
}