using DoodleJump.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoodleJump {
  public partial class Form1 : Form {
    Player player;
    Timer timer;
    Records records = new Records();
    public string playerName;
    
    public Form1() {
      InitializeComponent();
      PauseMenu.Hide();
      playerName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя игрока", "Настройка игрока", "Новый игрок");
      playerName = playerName == "" ? "Новый игрок" : playerName;
      playerName = playerName.ToString().Trim();

      if (playerName.Length > 20) {
        MessageBox.Show("Слишком длинное имя пользователя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Application.Restart();
      }

      Init();
      timer = new Timer();
      timer.Interval = 15;
      timer.Tick += new EventHandler(Update);
      timer.Start();
      KeyDown += new KeyEventHandler(OnKeyboardPressed);
      KeyUp += new KeyEventHandler(OnKeyboardUp);
      BackgroundImage = Properties.Resources.back;
      Height = 600;
      Width = 330;
      Paint += new PaintEventHandler(OnRepaint);
    }

    public void Init() {
      PlatformController.platforms = new List<Platform>();
      PlatformController.AddPlatform(new PointF(100, 400));
      PlatformController.startPlatformPosY = 400;
      PlatformController.score = 0;
      PlatformController.GenerateStartSequence();
      PlatformController.bullets.Clear();
      PlatformController.bonuses.Clear();
      PlatformController.enemies.Clear();
      player = new Player();
    }

    public void NewGame() {
      timer.Start();
    }

    private void OnKeyboardUp(object sender, KeyEventArgs e) {
      player.physics.dx = 0;
      player.sprite = Properties.Resources.man2;
      switch (e.KeyCode.ToString()) {
        case "Space":
        PlatformController.CreateBullet(new PointF(player.physics.transform.position.X + player.physics.transform.size.Width / 2, player.physics.transform.position.Y));
        break;
      }
    }

    private void OnKeyboardPressed(object sender, KeyEventArgs e) {
      switch (e.KeyCode.ToString()) {
        case "Right": player.physics.dx = 6; break;
        case "Left": player.physics.dx = -6; break;
        case "Space": player.sprite = Properties.Resources.man_shooting; break;
      }
    }

    private void Update(object sender, EventArgs e) {
      Text = playerName + ": " + PlatformController.score;

      if ((player.physics.transform.position.Y >= PlatformController.platforms[0].transform.position.Y + 200) || player.physics.StandartCollidePlayerWithObjects(true, false)) {
        SetNewRecord(playerName, PlatformController.score);
        timer.Stop();
        MessageBox.Show("Вы проиграли");
        Init();
        timer.Start();
      }

      player.physics.StandartCollidePlayerWithObjects(false, true);

      if (PlatformController.bullets.Count > 0) {
        for (int i = 0; i < PlatformController.bullets.Count; i++) {
          if (Math.Abs(PlatformController.bullets[i].physics.transform.position.Y - player.physics.transform.position.Y) > 500) {
            PlatformController.RemoveBullet(i);
            continue;
          }
          PlatformController.bullets[i].MoveUp();
        }
      }

      if (PlatformController.enemies.Count > 0) {
        for (int i = 0; i < PlatformController.enemies.Count; i++) {
          if (PlatformController.enemies[i].physics.StandartCollide()) {
            PlatformController.RemoveEnemy(i);
            break;
          }
        }
      }

      player.physics.ApplyPhysics();
      FollowPlayer();

      Invalidate();
    }

    public void FollowPlayer() {
      int offset = 400 - (int)player.physics.transform.position.Y;
      player.physics.transform.position.Y += offset;
      for (int i = 0; i < PlatformController.platforms.Count; i++) {
        var platform = PlatformController.platforms[i];
        platform.transform.position.Y += offset;
      }
      for (int i = 0; i < PlatformController.bullets.Count; i++) {
        var bullet = PlatformController.bullets[i];
        bullet.physics.transform.position.Y += offset;
      }
      for (int i = 0; i < PlatformController.enemies.Count; i++) {
        var enemy = PlatformController.enemies[i];
        enemy.physics.transform.position.Y += offset;
      }
      for (int i = 0; i < PlatformController.bonuses.Count; i++) {
        var bonus = PlatformController.bonuses[i];
        bonus.physics.transform.position.Y += offset;
      }
    }

    private void OnRepaint(object sender, PaintEventArgs e) {
      Graphics g = e.Graphics;
      if (PlatformController.platforms.Count > 0) {
        for (int i = 0; i < PlatformController.platforms.Count; i++)
          PlatformController.platforms[i].DrawSprite(g);
      }
      if (PlatformController.bullets.Count > 0) {
        for (int i = 0; i < PlatformController.bullets.Count; i++)
          PlatformController.bullets[i].DrawSprite(g);
      }
      if (PlatformController.enemies.Count > 0) {
        for (int i = 0; i < PlatformController.enemies.Count; i++)
          PlatformController.enemies[i].DrawSprite(g);
      }
      if (PlatformController.bonuses.Count > 0) {
        for (int i = 0; i < PlatformController.bonuses.Count; i++)
          PlatformController.bonuses[i].DrawSprite(g);
      }
      player.DrawSprite(g);
    }

    private void pictureBox1_Click(object sender, EventArgs e) {
      PauseMenu.Show();
      timer.Stop();
    }

    private void label5_Click(object sender, EventArgs e) {
      PauseMenu.Hide();
      timer.Start();
    }

    private void label3_Click(object sender, EventArgs e) {
      Application.Exit();
    }

    private void label4_Click(object sender, EventArgs e) {
      MessageBox.Show("Правила игры:\r\n\r\nВ Doodle Jump, цель состоит в руководстве четвероногим существом, похожим на пылесосик, имя которого Дудлик, на пути по бесконечной серии платформ без падений.\r\n\r\nИгроки могут получить короткий толчок от различных объектов, таких, как пропеллер-шляпы, реактивные ранцы, ракеты, пружины или батуты. Есть также монстры и НЛО, в которых Дудлер должен стрелять или прыгать на них, ликвидируя. Стрельба осуществляется нажатием на пробел.\r\n\r\nНе существует окончания игры, но конец каждой игры происходит, когда игрок падает (по достижении нижней части экрана) или попадает на монстра.");
    }

    private void label2_Click(object sender, EventArgs e) {
      records.ShowDialog();
    }

    public static void SetNewRecord(string playerName, int record) {
      const string recordsFilePath = @"records.txt";
      string[] records;
      Dictionary<string, int> recordsDict = new Dictionary<string, int>();

      if (!File.Exists(recordsFilePath)) {
        using (StreamWriter sw = new StreamWriter(recordsFilePath, true)) {
          sw.WriteLine($"{playerName}: {record}");
        }
        return;
      }

      using (StreamReader sr = new StreamReader(recordsFilePath, Encoding.UTF8)) {
        records = sr.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
      }

      foreach (var item in records) {
        string[] currentRecord = item.Split(':');
        string name = currentRecord[0].Trim();
        int score = int.Parse(currentRecord[1].Trim());

        recordsDict[name] = score;
      }

      if ((recordsDict.ContainsKey(playerName) && record > recordsDict[playerName]) || !recordsDict.ContainsKey(playerName)) {
        recordsDict[playerName] = record;
      }

      var sortedRecords = from item in recordsDict orderby item.Value descending select item;
      var topRecords = sortedRecords.OrderBy(pair => pair.Value).Take(10).Reverse();

      using (StreamWriter sw = new StreamWriter(recordsFilePath, false)) {
        foreach (var item in topRecords) {
          string name = item.Key;
          int score = item.Value;
          sw.WriteLine($"{name}: {score}\n");
        }
      }
    }

    private void Form1_Load(object sender, EventArgs e) {

    }
  }
}
