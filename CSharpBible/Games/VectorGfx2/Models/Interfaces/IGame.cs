using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorGfx2.Models.Interfaces;

public interface IGame
{
    int Score { get; set; }
    int Lives { get; set; }

    public void Start();
    public void Stop();
    public void Pause();
    public void Resume();
    public void Reset();
    public void Update();
    public void Draw();
    public void LoadContent();
    public void UnloadContent();
    public void Initialize();
    public void Dispose();
}
