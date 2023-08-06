using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace praktikaylrik.Pages
{
    public class AddEvent : PageModel
    {
        private readonly ILogger<AddEvent> _logger;

        public AddEvent(ILogger<AddEvent> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}