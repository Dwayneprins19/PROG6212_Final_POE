using System.Diagnostics;
using CMCSProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace CMCSProject.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
