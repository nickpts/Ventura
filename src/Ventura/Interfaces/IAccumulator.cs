namespace Ventura.Interfaces
{
    interface IAccumulator
    {
        bool HasEnoughEntropy { get; }
        void Distribute();
        byte[] GetRandomDataFromPools();
    }
}
