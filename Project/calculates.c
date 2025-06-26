#include "data.h"

// bu fonksiyon input stringindeki en kucuk ve en buyuk x ve y degerlerini bulur.
void    find_min_and_max(char *input, struct s_max_min *max_min)
{
    max_min->xmax = INT_MIN;
    max_min->ymax = INT_MIN;

    for (int i = 0; i < strlen(input); i++)
    {
        if (input[i] == '(')
        {
            int x = 0, y = 0;
            sscanf(&input[i + 1], "%d,%d", &x, &y);
            if (x > max_min->xmax)
                max_min->xmax = x;
            if (y > max_min->ymax)
                max_min->ymax = y;
        }
    }
}

// ekranin buyuklugune gore bir grid cell'in buyuklugu hesaplanir(square width, square height)
// ayrica yatayda ve dikeyde kac tane biome olacagi hesaplanir(biome width, biome height) bunun uzerine + 1 ekliyorum
// cunku ornekte de hep +1 aliyordu.
t_data    *calculate_data(char *input)
{
    int x;
    int y;

    struct s_max_min max_min;
    t_data *res = malloc(sizeof(t_data));

    find_min_and_max(input, &max_min);
    x = (max_min.xmax / BIOME_WIDTH) + (max_min.xmax % BIOME_WIDTH == 0 ? 0 : 1) + 1;
    y = (max_min.ymax / BIOME_HEIGHT) + (max_min.ymax % BIOME_HEIGHT == 0 ? 0 : 1) + 1;

    res->biome_width = x;
    res->biome_height = y;
    res->biome_count = x * y;
    res->biomes = malloc(sizeof(struct biome*) * res->biome_height);
    res->square_width = WIDTH / (x * BIOME_WIDTH);
    res->square_height = HEIGHT / (y * BIOME_HEIGHT);
    
    return (res);
}

// x = 4, y = 5 -> 20 biomes 20 * 9 = 180 total square a line square count 9 * 4 = 36 height square count 9 * 5 = 45
// screen heigth 600 screen width 800 600 / 45 = 13.33333333333333 a square width 800 / 36 = 22.22222222222222 a square width :)