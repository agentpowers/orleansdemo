using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace grains
{
    [StatelessWorker]
    public class ATMGrain : Grain, IATMGrain
    {
        Task IATMGrain.Transfer(long fromAccount, long toAccount, uint amountToTransfer)
        {
            return Task.WhenAll(
                this.GrainFactory.GetGrain<IAccountGrain>(fromAccount).Withdraw(amountToTransfer),
                this.GrainFactory.GetGrain<IAccountGrain>(toAccount).Deposit(amountToTransfer));
        }

        Task IATMGrain.Depoist(long account, uint amount)
        {
            return this.GrainFactory.GetGrain<IAccountGrain>(account).Deposit(amount);
        }
    }
}
