using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MoeAtHome.Web.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MoeAtHome.Web.Controllers
{
    [Route("api/[controller]")]
    public class LyricsController : Controller
    {
        private readonly LyricsService _lyricsService;

        public LyricsController(LyricsService lyricsService)
        {
            _lyricsService = lyricsService;
        }

        // GET api/lyrics/5
        [HttpGet("{title}/{artist}")]
        public async Task<IActionResult> Get(string title, string artist)
        {
            if (string.IsNullOrEmpty(title))
                return HttpBadRequest();

            var lrc = await _lyricsService.Find(title, artist);
            if (lrc == null)
                return HttpNotFound();
            return File(_lyricsService.OpenRead(lrc), "text/lrc", $"{title}-{artist}.lrc");
        }
    }
}
