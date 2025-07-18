namespace Application.Errors
{
    public class GameErrors
    {
        public GameError GameIsAlreadyOver() => new()
        {
            Message = "Игра закончена или не начата.",
            ErrorCode = ErrorCodes.GameIsOver
        };

        public GameError BoardNotCreated() => new()
        {
            Message = "Доска не была создана.",
            ErrorCode = ErrorCodes.BoardNotCreated
        };

        public GameError InvalidMove() => new()
        {
            Message = "Невозможно определить чей ход.",
            ErrorCode = ErrorCodes.InvalidMove
        };

        public GameError ErrorDeserializing() => new()
        {
            Message = "Ошибка при десериализации.",
            ErrorCode = ErrorCodes.ErrorDeserializing
        };

        public GameError CageOccupied() => new()
        {
            Message = "Клетка уже занята",
            ErrorCode = ErrorCodes.CageOccupied
        };

        public GameError InvalidPlayers() => new()
        {
            Message = "Невозможно определить игроков.",
            ErrorCode = ErrorCodes.InvalidPlayers
        };

        public GameError MoveOutsideBoard() => new()
        {
            Message = "Ход за пределами доски.",
            ErrorCode = ErrorCodes.MoveOutsideBoard
        };

        public GameError OtherMove() => new()
        {
            Message = "Сейчас не ваш ход.",
            ErrorCode = ErrorCodes.OtherMove
        };

        public GameError IncorrectGameSettings() => new()
        {
            Message = "Некорректные настройки игры.",
            ErrorCode = ErrorCodes.IncorrectGameSettings
        };

        public GameError InvalidSymbol() => new()
        {
            Message = "Неверный символ.",
            ErrorCode = ErrorCodes.InvalidSymbol
        };
    }
}
