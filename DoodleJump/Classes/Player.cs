using System.Drawing;

namespace DoodleJump.Classes {
  public class Player {
    public Physics physics;
    public Image sprite;

    public Player() {
      sprite = Properties.Resources.man2;
      physics = new Physics(new PointF(100, 350), new Size(40, 40));
    }

    public void DrawSprite(Graphics g) {
      g.DrawImage(sprite, physics.transform.position.X, physics.transform.position.Y, physics.transform.size.Width, physics.transform.size.Height);
    }
  }
}
