using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DoodleJump {
  public partial class Records : Form {
    public Records() {
      InitializeComponent();
    }

    private void pictureBox1_Click(object sender, EventArgs e) {
      Hide();
    }

    private void addRecordsColumn(string filePath, DataGridView dataGridView) {
      List<string> records;
      DataTable recordsTable = new DataTable();

      if (File.Exists(filePath)) {
        records = File.ReadAllLines(filePath).ToList();
      } else {
        records = new List<string>();
      }

      dataGridView.AllowUserToAddRows = false;
      recordsTable.Columns.Add("Игрок", typeof(string));

      for (int i = 0; i < records.Count; i++) {
        if (records[i] != "") {
          recordsTable.Rows.Add(records[i]);
          dataGridView.DataSource = recordsTable;
        }
      }
    }


    private void Clear(DataGridView dataGridView) {
      while (dataGridView.Rows.Count != 0) {
        for (int i = 0; i < dataGridView.Rows.Count; i++) {
          dataGridView.Rows.Remove(dataGridView.Rows[i]);
        }
      }
    }

    private void label1_Click(object sender, EventArgs e) {
      Clear(dataGridView1);
      string PathFile = @"records.txt";
      File.WriteAllText(PathFile, "");
    }

    private void Records_Load(object sender, EventArgs e) {
      addRecordsColumn("records.txt", dataGridView1);
    }
  }
}
