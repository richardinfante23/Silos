using System.Windows.Forms;

namespace principal
{
    internal static class CatalogGridHelper
    {
        public static void HideColumn(DataGridView grid, string columnName)
        {
            if (grid == null || string.IsNullOrEmpty(columnName)) return;
            if (grid.Columns.Contains(columnName))
                grid.Columns[columnName].Visible = false;
        }

        public static void SetHeader(DataGridView grid, string columnName, string headerText)
        {
            if (grid == null || string.IsNullOrEmpty(columnName)) return;
            if (grid.Columns.Contains(columnName))
                grid.Columns[columnName].HeaderText = headerText;
        }

        public static GridHeaderFilter EnsureHeaderFilter(DataGridView grid, ref GridHeaderFilter filter)
        {
            if (filter == null)
                filter = new GridHeaderFilter(grid);
            return filter;
        }
    }
}
