NAME = game

SRC = main.c calculates.c

$(NAME): $(SRC)
	cc -g $(SRC) -o $(NAME) -lraylib -lGL -lm -lpthread -ldl -lrt -lX11

start: $(NAME)
	./$(NAME)

clean:
	rm -f $(NAME)

re: clean $(NAME)