using DoiLibrary.Domain;
using DoiLibrary.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doi_WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DoiController : Controller
	{
		private readonly IDoiHttpClient _doiHttpClient;
		private readonly ILogger<DoiController> _logger;

		public DoiController(IDoiHttpClient doiClient, ILogger<DoiController> logger)
		{
			_doiHttpClient = doiClient;
			_logger = logger;
		}

		// POST api/<DoiController>
		[HttpPost]
		public async Task<ActionResult> PostDoi(Attributes doiAttributes)
		{
			try
			{
				var result = await _doiHttpClient.CreateDoi(doiAttributes);
				return Ok(result);
			}
			catch (FormatException fe)
			{
				_logger.LogError($"Something went wrong: {fe}");
				return StatusCode(500, "Internal server error");
			}
			//remove catch-Exception => in Debug-mode swagger shows stacktrace and remote server Http-response-Code:
			//catch (Exception ex)
			//{
			//    //var feature = this.HttpContext.Features.Get<IExceptionHandlerFeature>(); //maybe it is possible to get previous HTTP-response-Code and foreward?

			//    //Trying to request already existing Doi, results in HTTP error 422 (Unprocessable Entity). Do we want to show in response?

			//    _logger.LogError($"Something went wrong calling remote Api-Server: {ex}");
			//    return StatusCode(500, "Internal server error");
			//}
		}

		//// GET: DoiController
		//public ActionResult Index()
		//{
		//	return View();
		//}

		//// GET: DoiController/Details/5
		//public ActionResult Details(int id)
		//{
		//	return View();
		//}

		//// GET: DoiController/Create
		//public ActionResult Create()
		//{
		//	return View();
		//}

		//// POST: DoiController/Create
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Create(IFormCollection collection)
		//{
		//	try
		//	{
		//		return RedirectToAction(nameof(Index));
		//	}
		//	catch
		//	{
		//		return View();
		//	}
		//}

		//// GET: DoiController/Edit/5
		//public ActionResult Edit(int id)
		//{
		//	return View();
		//}

		//// POST: DoiController/Edit/5
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Edit(int id, IFormCollection collection)
		//{
		//	try
		//	{
		//		return RedirectToAction(nameof(Index));
		//	}
		//	catch
		//	{
		//		return View();
		//	}
		//}

		//// GET: DoiController/Delete/5
		//public ActionResult Delete(int id)
		//{
		//	return View();
		//}

		//// POST: DoiController/Delete/5
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Delete(int id, IFormCollection collection)
		//{
		//	try
		//	{
		//		return RedirectToAction(nameof(Index));
		//	}
		//	catch
		//	{
		//		return View();
		//	}
		//}
	}
}
