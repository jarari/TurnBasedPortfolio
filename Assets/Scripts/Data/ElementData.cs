using System;

namespace TurnBased.Data {
    [Flags]
    public enum ElementType {
        None,
        Physical        =       1 << 0,
        Fire            =       1 << 1,
        Ice             =       1 << 2,
        Lightning       =       1 << 3,
        Wind            =       1 << 4,
        Quantum         =       1 << 5,
        Imaginary       =       1 << 6
    }
}
