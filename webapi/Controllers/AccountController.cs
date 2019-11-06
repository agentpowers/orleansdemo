using System;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using grains;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	public class AccountController : Controller
	{
		private readonly IClusterClient _client;

		public AccountController(IClusterClient client)
		{
			_client = client;
		}

		[HttpPut("[action]")]
		public async Task<ActionResult> Transfer(long from, long to, uint amount)
		{
			var atm = _client.GetGrain<IATMGrain>(0);
			
			try
			{
				await atm.Transfer(from, to, amount);
			}
			catch (Exception ex)when(ex.InnerException is InvalidOperationException)
			{
				return BadRequest(ex.InnerException.Message);
			}

			var fromBalance = await _client.GetGrain<IAccountGrain>(from).GetBalance();
			var toBalance = await _client.GetGrain<IAccountGrain>(to).GetBalance();
			
			return Ok(new Dictionary<string, uint>()
			{
				["User" + from + " Balance"] = fromBalance,
				["User" + to + " Balance"] = toBalance,
			});
		}

		[HttpPut("[action]")]
		public async Task<ActionResult> Deposit(long id, uint amount)
		{
			var atm = _client.GetGrain<IATMGrain>(0);
			
			try
			{
				await atm.Depoist(id, amount);
			}
			catch (Exception ex)when(ex.InnerException is InvalidOperationException)
			{
				return BadRequest(ex.InnerException.Message);
			}

			var balance = await _client.GetGrain<IAccountGrain>(id).GetBalance();
			
			return Ok(new Dictionary<string, uint>()
			{
				["User" + id + " Balance"] = balance,
			});
		}

		[HttpGet("[action]/{id}")]
		public async Task<uint> Balance(long id)
		{
			var userGrain = _client.GetGrain<IUserGrain>(id);
			return await userGrain.GetBalance();
		}
	}
}