namespace Renderer
{
  public interface IRender
  {
    string Name { get; }
    bool IsParallel { get; set; }
    void Render();
  }
}