namespace WarOfClans
{
    public interface IAIInfantry : IInfantry
    {
        void Init(InfantryData infantryData, INavigation navigationController);        
    }
}
