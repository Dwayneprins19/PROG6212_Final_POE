using Microsoft.AspNetCore.Mvc;
using CMCSProject.Models;
using CMCSProject.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CMCSProject.Controllers
{
	public class ClaimController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly ILogger<ClaimController> _logger;
		private readonly IWebHostEnvironment _env;
		public ClaimController(ApplicationDbContext db, ILogger<ClaimController> logger, IWebHostEnvironment env)
		{
			_db = db;
			_logger = logger;
			_env = env;
		}
		public IActionResult Index()
		{
			return View("~/Views/Claim/Index.cshtml", new Claim());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public  async Task<IActionResult> Submit(Claim claim)
		{
			_logger.LogInformation("Submit() called at {time}", DateTime.Now);

			ModelState.Remove(nameof(claim.UploadedDocument));

			if (!ModelState.IsValid)
			{
				foreach (var kv in ModelState)
				{
					foreach (var error in kv.Value.Errors)
					{
						_logger.LogWarning("Field '{error}' error: {err}", kv.Key, error.ErrorMessage);
					}
				}
				return View("~/Views/Claim/Index.cshtml", claim);
			}

			var file = Request.Form.Files[nameof(claim.UploadedDocument)];

			if (file != null && file.Length > 0)
			{

				var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
				if (!Directory.Exists(uploadsDir))
					Directory.CreateDirectory(uploadsDir);

				var fileName = Path.GetFileName(file.FileName);
				var filePath = Path.Combine(uploadsDir, file.FileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				await file.CopyToAsync(stream);

				claim.UploadedDocument = fileName;

			}

				claim.DateSubmitted = DateTime.Now;
				claim.Status = "Pending";
				
				_db.Claims.Add(claim);
				await _db.SaveChangesAsync();

				TempData["Success"] = "Claim submitted successfully!";
				return RedirectToAction("Track");
		}
		public async Task<IActionResult> Track()
		{
			var claims = await _db.Claims.OrderByDescending(c => c.DateSubmitted).ToListAsync();
			return View("~/Views/Claim/Track.cshtml", claims);
		}

		public async Task<IActionResult> Verify()
		{
			var claims = await _db.Claims.OrderBy(c => c.Status).ThenByDescending(c => c.DateSubmitted).ToListAsync();
			return View("~/Views/Claim/Verify.cshtml", claims);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> VerifyClaim(int id)
		{
			var claim = await _db.Claims.FindAsync(id);
			if (claim != null)
			{
				claim.Status = "Verified";
				await _db.SaveChangesAsync();
				TempData["Info"] = $"Claim {id} verified.";
			}
			return RedirectToAction(nameof(Verify));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ApproveClaim(int id)
		{
			var claim = await _db.Claims.FindAsync(id);
			if (claim != null)
			{
				claim.Status = "Approved";
				await _db.SaveChangesAsync();
				TempData["Info"] = $"Claim {id} approved.";
			}
			return RedirectToAction(nameof(Approve));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RejectClaim(int id)
		{
			var claim = await _db.Claims.FindAsync(id);
			if (claim != null)
			{
				claim.Status = "Rejected";
				await _db.SaveChangesAsync();
				TempData["Info"] = $"Claim {id} rejected";
			}
			return RedirectToAction(nameof(Approve));
		}

		public async Task<IActionResult> Approve()
		{
			var claims = await _db.Claims.OrderBy(c => c.Status).ThenByDescending(c => c.DateSubmitted).ToListAsync();
			return View("~/Views/Claim/Approve.cshtml", claims);
		}

	}
}


