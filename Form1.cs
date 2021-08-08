using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace app
{
    public partial class Form1 : Form
    {
        private Car _car = new Car
        {
            Id = 0,
            Brand = "",
            Model = "",
            ProductionYear = DateTime.Now
        };

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns[3].DefaultCellStyle.Format = "dd.MM.yyyy";

            _car.UpdatedInputs += OnUpdatedInputs;

            await GetCars();
        }

        private async Task GetCars()
        {
            using (DBEntities entities = new DBEntities())
            {
                dataGridView1.DataSource = await entities.Cars.ToListAsync();
            }
        }

        private async Task AddCar()
        {
            using (DBEntities entities = new DBEntities())
            {
                entities.Cars.Add(new Car
                {
                    Brand = _car.Brand,
                    Model = _car.Model,
                    ProductionYear = _car.ProductionYear
                });
                await entities.SaveChangesAsync();
            }
        }

        private async Task UpdateCar()
        {
            using (DBEntities entities = new DBEntities())
            {
                entities.Entry(_car).State = EntityState.Modified;
                await entities.SaveChangesAsync();
            }
        }
        
        private async Task RemoveCar()
        {
            using (DBEntities entities = new DBEntities())
            {
                var car = await entities.Cars.FindAsync(_car.Id);
                entities.Cars.Remove(car);
                await entities.SaveChangesAsync();
            }
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _car.Brand = textBox1.Text;
        }
        
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            _car.Model = textBox2.Text;
        }
        
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            _car.ProductionYear = dateTimePicker1.Value;
        }
        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.ClearSelection();
            if (e.RowIndex != -1)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;

                _car.SetCar(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value),
                    dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(),
                    dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString(),
                    Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString()));
            }
        }
        
        private async void button1_Click(object sender, EventArgs e)
        {
            if (_car.Brand.Equals(""))
            {
                MessageBox.Show("Please provide brand name");
                return;
            }

            if (_car.Model.Equals(""))
            {
                MessageBox.Show("Please provide model name");
                return;
            }

            if (_car.Id == 0)
            {
                await AddCar();
                await GetCars();
                Clear();
            }
            else
            {
                await UpdateCar();
                await GetCars();
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            Clear();
        }
        
        private async void button3_Click(object sender, EventArgs e)
        {
            await RemoveCar();
            await GetCars();
            Clear();
        }
        
        private void Clear()
        {
            _car.SetCar(0, "", "", DateTime.Now);
        }
        
        public void OnUpdatedInputs(object source, EventArgs args)
        {
            if (_car.Id == 0)
            {
                textBox1.Text = "";
                textBox2.Text = "";
                dateTimePicker1.Value = DateTime.Now;

                button1.Text = "Add";
                button2.Visible = false;
                button3.Visible = false;
                dataGridView1.ClearSelection();
            }
            else
            {
                textBox1.Text = _car.Brand;
                textBox2.Text = _car.Model;
                dateTimePicker1.Value = (DateTime)_car.ProductionYear;

                button1.Text = "Edit";
                button2.Visible = true;
                button3.Visible = true;
            }
        }
    }
}
