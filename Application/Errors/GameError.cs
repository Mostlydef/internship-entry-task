using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Errors
{
    public class GameError
    {
        public string Message { get; set; } = string.Empty;
        public ErrorCodes ErrorCode { get; set; }
    }
}
