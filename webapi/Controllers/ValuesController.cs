﻿using System;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using grains;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		private readonly IClusterClient _client;

		public ValuesController(IClusterClient client)
		{
			_client = client;
		}

		[HttpGet("[action]/{id}")]
		public async Task<object> GetInfo(long id)
		{
			var userGrain = _client.GetGrain<IUserGrain>(id);
			return await userGrain.GetInfo();
		}

		[HttpPut("[action]/{id}")]
		public async Task<object> UpdateInfo(long id, string name, int age)
		{
			var userGrain = _client.GetGrain<IUserGrain>(id);
			return await userGrain.UpdateInfo(new UserInfo { Name = name, Age = age });
		}

		[HttpPut("[action]")]
		public async Task<object> Transfer(long from, long to, uint amount)
		{
			var atm = _client.GetGrain<IATMGrain>(0);
			
			try
			{
				await atm.Transfer(from, to, amount);
			}
			catch (Exception ex)when(ex.InnerException is InvalidOperationException)
			{
				return ex.InnerException.Message;
			}

			var fromBalance = await _client.GetGrain<IAccountGrain>(from).GetBalance();
			var toBalance = await _client.GetGrain<IAccountGrain>(to).GetBalance();
			
			return new Dictionary<string, uint>()
			{
				["User" + from + " Balance"] = fromBalance,
				["User" + to + " Balance"] = toBalance,
			};
		}

		[HttpPut("[action]")]
		public async Task<object> Deposit(long id, uint amount)
		{
			var atm = _client.GetGrain<IATMGrain>(0);
			
			try
			{
				await atm.Depoist(id, amount);
			}
			catch (Exception ex)when(ex.InnerException is InvalidOperationException)
			{
				return ex.InnerException.Message;
			}

			var balance = await _client.GetGrain<IAccountGrain>(id).GetBalance();
			
			return new Dictionary<string, uint>()
			{
				["User" + id + " Balance"] = balance,
			};
		}

		[HttpGet("[action]/{id}")]
		public async Task<object> GetAccountInfo(long id)
		{
			var userGrain = _client.GetGrain<IUserGrain>(id);
			return await userGrain.GetBalance();
		}
	}
}