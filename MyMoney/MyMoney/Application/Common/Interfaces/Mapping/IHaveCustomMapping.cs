using AutoMapper;

namespace MyMoney.Application.Common.Interfaces.Mapping
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile configuration);
    }
}
