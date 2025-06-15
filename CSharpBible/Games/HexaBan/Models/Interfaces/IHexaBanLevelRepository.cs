using HexaBan.Models.Interfaces;

public interface IHexaBanLevelRepository
{
    (TileType[,] tiles,(int,int) player) GetLevel(int level);
    string GetLevelAsciiArt(int level);
    string GetLevelHint(int level);
}