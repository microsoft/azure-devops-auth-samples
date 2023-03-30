using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicePrincipalsSamples
{
    public class TableResult<T>
    {
        public IList<Column<T>> Columns { get; } = new List<Column<T>>();

        public class Column<T>
        {
            public int Padding { get; set; }
            public string Header { get; set; }
            public Func<T, string> GetValue { get; set; }

            public Column(int padding, string header, Func<T, string> getValue)
            {
                Padding = padding;
                Header = header;
                GetValue = getValue;
            }
        }

        public TableResult<T> AddColumn(int padding, string header, Func<T, string> getValue)
        {
            Columns.Add(new Column<T>(padding, header, getValue));
            return this;
        }

        public void Display(IList<T> items)
        {
            var tableFormatBuilder = new StringBuilder();
            var headers = new string[Columns.Count];
            var rows = new Dictionary<int, List<string>>();

            for (var i = 0; i < Columns.Count; i++)
            {
                if (i != 0)
                {
                    tableFormatBuilder.Append(' ');
                }

                tableFormatBuilder.Append($"{{{i},{Columns[i].Padding}}}");
                headers[i] = Columns[i].Header;
                for (var j = 0; j < items.Count; j++)
                {
                    rows.GetOrAddValue(j).Add(Columns[i].GetValue(items[j]));
                }
            }

            var tableFormat = tableFormatBuilder.ToString();
            PrintTableHeaders(tableFormat, headers);

            foreach (var row in rows)
            {
                Console.WriteLine(tableFormat, row.Value.ToArray());
            }
        }

        private static void PrintTableHeaders(string tableFormat, params string[] headers)
        {
            var headersString = string.Format(tableFormat, headers);
            Console.WriteLine(headersString);
            Console.WriteLine(string.Join("", headersString.Select(v => "-")));
        }
    }
}
