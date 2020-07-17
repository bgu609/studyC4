using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using MySql.Data.MySqlClient;

namespace BookRentalShopApp2020.subforms
{
    public partial class RentalForm : MetroForm
    {
        #region 멤버변수
        private readonly string strConnectionString = "Data Source=localhost; Port=3306; database=bookrentalshop; Uid=root; Password=epfls+358471;";
        readonly string strTblName = "rentalTbl";
        #endregion

        #region 생성자
        public RentalForm()
        {
            InitializeComponent();
        }
        #endregion

        #region 이벤트 핸들러
        private void RentalForm_Load(object sender, EventArgs e)
        {
            UpdateCboMember();
            UpdateCboBook();
            UpdateData();

            InitControls();
        }

        private void metroGrid1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        #endregion

        #region 커스텀 메소드
        private void InitControls()
        {
            /*
            TxtIdx.Text = TxtAuthor.Text = TxtIsbn.Text = TxtNames.Text = TxtPrice.Text = string.Empty;
            CboDivision.SelectedIndex = 0;
            TxtIdx.ReadOnly = true;

            TxtIdx.Focus();

            DtpReleaseDate.CustomFormat = "yyyy-MM-dd";
            DtpReleaseDate.Format = DateTimePickerFormat.Custom;
            DtpReleaseDate.Value = DateTime.Now;*/
        }

        private void UpdateCboMember()
        {
            using (MySqlConnection con = new MySqlConnection(strConnectionString)) //Commons.CONNSTR
            {
                con.Open();

                string strQuery = "SELECT Idx, Names FROM memberTbl ";

                MySqlCommand cmd = new MySqlCommand(strQuery, con);
                MySqlDataReader reader = cmd.ExecuteReader();

                Dictionary<string, string> temp = new Dictionary<string, string>();
                temp.Add("0", "선택");
                while (reader.Read())
                {
                    temp.Add(reader[0].ToString(), reader[1].ToString());
                }

                metroComboBox1.DataSource = new BindingSource(temp, null);
                metroComboBox1.DisplayMember = "Value";
                metroComboBox1.ValueMember = "Key";
                metroComboBox1.SelectedIndex = 0;
            }
        }

        private void UpdateCboBook()
        {
            using (MySqlConnection con = new MySqlConnection(strConnectionString)) //Commons.CONNSTR
            {
                con.Open();

                string strQuery = "SELECT Idx, b.Names, (SELECT d.Names FROM divTbl AS d WHERE Division = b.Division) AS Division FROM booksTbl AS b ";

                MySqlCommand cmd = new MySqlCommand(strQuery, con);
                MySqlDataReader reader = cmd.ExecuteReader();

                Dictionary<string, string> temp = new Dictionary<string, string>();
                temp.Add("0", "선택");
                while (reader.Read())
                {
                    temp.Add($"{reader[0]}", $"[{reader[2]}] {reader[1]}");
                }

                metroComboBox2.DataSource = new BindingSource(temp, null);
                metroComboBox2.DisplayMember = "Value";
                metroComboBox2.ValueMember = "Key";
                metroComboBox2.SelectedIndex = 0;
            }
        }

        private void UpdateData()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(strConnectionString)) //Commons.CONNSTR
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = con
                    };

                    string strQuery = "SELECT r.idx AS '대여번호', " +
                                      "       m.Names AS '대여회원', " +
                                      "       b.Names AS '대여책제목', " +
                                      "       b.ISBN, " +
                                      "       r.rentalDate AS '대여일' " +
                                      "  FROM rentaltbl AS r " +
                                      " INNER JOIN membertbl AS m " +
                                      "    ON r.memberIdx = m.Idx " +
                                      " INNER JOIN bookstbl AS b " +
                                      "    ON r.bookIdx = b.Idx ";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(strQuery, con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, strTblName);

                    metroGrid1.DataSource = ds;
                    metroGrid1.DataMember = strTblName;
                }
                SetColumnHeaders();
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(this, $"에러 발생 {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetColumnHeaders()
        {
            DataGridViewColumn column;

            column = metroGrid1.Columns[0];
            column.Width = 50;
            column.HeaderText = "대여번호";

            column = metroGrid1.Columns[1];
            column.Width = 80;
            column.HeaderText = "대여회원";

            column = metroGrid1.Columns[2];
            column.Width = 200;
            column.HeaderText = "대여책 제목";

            column = metroGrid1.Columns[3];
            column.Width = 100;
            column.HeaderText = "ISBN";

            column = metroGrid1.Columns[4];
            column.Width = 100;
            column.HeaderText = "대여일";
        }
        #endregion
    }
}
