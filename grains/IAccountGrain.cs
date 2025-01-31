﻿using System.Threading.Tasks;
using Orleans;

namespace grains
{
    public interface IAccountGrain : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Join)]
        Task Withdraw(uint amount);

        [Transaction(TransactionOption.Join)]
        Task Deposit(uint amount);

        [Transaction(TransactionOption.Create)]
        Task<uint> GetBalance();
    }
}
