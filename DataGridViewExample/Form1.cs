using System.Security.Cryptography.X509Certificates;

using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace DataGridViewExample
{
    public partial class Form1 : Form
    {
        Product selectedProduct;



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            Reload();
        }
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
        }


        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            //var cell = datagridview1.rows[0].cells[1].value.tostring;
            //var index = e;


        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {


            if (e.RowIndex != -1)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                if (row != null)
                {
                    selectedProduct = new Product();
                    selectedProduct.Id = (int)(row.Cells[0].Value);
                    selectedProduct.Name = row.Cells[1].Value.ToString();
                    selectedProduct.Price = (int)(row.Cells[2].Value);

                    txtName.Text = selectedProduct.Name;
                    txtPrice.Text = selectedProduct.Price.ToString();

                }

            }

        }
        private int UpdateProduct(Product updatedProduct)
        {
            SqlConnection connection = new SqlConnection("server=.\\SQLEXPRESS; Initial Catalog=ProductDb;Integrated Security=true");
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "update Products set Name=@name,Price=@price where Id=@id";

            command.Parameters.AddWithValue("@name", updatedProduct.Name);
            command.Parameters.AddWithValue("@price", updatedProduct.Price);
            command.Parameters.AddWithValue("@id", updatedProduct.Id);

            connection.Open();
            int effectedRows = command.ExecuteNonQuery();
            connection.Close();
            return effectedRows;
        }
        private void ClearText()
        {
            txtName.Text = string.Empty;
            txtPrice.Text = string.Empty;
        }
        public List<Product> ListProduct()
        {
            var productList = new List<Product>();
            SqlConnection connection = new SqlConnection("server=.\\SQLEXPRESS;database=ProductDb;Integrated Security=true");

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "select * from Products";
            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var product = new Product();
                product.Id = Convert.ToInt32(reader[0]);
                product.Name = Convert.ToString(reader[1]);
                product.Price = Convert.ToInt32(reader[2]);
                productList.Add(product);
            }
            reader.Close();
            connection.Close();
            return productList;

        }
        public void Reload()
        {
            var data = ListProduct();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = data;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text) && !string.IsNullOrEmpty(txtPrice.Text))
            {
                SqlConnection connection = new SqlConnection("server=.\\SQLEXPRESS; Initial Catalog=ProductDb;Integrated Security=true");
                SqlCommand command = new SqlCommand();
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                command.CommandText = "insert into Products values(@name,@price)";

                var price = int.Parse(txtPrice.Text);
                command.Parameters.AddWithValue("@name", txtName.Text);
                command.Parameters.AddWithValue("@price", price);
                connection.Open();
                var effectedRows = command.ExecuteNonQuery();
                command.Parameters.Clear();
                connection.Close();

                if (effectedRows > 0)
                {
                    MessageBox.Show("Kayýt eklendi.");
                    Reload();
                    ClearText();

                }

            }
            else
            {
                MessageBox.Show("Alanlarý boþ býrakmayýnýz.");
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (selectedProduct != null)
            {

                SqlConnection connection = new SqlConnection("server=.\\SQLEXPRESS; Initial Catalog=ProductDb;Integrated Security=true");
                SqlCommand command = new SqlCommand();
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                command.CommandText = "delete from Products where Id=@id";
                command.Parameters.AddWithValue("@id", selectedProduct.Id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                command.Parameters.Clear();
                Reload();
                ClearText();

            }
            else
            {
                MessageBox.Show("Bir ürün seçiniz.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedProduct != null)
            {
                if (!string.IsNullOrEmpty(txtPrice.Text) && !string.IsNullOrEmpty(txtName.Text))
                {
                    UpdateProduct(new Product { Name = txtName.Text, Price = int.Parse(txtPrice.Text), Id = selectedProduct.Id });
                    Reload();
                }
                else
                {
                    MessageBox.Show("Güncellenecek ürünü seçiniz.");
                }
            }
        }
    }
}