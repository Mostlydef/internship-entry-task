using Application.Configurations;
using Application.DTOs;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Web;

namespace TicTacToeTests
{
    public class GameApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public GameApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var customizedFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    var dbContextServices = services
                        .Where(s => s.ServiceType.FullName != null && s.ServiceType.FullName.Contains("AppDbContext"))
                        .ToList();
                    foreach (var s in dbContextServices)
                        services.Remove(s);

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });

                    services.Configure<GameSettings>(opts =>
                    {
                        opts.BoardSize = 3;
                        opts.WinCondition = 3;
                    });

                    var provider = services.BuildServiceProvider();

                    using var scope = provider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                });
            });

            _client = customizedFactory.CreateClient();
        }
        [Fact]
        public async Task CreateGame_ReturnsSuccess()
        {
            // Arrange
            var url = "/api/Game";

            //Act
            var response = await _client.PostAsync(url, null);

            //Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CreateGame_ThenMakeMove_ShouldReturnUpdatedGame()
        {
            // Arrange
            var url = "/api/Game";

            // Act
            var createResponse = await _client.PostAsync(url, null);
            createResponse.EnsureSuccessStatusCode();

            var createdGame = await createResponse.Content.ReadFromJsonAsync<GameDto>();

            Assert.NotNull(createdGame);
            Assert.Equal(GameStatus.InProgress, createdGame.Status);

            var moveRequest = new MoveRequest
            {
                Column = 0,
                Row = 0,
                Player = createdGame.PlayerTurn,
                Symbol = Symbols.X
            };

            var moveResponse = await _client.PostAsJsonAsync($"/api/Game/{createdGame.Id}/moves", moveRequest);
            moveResponse.EnsureSuccessStatusCode();

            var move = await moveResponse.Content.ReadFromJsonAsync<MoveDto>();

            // Assert
            Assert.NotNull(move);
            Assert.Equal(createdGame.Id, move.GameId);
        }

        [Fact]
        public async Task MakeMove_WithBadJson_ReturnsBadRequest()
        {
            // Arrange
            var gameCreateResponse = await _client.PostAsync("/api/Game", null);
            gameCreateResponse.EnsureSuccessStatusCode();

            var game = await gameCreateResponse.Content.ReadFromJsonAsync<GameDto>();
            Assert.NotNull(game);

            var badJson = "{ Player: , Row: 0, Col: ";

            var content = new StringContent(badJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/api/Game/{game.Id}/moves", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task MakeMove_WithInvalidJson_ReturnsBadRequest()
        {
            // Arrange
            var gameCreateResponse = await _client.PostAsync("/api/Game", null);
            gameCreateResponse.EnsureSuccessStatusCode();

            var game = await gameCreateResponse.Content.ReadFromJsonAsync<GameDto>();
            Assert.NotNull(game);

            var badJson = """
                    {
                        "Player": 123,
                        "Row": "zero",
                        "Col": null
                    }
                    """;

            var content = new StringContent(badJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/api/Game/{game.Id}/moves", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal(400, problem.Status);
        }
    }
}
