from PlayerType import *

class Connect4Board(object):
    """Contains all the board data and methods"""

    # ----------------------------------------------------------------------------------------------------------------

    # Class constructor
    def __init__(self, nRows, nColumns):
        self.nRows = 5
        self.nColumns = 6
        self.Board = [[0] * (self.nColumns + 1)] * (self.nRows + 1)

    # ----------------------------------------------------------------------------------------------------------------

    # Prints the Board
    def Show(self):
        for i in self.Board:
            print(i)

    # ----------------------------------------------------------------------------------------------------------------

    # player: 1 for computer and 2 for oponent
    def PlaceChipInBoard(self, row, column, playerType):
        self.Board[row][column] = playerType;

    # -----------------------------------------------------------------------------------------------------------------

    # Checks if there is a solution with the new chip added
    def IsWinningMovement(self, row, column):

        if (IsWinningColumn(row, column) or IsWinningRow(row, column) or IsWinningDiagonal(row, column)):
            return True

        return 0

    # -----------------------------------------------------------------------------------------------------------------

    # Verifies if it's a winning row
    def IsWinningRow(self, row, column):
        
        playerType = self.Board[row][column]
        result = False

        # Check 3 chips to the right associated to the same player
        if (column + 3 <= self.nColumns):
            for nextColumn in range(1, 4):
                if (self.Board[row][column + nextColumn]) != playerType:
                    result = False
                    break
                result = True
        
            if result:
                return True

        # Check 3 chips to the left associated to the same player
        if (column - 3 >= 0):
            for previousColumn in range(1, 4):
                if (self.Board[row][column - previousColumn]) != playerType:
                    result = False
                    break
                result = True

    # -----------------------------------------------------------------------------------------------------------------

    # Verifies if it's a winning column
    def IsWinningColumn(self, row, column):
        
        playerType = self.Board[row][column]
        
        # Check 3 chips below associated to the same player
        if (row + 3 <= self.nColumns):
            for lowerRow in range(1, 4):
                if (self.Board[row + lowerRow][column]) != playerType:
                    return False
        return True

    # -----------------------------------------------------------------------------------------------------------------

    # Verifies if it's a winning diagonal
    def IsWinningDiagonal(self, row, column):
        
        playerType = self.Board[row][column]
        result = False

        # Check 3 chips above associated to the same player
        if (row - 3 >= 0 and column + 3 <= self.nColumns):
            for upperRow in range(1, 4):
                if (self.Board[row - upperRow][column + upperRow]) != playerType:
                    result = False
                    break
                result = True
            
            if result:
                return True
        
        # Check 3 chips below associated to the same player
        if (row + 3 <= self.nColumns and column - 3 >= 0):
            for lowerRow in range(1, 4):
                if (self.Board[row + lowerRow][column - lowerRow]) != playerType:
                    result = False
                    break
                result = True
        return result

    # -----------------------------------------------------------------------------------------------------------------

    # Add a chip to the board if possible
    def AddChip(self, row, column, playerType):
        if ((0 <= row <= self.nRows and 0 <= column <= self.nColumns) and self.Board[row][column] == 0):                                          # Empty space 
               if (row < 6 and self.Board[6][column] != 0) or row == 6:      # There's a chip below the new one  
                   self.Board[row][column] = playerType

    # -----------------------------------------------------------------------------------------------------------------

