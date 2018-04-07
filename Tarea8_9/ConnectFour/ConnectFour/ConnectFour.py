from Connect4Board import *


board = Connect4Board(6, 7)

while True:

    board.Show()

    # Machine movements
    machRow = int(input("Machine Row: ")) - 1
    machColumn = int(input("Machine Column: ")) - 1

    board.AddChip(machRow, machColumn, PlayerType.Computer)

    board.Show();

    # Human movements

    humanRow = int(input("Human Row: ")) - 1
    humanColumn = int(input("Human Column: ")) - 1
    board.AddChip(humanRow, humanColumn, PlayerType.Oponent)