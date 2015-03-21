namespace Renderer
{
  public interface IDisplay
  {
    void SetPixel(int x, int y, float r, float g, float b);
    void UpdateDisplay();
  }
}