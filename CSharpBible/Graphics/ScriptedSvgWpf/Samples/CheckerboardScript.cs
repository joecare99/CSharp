namespace ScriptedSvgWpf.Samples;

public static class CheckerboardScript
{
    public const string Source = """
canvas(640, 640, "white");
int rows = 47;
float cell = 630/rows;
float centerX = 320;
float centerY = 320;

for (int row = 0; row < rows; row = row + 1) {
    for (int column = 0; column < rows; column = column + 1) {
        if ((row + column) % 2 == 0) {
            float x = centerX + ( column -rows /2 ) * cell;
            float y = centerY - (row - rows/2 + 1) * cell;
            float squareCenterX = x + cell / 2;
            float squareCenterY = y + cell / 2;
            float distance = sqrt(
                (squareCenterX - centerX) * (squareCenterX - centerX) +
                (squareCenterY - centerY) * (squareCenterY - centerY));
            float scale = min(1, distance / 320 / 0.8);
            float rotation =45+ atan2((squareCenterX - centerX),(squareCenterY - centerY)) / pi() * 180 + distance / 310 * sqrt(2) * 90;
            rect(x, y, cell, cell, "black", scale, rotation);
        }
    }
}
""";
}
