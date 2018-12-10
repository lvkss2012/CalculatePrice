using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CalculatePrice;

namespace CalculatePrice
{
    public partial class MainForm : Form
    {
        private List<MenuItemPrice> m_AllMenuItem = new List<MenuItemPrice>();
        private List<MenuItemPrice> m_SelectedMenuItem = new List<MenuItemPrice>();
        private List<int> m_listTotal = new List<int> { 16, 25, 36, 50, 56, 75, 96, 100 };
        private List<int> m_listAccou = new List<int> { 9, 13, 15, 17, 18, 21, 26, 27 };

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var filePath = @"菜单.csv";
            var lines = File.ReadAllLines(filePath, Encoding.Default);
            foreach (var line in lines)
            {
                var cells = line.Split(',');
                var firstCell = cells[0];
                if (firstCell == "名称")
                {
                    continue;
                }

                MenuItemPrice item = new MenuItemPrice();
                item.Name = cells[0];
                item.WaiMai = float.Parse(cells[1]);
                m_AllMenuItem.Add(item);
            }

            SetDateGrid1(m_AllMenuItem);
        }

        private void SetDateGrid1(List<MenuItemPrice> allMenuItem)
        {
            foreach (var item in allMenuItem)
            {
                this.dataGridView1.Rows.Add(false, item.Name, item.WaiMai, item.DianPu);
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void getCellValue(DataGridViewCellCollection cells, int index, ref bool vlaue)
        {
            if (cells == null)
            {
                vlaue = false;
                return;
            }
            var cell = cells[index];
            if (cell == null)
            {
                vlaue = false;
                return;
            }
            var _value = cell.Value;
            if (_value == null)
            {
                vlaue = false;
                return;
            }
            vlaue = bool.Parse(_value.ToString());
            return;
        }

        private void getCellValue(DataGridViewCellCollection cells, int index, ref string vlaue)
        {
            if (cells == null)
            {
                vlaue = "";
                return;
            }
            var cell = cells[index];
            if (cell == null)
            {
                vlaue = "";
                return;
            }
            var _value = cell.Value;
            if (_value == null)
            {
                vlaue = "";
                return;
            }
            vlaue = _value.ToString();
            return;
        }

        private void getCellValue(DataGridViewCellCollection cells, int index, ref float vlaue)
        {
            if (cells == null)
            {
                vlaue = 0;
                return;
            }
            var cell = cells[index];
            if (cell == null)
            {
                vlaue = 0;
                return;
            }
            var _value = cell.Value;
            if (_value == null)
            {
                vlaue = 0;
                return;
            }
            vlaue = float.Parse(_value.ToString());
            return;
        }

        private void getCellValue(DataGridViewCellCollection cells, int index, ref int vlaue)
        {
            if (cells == null)
            {
                vlaue = 0;
                return;
            }
            var cell = cells[index];
            if (cell == null)
            {
                vlaue = 0;
                return;
            }
            var _value = cell.Value;
            if (_value == null)
            {
                vlaue = 0;
                return;
            }
            vlaue = int.Parse(_value.ToString());
            return;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ColumnCheck")
            {
                var row = dataGridView1.Rows[e.RowIndex];
                bool check = false;
                getCellValue(row.Cells, 0, ref check);

                string name = "";
                getCellValue(row.Cells, 1, ref name);

                float waimai = 0;
                getCellValue(row.Cells, 2, ref waimai);

                if (check)
                {
                    this.dataGridView2.Rows.Add(name, 1, 1 * waimai);
                }
                else
                {
                    var find_row = getGridView2Row(name);
                    if (find_row != null)
                    {
                        this.dataGridView2.Rows.Remove(find_row);
                    }
                }

                dataGridView1.Invalidate();

                buttonCalcu_Click(null, null);
            }
        }

        private DataGridViewRow getGridView2Row(string name)
        {
            var rows = dataGridView2.Rows;
            foreach (DataGridViewRow row in rows)
            {
                string _name = "";
                getCellValue(row.Cells, 0, ref _name);

                if (_name == name)
                {
                    return row;
                }
            }
            return null;
        }

        private DataGridViewRow getGridView1Row(string name)
        {
            var rows = dataGridView1.Rows;
            foreach (DataGridViewRow row in rows)
            {
                string _name = "";
                getCellValue(row.Cells, 1, ref _name);

                if (_name == name)
                {
                    return row;
                }
            }
            return null;
        }

        private void buttonCalcu_Click(object sender, EventArgs e)
        {
            this.richTextBoxInfo.Clear();

            var rows = dataGridView2.Rows;
            float total = 0;
            foreach (DataGridViewRow row in rows)
            {
                var totalCell = row.Cells[2].Value;
                if (totalCell == null)
                {
                    continue;
                }
                var price = float.Parse(totalCell.ToString());
                total += price;
            }

            int index = -1;
            for (int i = 0; i < m_listTotal.Count; i++)
            {
                var _tatal = m_listTotal[i];
                if (_tatal <= total)
                {
                    index = i;
                }
                else
                {
                    break;
                }
            }

            int account = 0;
            if (index >= 0)
            {
                account = m_listAccou[index];
                string message = string.Format("原价：{0}\n满{1}减{2}\n合计{3}",
                                    total,
                                    m_listTotal[index],
                                    account,
                                    total - account);
                this.richTextBoxInfo.AppendText(message);
            }
            else
            {
                string message = string.Format("原价：{0}\n合计{1}",
                     total,
                     total);
                this.richTextBoxInfo.AppendText(message);
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (dataGridView2.Columns[e.ColumnIndex].Name != "ColumnCount")
            {
                return;
            }

            var row = this.dataGridView2.Rows[e.RowIndex];
            string name = "";
            getCellValue(row.Cells, 0, ref name);

            int count = 0;
            getCellValue(row.Cells, 1, ref count);

            var _row = getGridView1Row(name);
            float price = 0;
            if (_row != null)
            {
                getCellValue(_row.Cells, 2, ref price);

                row.Cells[2].Value = count * price;
            }

            buttonCalcu_Click(null, null);
        }
    }
}
