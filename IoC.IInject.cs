namespace Levels.Universal {

    using System;

    public interface IInject {
        void Receive(object[] values);

        (Type, string)[] Request();
    }
}