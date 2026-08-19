namespace ScriptedSvgWpf.Samples;

public static class CheckerboardScript
{
    public const string Source = """
canvas(640, 640, "white");
int rows = 12;
float cell = 48;
float centerX = 320;
float centerY = 320;

for (int row = 0; row < rows; row = row + 1) {
    for (int column = 0; column < rows; column = column + 1) {
        if ((row + column) % 2 == 0) {
            float x = column * cell;
            float y = row * cell;
            float squareCenterX = x + cell / 2;
            float squareCenterY = y + cell / 2;
            float distance = sqrt(
                (squareCenterX - centerX) * (squareCenterX - centerX) +
                (squareCenterY - centerY) * (squareCenterY - centerY));
            float scale = min(1, distance / 450);
            float rotation = (row * rows + column) * 7;
            rect(x, y, cell, cell, "black", scale, rotation);
        }
    }
}
""";
}
