namespace Ventura.Interfaces
{
    interface IAccumulator
    {
        bool HasEnoughEntropy { get; }
        byte[] GetRandomDataFromPools(int reseedCounter);
    }
}
