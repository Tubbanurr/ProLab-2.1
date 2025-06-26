#include "data.h"
int count = 0;

// once grid celleri cizilir ve ardindan biome cizgileri cizilir.
void draw_grids(t_data *data)
{
    for (int i = 0; i < HEIGHT; i+= data->square_height)
    {
        DrawLine(0, i, WIDTH, i, LIGHTGRAY);
        for (int j = 0; j < WIDTH; j+= data->square_width)
        {
            DrawLine(j, 0, j, HEIGHT, LIGHTGRAY);
        }
    }

    for (int i = 0; i < data->biome_height * BIOME_HEIGHT; i+=4)
    {
        DrawLine(0, i * data->square_height, WIDTH, i * data->square_height, GRAY);
        for (int j = 0; j < data->biome_width * BIOME_WIDTH; j+=4)
        {
            DrawLine(j * data->square_width, 0, j * data->square_width, HEIGHT, GRAY);
        }
    }
}

// tum noktalari bulur ve bunlari bir arraye atar.
Vector2 *find_all_dots(char *input, t_data *data)
{
    Vector2 *all_points;

    for (int i = 0; i < strlen(input); i++)
    {
        if (input[i] == '(')
            count++;
    }
    all_points = malloc(sizeof(Vector2) * count);
    count = 0;

    for (int i = 0; i < strlen(input); i++)
    {
        if (input[i] == '(')
        {
            int x = 0, y = 0;
            sscanf(&input[i + 1], "%d,%d", &x, &y);
            all_points[count].x = x * data->square_width;
            all_points[count].y = y * data->square_height;
            count++;
        }
    }
    return all_points;
}

// tum noktalari cizer. Ayni noktaya tekrar geldiginde ise cizgiyi cizmez.
void    draw_data(Vector2 *dots, t_data *data)
{
    Vector2 begin = dots[0];
    int counter = 0;
    for (int i = 0; i < count - 1 ; i++)
    {
        Vector2 start = {dots[i].x, dots[i].y};
        Vector2 end = {dots[i + 1].x, dots[i + 1].y};
        if (begin.x == start.x && begin.y == start.y && ++counter == 2)
        {
            begin = end;
            counter = 0;
        }
        else
        {
            DrawLineEx(start, end, 2, BLUE);
            DrawText(TextFormat("(%.0f,%.0f)", dots[i].x / data->square_width, dots[i].y / data->square_height), dots[i].x + 15, dots[i].y - 15, 24, GREEN);
        }
    }

    for (int i = 0; i < count; i++)
    {
        DrawCircle(dots[i].x, dots[i].y, 5, RED);
    }
}

int main(void) {

    char *input = "2B(5,5)(13,12)(8,17)(1,10)(5,5)(20,20)(30,20)(20,40)(10,40)(20,20)(40,22)(50,32)(30,32)(40,22)F";
    t_data *game = calculate_data(input);
    Vector2 *all_points = find_all_dots(input, game);
    InitWindow(WIDTH, HEIGHT, "Canodis");

    while (!WindowShouldClose()) {

        BeginDrawing();

        ClearBackground(WHITE);

        draw_grids(game);

        draw_data(all_points, game);

        EndDrawing();
    }

    CloseWindow();

    return 0;
}
// 1B(5,5)(13,12)(8,17)(1,10)(5,5)F
