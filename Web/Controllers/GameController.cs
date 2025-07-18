using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Web.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IMoveService _moveService;

        public GameController(IGameService gameService, IMoveService moveService)
        {
            _gameService = gameService;
            _moveService = moveService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame()
        {
            var game = _gameService.CreateGame();
            await _gameService.AddAsync(game);
            return CreatedAtAction(nameof(CreateGame), new { id = game.Id }, game);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game);
        }

        [HttpPost("{id}/moves")]
        public async Task<IActionResult> MakeMove(Guid id, [FromBody] MoveRequest request)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            string hash = GetRequesthash(request);
            var move = game.Moves.FirstOrDefault(x => x.Row == request.Row && x.Column == request?.Column && x.Player == request.Player);
            if (move != null)
            {
                Response.Headers["ETag"] = hash;
                return Ok(move);
            }

            var (success, error, newMove, newGame) = _gameService.MakeMove(game, request);
            if (!success && error != null)
            {
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Неверный ход",
                    Detail = error.Message
                };
                return BadRequest(problem);
            }
            
            if (newMove == null)
            {
                return BadRequest();
            }
            await _moveService.AddAsync(newMove);
            await _gameService.Update(id,game);
            Response.Headers["ETag"] = hash;
            return Ok(newMove);
        }

        [HttpGet("{id}/moves")]
        public ActionResult<IEnumerable<MoveDto>> GetMoves(Guid id)
        {
            var game = _gameService.GetByIdAsync(id);
            if (game.Result == null)
            {
                return NotFound();
            }
            return Ok(game.Result.Moves);
        }

        private string GetRequesthash(MoveRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(hash);
        }
    }
}
