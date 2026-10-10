
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Bai5._3_DanhSachSanPham
{
    public class Product
    {
        public string ProductId { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; } = "";
    }

    public partial class Form1 : Form
    {
        private List<Product> products = new List<Product>();
        private BindingSource bindingSource = new BindingSource();

        private TextBox txtProductId = new TextBox();
        private TextBox txtProductName = new TextBox();
        private TextBox txtUnitPrice = new TextBox();
        private TextBox txtQuantity = new TextBox();
        private ComboBox cboCategory = new ComboBox();
        private TextBox txtSearch = new TextBox();
        private DataGridView dgvProducts = new DataGridView();

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Bài 5.3 - Quản lý sản phẩm";
            Size = new Size(950, 600);
            StartPosition = FormStartPosition.CenterScreen;

            GroupBox grpInfo = new GroupBox
            {
                Text = "Thông tin sản phẩm",
                Location = new Point(20, 15),
                Size = new Size(420, 240)
            };

            AddField(grpInfo, "Mã SP:", txtProductId, 25, 30);
            AddField(grpInfo, "Tên SP:", txtProductName, 25, 70);
            AddField(grpInfo, "Đơn giá:", txtUnitPrice, 25, 110);
            AddField(grpInfo, "Số lượng:", txtQuantity, 25, 150);

            Label lblCategory = new Label
            {
                Text = "Danh mục:",
                Location = new Point(25, 195),
                AutoSize = true
            };

            cboCategory.Location = new Point(120, 190);
            cboCategory.Size = new Size(260, 28);
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.Items.AddRange(new object[]
            {
                "Điện tử", "Gia dụng", "Thời trang", "Khác"
            });
            cboCategory.SelectedIndex = 0;

            grpInfo.Controls.Add(lblCategory);
            grpInfo.Controls.Add(cboCategory);
            Controls.Add(grpInfo);

            GroupBox grpActions = new GroupBox
            {
                Text = "Chức năng",
                Location = new Point(460, 15),
                Size = new Size(450, 240)
            };

            Button btnAdd = CreateButton("Thêm", 20, 30);
            Button btnEdit = CreateButton("Sửa", 130, 30);
            Button btnDelete = CreateButton("Xóa", 240, 30);
            Button btnClear = CreateButton("Làm mới", 20, 85);

            Label lblSearch = new Label
            {
                Text = "Tìm theo tên:",
                Location = new Point(20, 145),
                AutoSize = true
            };

            txtSearch.Location = new Point(120, 140);
            txtSearch.Size = new Size(200, 28);

            Button btnSearch = CreateButton("Tìm", 330, 137);

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearInputs();
            btnSearch.Click += BtnSearch_Click;

            grpActions.Controls.AddRange(new Control[]
            {
                btnAdd, btnEdit, btnDelete, btnClear,
                lblSearch, txtSearch, btnSearch
            });

            Controls.Add(grpActions);

            dgvProducts.Location = new Point(20, 275);
            dgvProducts.Size = new Size(890, 270);
            dgvProducts.ReadOnly = true;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;
            dgvProducts.AutoGenerateColumns = true;
            dgvProducts.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            bindingSource.DataSource = new BindingList<Product>(products);
            dgvProducts.DataSource = bindingSource;
            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;

            Controls.Add(dgvProducts);
        }

        private void AddField(
            Control parent, string labelText, TextBox textBox,
            int x, int y)
        {
            Label label = new Label
            {
                Text = labelText,
                Location = new Point(x, y + 4),
                AutoSize = true
            };

            textBox.Location = new Point(x + 95, y);
            textBox.Size = new Size(260, 28);

            parent.Controls.Add(label);
            parent.Controls.Add(textBox);
        }

        private Button CreateButton(string text, int x, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(95, 35)
            };
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtProductId.Text) ||
                string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtUnitPrice.Text) ||
                string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return false;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal price)
                || price < 0)
            {
                MessageBox.Show("Đơn giá phải là số không âm!");
                txtUnitPrice.Focus();
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity)
                || quantity < 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên không âm!");
                txtQuantity.Focus();
                return false;
            }

            return true;
        }

        private Product ReadProduct()
        {
            return new Product
            {
                ProductId = txtProductId.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                UnitPrice = decimal.Parse(txtUnitPrice.Text),
                Quantity = int.Parse(txtQuantity.Text),
                Category = cboCategory.Text
            };
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            if (products.Any(p =>
                p.ProductId.Equals(
                    txtProductId.Text.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại!");
                return;
            }

            products.Add(ReadProduct());
            RefreshGrid();
            ClearInputs();

            MessageBox.Show("Thêm sản phẩm thành công!");
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is not Product selected)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!");
                return;
            }

            if (!ValidateInput()) return;

            if (products.Any(p =>
                p != selected &&
                p.ProductId.Equals(
                    txtProductId.Text.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại!");
                return;
            }

            selected.ProductId = txtProductId.Text.Trim();
            selected.ProductName = txtProductName.Text.Trim();
            selected.UnitPrice = decimal.Parse(txtUnitPrice.Text);
            selected.Quantity = int.Parse(txtQuantity.Text);
            selected.Category = cboCategory.Text;

            RefreshGrid();
            MessageBox.Show("Cập nhật sản phẩm thành công!");
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is not Product selected)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa sản phẩm này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                products.Remove(selected);
                RefreshGrid();
                ClearInputs();
            }
        }

        private void DgvProducts_SelectionChanged(
            object? sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is not Product p)
                return;

            txtProductId.Text = p.ProductId;
            txtProductName.Text = p.ProductName;
            txtUnitPrice.Text = p.UnitPrice.ToString();
            txtQuantity.Text = p.Quantity.ToString();
            cboCategory.SelectedItem = p.Category;
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            var result = products
                .Where(p => p.ProductName.Contains(
                    keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            bindingSource.DataSource = new BindingList<Product>(result);
            dgvProducts.DataSource = bindingSource;
        }

        private void RefreshGrid()
        {
            bindingSource.DataSource = new BindingList<Product>(products);
            dgvProducts.DataSource = bindingSource;
        }

        private void ClearInputs()
        {
            txtProductId.Clear();
            txtProductName.Clear();
            txtUnitPrice.Clear();
            txtQuantity.Clear();
            cboCategory.SelectedIndex = 0;
            dgvProducts.ClearSelection();
            txtProductId.Focus();
        }
    }
}
