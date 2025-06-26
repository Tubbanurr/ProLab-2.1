#ifndef DATA_H
# define DATA_H

#include <raylib.h>
#include <raymath.h>
#include <rlgl.h>
#include <string.h>
#include <limits.h>
#include <stdio.h>
#include <stdlib.h> 

# define WIDTH 800
# define HEIGHT 600
# define BIOME_WIDTH 4
# define BIOME_HEIGHT 4


struct biome
{
    int x;
    int y;
};


struct s_max_min
{
    int xmax;
    int ymax;
};

typedef struct s_data
{
    int square_width;
    int square_height;
    int biome_width;
    int biome_height; 
    int biome_count;
    struct biome **biomes;
} t_data;



void    find_min_and_max(char *data, struct s_max_min *max_min);
t_data    *calculate_data(char *data);

#endif
