using Application.Configurations;
using Application.DTOs;
using Application.Errors;
using Application.Service;
using Domain.Enums;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Json;

namespace TicTacToeTests
{
    public class GameServiceTests
    {
        private readonly Mock<IUnitOfWorkRepository> _unitOfWorkMock;
        private readonly GameService _gameService;
        private readonly Mock<IOptions<GameSettings>> _settingsMock;
        private string[][] board = new string[3][] { new string[] { "X", "X", "" }, new string[3], new string[3] };

        public GameServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWorkRepository>();
            _settingsMock = new Mock<IOptions<GameSettings>>();
            _settingsMock.Setup(s => s.Value).Returns(new GameSettings
            {
                BoardSize = 3,  
                WinCondition = 3
            });

            _gameService = new GameService(_unitOfWorkMock.Object, _settingsMock.Object);
        }

        [Fact]
        public void CreateGame_ShouldReturnValidGameDto_WhenValidSettings()
        {
            //Arrange
            //Act
            var gameDto = _gameService.CreateGame();

            //Assert
            Assert.NotNull(gameDto);
            Assert.Equal(3, gameDto.BoardSize);
            Assert.Equal(3, gameDto.WinCondition);
            Assert.Equal(GameStatus.InProgress, gameDto.Status);
            Assert.NotEqual(gameDto.PlayerX, Guid.Empty);
            Assert.NotEqual(gameDto.PlayerO, Guid.Empty);
            Assert.NotNull(gameDto.Board);
        }

        [Fact]
        public void CreateGame_ShouldThrowException_WhenInvalidSettings()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWorkRepository>();
            var invalidOptions = Options.Create(new GameSettings
            {
                BoardSize = 2,    
                WinCondition = 2  
            });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new GameService(unitOfWorkMock.Object, invalidOptions));
        }

        [Fact]
        public void MakeMove_ShouldReturnError_WhenGameIsNotInProgress()
        {
            board[0][2] = "X";
            // Arrange
            var game = new GameDto (3)
            {
                Status = GameStatus.WonByX,
                PlayerTurn = Guid.NewGuid()
            };
            var request = new MoveRequest
            {
                Row = 1,
                Column = 1,
                Player = Guid.NewGuid(),
                Symbol = Symbols.X
            };

            // Act
            var result = _gameService.MakeMove(game, request);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(new GameErrors().GameIsAlreadyOver().GetType(), result.Error.GetType());
        }

        [Fact]
        public void MakeMove_ShouldReturnError_WhenCellIsOccupied()
        {
            //Arrange
            var playerX = Guid.NewGuid();
            var playerO = Guid.NewGuid();
            var game = new GameDto(3)
            {
                Status = GameStatus.InProgress,
                Board = JsonSerializer.Serialize(board),
                BoardSize = 3,
                WinCondition = 3,
                PlayerTurn = playerX,
                PlayerO = playerO,
                PlayerX = playerX,
            };

            var request = new MoveRequest
            {
                Row = 0,
                Column = 0,
                Symbol = Symbols.X,
                Player = game.PlayerTurn
            };

            // Act
            var result = _gameService.MakeMove(game, request);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(new GameErrors().CageOccupied().GetType(), result.Error.GetType());
        }


        [Fact]
        public void MakeMove_ShouldUpdateGame_WhenMoveIsValid()
        {
            // Arrange
            var playerX = Guid.NewGuid();
            var playerO = Guid.NewGuid();
            var game = new GameDto(3)
            {
                Status = GameStatus.InProgress,
                Board = JsonSerializer.Serialize(board),
                BoardSize = 3,
                WinCondition = 3,
                PlayerTurn = playerX,
                PlayerO = playerO,
                PlayerX = playerX,
            };

            var request = new MoveRequest
            {
                Row = 1,
                Column = 1,
                Symbol = Symbols.X,
                Player = game.PlayerTurn
            };

            // Act
            var result = _gameService.MakeMove(game, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Move); 
            var updatedBoard = JsonSerializer.Deserialize<string[][]>(game.Board);
            Assert.NotNull(updatedBoard);
            Assert.Equal("X", updatedBoard![request.Row][request.Column]);
        }

        [Fact]
        public void CheckWin_ShouldReturnTrue_WhenPlayerHasWon()
        {
            // Arrange
            board[0][2] = "X";
            var playerX = Guid.NewGuid();
            var playerO = Guid.NewGuid();
            var game = new GameDto(3)
            {
                Status = GameStatus.InProgress,
                Board = JsonSerializer.Serialize(board),
                BoardSize = 3,
                WinCondition = 3,
                PlayerTurn = playerX,
                PlayerO = playerO,
                PlayerX = playerX,
            };

            var request = new MoveRequest
            {
                Row = 1,
                Column = 2,
                Symbol = Symbols.X,
                Player = game.PlayerTurn
            };

            // Act
            var result = _gameService.MakeMove(game, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(GameStatus.WonByX, game.Status);
        }


        [Fact]
        public void MakeMove_ShouldSwitchSymbol_OnThirdMove()
        {
            // Arrange
            var playerX = Guid.NewGuid();
            var playerO = Guid.NewGuid();
            var game = new GameDto(3)
            {
                Status = GameStatus.InProgress,
                Board = JsonSerializer.Serialize(board),
                BoardSize = 3,
                WinCondition = 3,
                PlayerTurn = playerX,
                PlayerO = playerO,
                PlayerX = playerX,
            };

            var request = new MoveRequest
            {
                Row = 1,
                Column = 2,
                Symbol = Symbols.X,
                Player = game.PlayerTurn
            };

            // Act
            var result = _gameService.MakeMove(game, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Move);
            Assert.Contains(result.Move.Symbol, new[] { Symbols.X, Symbols.O });  
        }

    }
}
