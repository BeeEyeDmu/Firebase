using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Data;
using System.Windows.Forms;

namespace CSFirebase
{
  public partial class Form1 : Form
  {
    DataTable dt = new DataTable();

    IFirebaseConfig config = new FirebaseConfig
    {
      AuthSecret = "GrhH4F0XaH46BdrTMVhz2VWTbWN5iBS3XNg4aCuE",
      BasePath = "https://csfirebase-e3e0d-default-rtdb.firebaseio.com/"
    };

    IFirebaseClient client;
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      client = new FireSharp.FirebaseClient(config);

      if (client != null)
        MessageBox.Show("Connection Established!");

      dt.Columns.Add("id");
      dt.Columns.Add("name");
      dt.Columns.Add("address");
      dt.Columns.Add("age");
      dataGridView1.DataSource = dt;

      export();
    }

    // Insert
    private async void button1_Click(object sender, EventArgs e)
    {
      // 자동으로 Id를 하나씩 업데이트
      FirebaseResponse resp = await client.GetAsync("Counter/node");
      Count_class get = resp.ResultAs<Count_class>();
      //MessageBox.Show(get.cnt.ToString());

      var data = new Data
      {
        Id = (Convert.ToInt32(get.cnt)+1).ToString(),  // textBox1.Text
        Name = textBox2.Text,
        Address = textBox3.Text,
        Age = textBox4.Text,
      };
      SetResponse response = await client.SetAsync("Information/"+data.Id, data);
      Data result = response.ResultAs<Data>();

      MessageBox.Show("Data Inserted " + result.Id);
      var obj = new Count_class
      {
        cnt = data.Id
      };
      SetResponse res1 = await client.SetAsync("Counter/node", obj);
      export();
      MessageBox.Show("Counter updated : " + result.Id);
      
    }

    // retrieve
    private async void button2_Click(object sender, EventArgs e)
    {
      FirebaseResponse response = await client.GetAsync("Information/" + textBox1.Text);
      Data obj = response.ResultAs<Data>();
      if (obj == null)
      {
        MessageBox.Show("No such Data!");
        return;
      }
      textBox1.Text = obj.Id;
      textBox2.Text = obj.Name;
      textBox3.Text = obj.Address;
      textBox4.Text = obj.Age;

      MessageBox.Show("Data Retrieved Successfully!"); 
    }

    // update
    private async void button3_Click(object sender, EventArgs e)
    {
      var data = new Data
      {
        Id = textBox1.Text,
        Name = textBox2.Text,
        Address = textBox3.Text,
        Age = textBox4.Text,
      };
      FirebaseResponse response = await client.UpdateAsync("Information/" + textBox1.Text, data);
      Data result = response.ResultAs<Data>();
      export();
      MessageBox.Show("Data updated at Id : " + result.Id);
    }

    // Delete
    private async void button4_Click(object sender, EventArgs e)
    {
      FirebaseResponse response = await client.DeleteAsync("Information/" + textBox1.Text);      
      export();
      MessageBox.Show("Deleted record of Id : " + textBox1.Text);
    }

    // Delete All
    private async void button5_Click(object sender, EventArgs e)
    {
      FirebaseResponse response = await client.DeleteAsync("Information");
      export();
      MessageBox.Show("All Data deleted!\nInformation Node has been deleted!");
    }

    // dataGridView에 보이기
    private void button6_Click(object sender, EventArgs e)
    {
      export();      
    }

    private async void export()
    {
      FirebaseResponse resp1 = await client.GetAsync("Counter/node");
      Count_class obj1 = resp1.ResultAs<Count_class>();

      dt.Rows.Clear();

      int i = 0;
      int cnt = Convert.ToInt32(obj1.cnt);

      while (true)
      {
        if (i == cnt)
          break;
        i++;
        try
        {
          FirebaseResponse resp2 = await client.GetAsync("Information/" + i);
          Data obj = resp2.ResultAs<Data>();

          if (obj == null)
            continue;
          DataRow row = dt.NewRow();
          row["id"] = obj.Id;
          row["name"] = obj.Name;
          row["address"] = obj.Address;
          row["age"] = obj.Age;

          dt.Rows.Add(row);
        }
        catch
        {

        }
      }
      MessageBox.Show("Done!");
    }

    // dataGridView에서 셀이 선택되었을 때
    private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      DataGridView dgv = sender as DataGridView;
      int id = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["id"].Value);
      //MessageBox.Show(dgv.Rows[e.RowIndex].Cells["id"].Value.ToString());

      FirebaseResponse response = await client.GetAsync("Information/" + id);
      Data obj = response.ResultAs<Data>();

      textBox1.Text = obj.Id;
      textBox2.Text = obj.Name;
      textBox3.Text = obj.Address;
      textBox4.Text = obj.Age;
    }
  }
}
