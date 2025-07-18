using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Enums;
using Infrastructure.Persistence.Repositories.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Application.Configurations;
using Application.Errors;

namespace Application.Service
{
    public class GameService : IGameService
    {
        private readonly Random _random = new();
        private readonly IUnitOfWorkRepository _unitOfWork;
        private readonly GameSettings _settings;
        private readonly GameErrors _errors;

        public GameService(IUnitOfWorkRepository unitOfWork, IOptions<GameSettings> options)
        {
            _unitOfWork = unitOfWork;
            _settings = options.Value;
            _errors = new GameErrors();
            if (_settings.BoardSize <= 2 || _settings.WinCondition <= 2)
            {
                throw new InvalidOperationException(_errors.IncorrectGameSettings().Message);
            }
        }

        public GameDto CreateGame()
        {
            var playerX = Guid.NewGuid();
            return new GameDto(_settings.BoardSize)
            {
                Id = Guid.NewGuid(),
                BoardSize = _settings.BoardSize,
                WinCondition = _settings.WinCondition,
                Status = GameStatus.InProgress,
                PlayerX = playerX,
                PlayerO = Guid.NewGuid(),
                PlayerTurn = playerX
            };
        }

        public (bool Success, GameError? Error, MoveDto? Move, GameDto UpdatedGame) MakeMove(GameDto game, MoveRequest request)
        {
            if (game.Status != GameStatus.InProgress)
            {
                return (false, _errors.GameIsAlreadyOver(), null, game);
            }

            if (string.IsNullOrEmpty(game.Board))
            {
                return (false, _errors.BoardNotCreated(), null, game);
            }

            if (game.PlayerTurn == Guid.Empty)
            {
                return (false, _errors.InvalidMove(), null, game);
            }

            var board = JsonSerializer.Deserialize<string[][]>(game.Board);

            if (request.Row < 0 || request.Row >= game.BoardSize || request.Column < 0 || request.Column >= game.BoardSize)
            {
                return (false, _errors.MoveOutsideBoard(), null, game);
            }

            if (board == null)
            {
                return (false, _errors.ErrorDeserializing(), null, game);
            }

            if (!string.IsNullOrEmpty(board[request.Row][request.Column]))
            {
                return (false, _errors.CageOccupied(), null, game);
            }

            if ((game.PlayerX == Guid.Empty || game.PlayerO == Guid.Empty) && (game.PlayerX != request.Player && game.PlayerO != request.Player))
            {
                return (false, _errors.InvalidPlayers(), null, game);
            }

            if((request.Symbol == Symbols.X && request.Player == game.PlayerO) || (request.Symbol == Symbols.O && request.Player == game.PlayerX))
            {
                return (false, _errors.InvalidSymbol(), null, game);
            }

            Symbols symbol;
            Guid excpectedPlayer = Guid.Empty;
            if (request.Player == game.PlayerX)
            {
                excpectedPlayer = game.PlayerX;
                symbol = Symbols.X;
            }
            else
            {
                excpectedPlayer = game.PlayerO;
                symbol = Symbols.O;
            }

            if (game.PlayerTurn != excpectedPlayer)
            {
                return (false, _errors.OtherMove(), null, game);
            }

            int moveNumber = game.Moves.Count + 1;

            if (moveNumber % 3 == 0)
            {
                int chance = _random.Next(1, 11);
                if (chance == 1)
                {
                    if (symbol == Symbols.X)
                    {
                        symbol = Symbols.O;
                    }
                    else
                    {
                        symbol = Symbols.X;
                    }
                }
            }

            board[request.Row][request.Column] = symbol.ToString();

            var move = new MoveDto
            {
                GameId = game.Id,
                Player = request.Player,
                Row = request.Row,
                Column = request.Column,
                Symbol = symbol,
                Timestamp = DateTime.UtcNow
            };

            game.Moves.Add(move);
            game.Board = JsonSerializer.Serialize(board);

            bool hasWon = CheckWin(board, symbol, _settings.WinCondition);
            if (hasWon)
            {
                if (symbol == Symbols.X)
                {
                    game.Status = GameStatus.WonByX;
                }
                else
                {
                    game.Status = GameStatus.WonByO;
                }
            }
            else
            {
                bool isDraw = game.Moves.Count == game.BoardSize * game.BoardSize;
                if (isDraw)
                {
                    game.Status = GameStatus.Draw;
                }
                else
                {
                    if (game.PlayerTurn == request.Player)
                    {
                        game.PlayerTurn = game.PlayerO;
                    }
                    else
                    {
                        game.PlayerTurn = game.PlayerX;
                    }
                }
            }

            return (true, null, move, game);
        }

        private bool CheckWin(string[][] board, Symbols symbol, int winLength)
        {
            int n = board.GetLength(0);
            // Проверка по строкам, столбцам и диагоналям
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (CheckDirection(board, symbol, i, j, 1, 0, winLength) || // горизонталь
                        CheckDirection(board, symbol, i, j, 0, 1, winLength) || // вертикаль
                        CheckDirection(board, symbol, i, j, 1, 1, winLength) || // диагональ
                        CheckDirection(board, symbol, i, j, 1, -1, winLength))  // обратная диагональ
                        return true;
                }
            }
            return false;
        }

        private bool CheckDirection(string[][] board, Symbols symbol, int startRow, int startCol, int dRow, int dCol, int winLength)
        {
            int n = board.GetLength(0);
            int count = 0;
            for (int k = 0; k < winLength; k++)
            {
                int r = startRow + dRow * k;
                int c = startCol + dCol * k;
                if (r < 0 || r >= n || c < 0 || c >= n)
                    return false;
                if (board[r][c] != symbol.ToString())
                    return false;
                count++;
            }
            return count == winLength;
        }

        public async Task AddAsync(GameDto game)
        {
            await _unitOfWork.Games.AddAsync(game.MapToDomain());
            await _unitOfWork.SaveChangesAsync(); 
        }

        public Task RemoveAsync(GameDto game)
        {
             _unitOfWork.Games.Delete(game.MapToDomain());
            return _unitOfWork.SaveChangesAsync();
        }

        public async Task<GameDto?> GetByIdAsync(Guid id)
        {
            var game = await _unitOfWork.Games.GetByIdAsync(id);
            if(game == null)
            {
                return null;
            }
           return GameMapper.MapToDto(game);
        }

        public async Task<IEnumerable<GameDto>> GetAllAsync()
        {
            var games = await _unitOfWork.Games.GetAllAsync();
            return games.Where(x => x != null).Select(GameMapper.MapToDto);
        }

        public async Task Update(GameDto game)
        {
            _unitOfWork.Games.Update(game.MapToDomain());
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Update(Guid id, GameDto dto)
        {
            var game = await _unitOfWork.Games.GetByIdAsync(id);
            if(game == null)
            {
                return;
            }
            game.MapToDomain(dto);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
