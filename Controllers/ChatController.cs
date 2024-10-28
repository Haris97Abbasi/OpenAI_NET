using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class ChatController : Controller
{
    private readonly OpenAiService _openAiService;

    public ChatController(OpenAiService openAiService)
    {
        _openAiService = openAiService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string query)
    {
        return View();
    }
}
