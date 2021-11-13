﻿namespace MyMoney.Application.Common.Interfaces
{
    public interface IContextAdapter
    {
        IEfCoreContext Context { get; }

        void RecreateContext();
    }
}
