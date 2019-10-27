using System;
using System.Threading.Tasks;
using Orleans;

namespace grains
{
    public interface IATMGrain : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        Task Transfer(long fromAccount, long toAccount, uint amountToTransfer);

        [Transaction(TransactionOption.Create)]
        Task Depoist(long accout, uint amount);
    }
}
