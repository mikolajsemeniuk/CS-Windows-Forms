using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace app
{
    public partial class Form1 : Form
    {
        // Inicjalizacja obiektu który będzie
        // przechowywał dane samochodu wybranego
        // z tabeli w celu modyfikacji albo usuniecia
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
            // Ta linijka jest aby zmienic domyslny format daty w tabeli z `dd.MM.yyyy hh:ss` na `dd.MM.yyyy`
            dataGridView1.Columns[3].DefaultCellStyle.Format = "dd.MM.yyyy";

            // ta linijka uruchomi funckje `OnUpdatedInputs`
            // kiedy zmienimy wartosci w zmiennej `_car` za pomocą
            // eventu `UpdatedInputs`, funckja `OnUpdatedInputs`
            // zmieni wartosci inputow tak aby zawsze byly aktualne
            // np po wyborze samochodu albo kliknieciu cancel
            _car.UpdatedInputs += OnUpdatedInputs;

            await GetCars();
        }

        // wyciagnij wszystkie samochody asynchronicznie z tabeli i wloz je do tabeli `dataGridView1`
        private async Task GetCars()
        {
            using (DBEntities entities = new DBEntities())
            {
                dataGridView1.DataSource = await entities.Cars.ToListAsync();
            }
        }
        // dodaj samochod do bazy danej ktory ma identyczne dane jak zmienna `_car`
        // i zapisz zmianny asynchronicznie
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
        // zaktualizuj samochod z bazy danej ktory ma identyczne dane jak zmienna `_car`
        // i zapisz zmianny asynchronicznie
        private async Task UpdateCar()
        {
            using (DBEntities entities = new DBEntities())
            {
                entities.Entry(_car).State = EntityState.Modified;
                await entities.SaveChangesAsync();
            }
        }
        // zaktualizuj samochod z bazy danej ktory ma identyczne dane jak zmienna `_car`
        // i zapisz zmianny asynchronicznie
        private async Task RemoveCar()
        {
            using (DBEntities entities = new DBEntities())
            {
                var car = await entities.Cars.FindAsync(_car.Id);
                entities.Cars.Remove(car);
                await entities.SaveChangesAsync();
            }
        }
        // zmien wartosc w zmiennej `_car.Brand` na wartosc z inputa `Brand`
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _car.Brand = textBox1.Text;
        }
        // zmien wartosc w zmiennej `_car.Model` na wartosc z inputa `Model`
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            _car.Model = textBox2.Text;
        }
        // zmien wartosc w zmiennej `_car.ProductionYear` na wartosc z inputa `Production Year`
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            _car.ProductionYear = dateTimePicker1.Value;
        }
        // zaznacz cala linijke podczas wyboru samochodu z tabeli
        // i wywolaj funckje setCar ktory zaktualizauje nam dane i wywola event
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
        // jezeli id ze zmiennej `_car` jest rowne 0 to znaczy ze 
        // chcemy utworzyc nowy rekord w bazie i wywolac funckje `AddCar`
        // a jezeli inny niz 0 to chcemy zaktuatlizowac i wywolac funkcje `UpdateCar`
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
        // wyczysc dane ze zmiennej `_car` i daj `_car.Id` na zero tak abysmy mogli utworzyc nowy rekord
        private void button2_Click(object sender, EventArgs e)
        {
            Clear();
        }
        // usun samochod z bazy danych
        // wyciagnij uaktualnione samochody z bazy danych do tabeli
        // wyczysc dane ze zmiennej `_car` i daj `_car.Id` na zero tak abysmy mogli utworzyc nowy rekord
        private async void button3_Click(object sender, EventArgs e)
        {
            await RemoveCar();
            await GetCars();
            Clear();
        }
        // ustaw dane w _car na Id = 0, Brand = "", Model = "", ProductionYear = today
        private void Clear()
        {
            _car.SetCar(0, "", "", DateTime.Now);
        }
        // jezeli `_car.Id` sie zmieni na 0 to wyczysc pola
        // jezeli nie to daj dane o aktualnym samochodzie
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
