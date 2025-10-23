using Microsoft.AspNetCore.Mvc;
using CMCSProject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
		
namespace CMCSProject.Controllers
{
	public class ClaimController : Controller
	{
		private static List<Claim> claims = new List<Claim>();
		private readonly ILogger<ClaimController> _logger;

		public ClaimController(ILogger<ClaimController> logger)
		{
			_logger = logger;
		}
		public IActionResult Index()
		{
			_logger.LogInformation("Loading Claim submission page...");
			return View("~/Views/Claim/Index.cshtml", new Claim());
		}

		[HttpPost]
		public IActionResult Submit(Claim claim, IFormFile document)
		{
			_logger.LogInformation("Submit() called at {time}", DateTime.Now);

			if (!ModelState.IsValid)
			{
				_logger.LogWarning("ModelState invalid. Returning Claim/Index view.");
				return View("~/Views/Claim/Index.cshtml", claim);
			}
			try
			{
				if (document != null && document.Length > 0)
				{

					var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
					if (!Directory.Exists(uploadsDir))
						Directory.CreateDirectory(uploadsDir);

					var filePath = Path.Combine(uploadsDir, Path.GetFileName(document.FileName));
					using (var stream = new FileStream(filePath, FileMode.Create))
						document.CopyTo(stream);

					claim.UploadedDocument = document.FileName;
				}

				claim.Id = claims.Count > 0 ? claims.Max(c => c.Id) + 1 : 1;
				claim.Status = "Pending";
				claim.DateSubmitted = DateTime.Now;
				claims.Add(claim);

				TempData["Success"] = "Claim submitted successfully!";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error submitting claim.");
				TempData["Error"] = $"An Error occcured: {ex.Message}";
			}
			return RedirectToAction("Track");

		}

		public IActionResult Track()
		{
			return View("~/Views/Claim/Index.cshtml", claims);
		}

		public IActionResult Verify()
		{
			return View("~/Views/Claim/Index.cshtml", claims);
		}

		public IActionResult Approve()
		{
			return View("~/Views/Claim/Index.cshtml", claims);
		}

	}
}
