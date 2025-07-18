using System.ComponentModel.DataAnnotations;

namespace Application.Configurations
{
    public class GameSettings
    {
        [Range(3, 100, ErrorMessage = "Размер доски должен быть от 3 до 100")]
        public int BoardSize { get; set; }
        [Range(3, 100, ErrorMessage = "Условие победы должно быть от 3 до 100")]
        public int WinCondition { get; set; }
    }
}
